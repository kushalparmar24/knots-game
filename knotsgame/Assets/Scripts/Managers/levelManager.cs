using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class levelManager : MonoBehaviour
{
    static levelManager instance;
    public enum GameState { PLAYING, LOADING, COMPLETED,PAUSED };
    public GameState gameState;
    [SerializeField] public Tilemap brickTileMap, BGTileMap, nodeTileMap, lineTileMap;
    [SerializeField] colorNodeData colorNodeData;
    [SerializeField] GameObject cursor;
    List<baseKnots> usedBaseKnots = new List<baseKnots>();
    baseKnots currentNode;
    bool pressing, canDraw;
    ArrayLayout levels;
    int xStart;
    int yStart;
    int row;
    int col;
    Dictionary<int, List<Vector3Int>> levelNods;
    Vector3Int currentPos, prevPos, cell;
    Tile currentBrickTile, currentNodeTile, currentWallTile;
    #region getters
    public List<baseKnots> getUsedBaseKnots()
    {
        return usedBaseKnots;
    }
    public colorNodeData GetColorNodeData()
    {
        return colorNodeData;
    }

    public static levelManager Instance { get { return instance; } }
    #endregion

    #region private methods
    private void Awake()
    {
        if (instance == null)
            instance = this;
        //if (instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //DontDestroyOnLoad(this);

        gameState = GameState.LOADING;
        currentNode = null;
        setLevel();
    }
    /// <summary>
    /// based on Current level, sets the level on the screen. takes the 2D array level data stored in the game manager
    /// </summary>
    void setLevel()
    {
        gameState = GameState.LOADING;
        UIInGameManager.Instance.setLevelText();
        levels = gameManager.Instance.getlevelDatas()[gameManager.Instance.currentLevel];
        levelNods = new Dictionary<int, List<Vector3Int>>();
        row = levels.rows[0].row.GetLength(0);
        col = levels.rows.GetLength(0);
        xStart = (row / 2)+1;
        yStart = (col / 2)+1;
        //these Loops Does multiple things. its places all the base tiles based of size of array. Also checks the id present on the array and places the node tiles.
        //based on the count of unique ID present on the array, creates same count of baseknots.
        for (int j = 0; j < row; j++)
        {
            for (int i = 0; i < col; i++)
            {
                Vector3Int cell = new Vector3Int(j-xStart, (col - i)-yStart, 0);
                BGTileMap.SetTile(cell, colorNodeData.getBGTile());
                if (levels.rows[i].row[j]< 100)
                    continue;
                if (!levelNods.ContainsKey(levels.rows[i].row[j]))
                {
                    List<Vector3Int> temp = new List<Vector3Int>();
                    temp.Add(new Vector3Int(i, j, 0));
                    levelNods.Add(levels.rows[i].row[j], temp);

                    nodeTileMap.SetTile(cell, getNodetile(levels.rows[i].row[j]).getNodeTile());
                    createKnotFromID(levels.rows[i].row[j]);
                }
                else
                {
                   // levelNods.TryGetValue(level[i, j], out List<Vector3Int> temp);
                    //temp.Add(new Vector3Int(i, j, 0));
                    nodeTileMap.SetTile(cell, getNodetile(levels.rows[i].row[j]).getNodeTile());
                }
            }
        }
        gameState = GameState.PLAYING;
    }
    /// <summary>
    /// add the one knot of the usedknots list.
    /// </summary>
    void createKnotFromID(int id_)
    {
        colorIDData colorIDDatas = colorNodeData.colorIDDatasList().FirstOrDefault(x => x.getID() == id_);
        baseKnots cbaseKnots = new baseKnots(colorIDDatas.getID(), colorIDDatas.getBrickTile(), colorIDDatas.getNodeTile());
        usedBaseKnots.Add(cbaseKnots);
    }

    /// <summary>
    /// based on the id, fetches the colordata from SO
    /// </summary>
    colorIDData getNodetile(int id_)
    {
        return colorNodeData.colorIDDatasList().FirstOrDefault(x => x.getID() == id_);
    }

   
    
    /// <summary>
    /// process the first touch and existing touch based on user input position.
    /// </summary>
    void processTouch(Vector3 pos_)
    {
        cell = brickTileMap.WorldToCell(pos_);
        currentPos = cell;
        cursor.transform.position = new Vector2(pos_.x, pos_.y); ;
        if (currentPos == prevPos)//makes sure that all the Methods gets called only once per cell visit for optimization purpose.
            return;
        //Debug.Log("moving");
        setTileMapCells();
        checkForNodeTouch(pos_);
        if (!pressing)
        {
            checkForLineTouch();
        }

        if(pressing)
        {
            if (prevPos == currentPos || currentWallTile == null)
                return;
            checkSwipingNodes();
            checkAddOrRemove();
           // canDraw = true;
        }
        prevPos = currentPos;
    }

    /// <summary>
    /// one user input is stopped it resets some variables and current knot.
    /// </summary>
    void processStopTouch(Vector3 pos_)
    {
        pressing = false;
        canDraw = false;
        cursor.SetActive(false);
        for (int i = 0; i < usedBaseKnots.Count; i++)
        {
            usedBaseKnots[i].resetdict();
        }
        currentNode = null;
        prevPos = new Vector3Int(-10000, -100000, -1);
    }

    /// <summary>
    /// gets the tiles of brick, node and wall layer map of current visiting cell
    /// </summary>
    void setTileMapCells()
    {
        currentBrickTile = brickTileMap.GetTile<Tile>(cell);
        currentNodeTile = nodeTileMap.GetTile<Tile>(cell);
        currentWallTile = BGTileMap.GetTile<Tile>(cell);
    }

    /// <summary>
    /// checks if the first touch is on the any node tile and activates that knot as current node.
    /// </summary>
    void checkForNodeTouch(Vector3 point)
    {
        if (pressing)
            return;
       // cell = brickTileMap.WorldToCell(point);
        //Tile tile2 = nodeTileMap.GetTile<Tile>(cell);
        for (int i = 0; i < usedBaseKnots.Count; i++)
        {
            if (currentNodeTile == usedBaseKnots[i].getNodeTile())
            {
                currentNode = usedBaseKnots[i];
                currentNode.setFirstNodeData(cell);
                canDraw = true;
                pressing = true;
                cursor.SetActive(true);
                cursor.GetComponent<SpriteRenderer>().color = new Color(currentNode.getNodeTile().color.r, currentNode.getNodeTile().color.g, currentNode.getNodeTile().color.b, 0.5f);
            }
        }
    }
    /// <summary>
    /// checks if the first touch is on the any colored tile and activates that knot as current node.
    /// </summary>
    void checkForLineTouch()
    {
        Tile tile = brickTileMap.GetTile<Tile>(cell);
        baseKnots possibleNode = usedBaseKnots.FirstOrDefault(x => x.getBrickTile() == tile);
        if (possibleNode != null)
        {
            currentNode = possibleNode;
            canDraw = true;
            pressing = true;
            cursor.SetActive(true);
            cursor.GetComponent<SpriteRenderer>().color = new Color(currentNode.getNodeTile().color.r, currentNode.getNodeTile().color.g, currentNode.getNodeTile().color.b, 0.5f);
        }
    }

    /// <summary>
    /// checks swips of input is getting blocked by other nodes or by some conditions. also checks if all tiles should be removed from currentknot if the conditions are met.
    /// </summary>
    void checkSwipingNodes()
    {
        if (currentNodeTile != null && canDraw && (currentNodeTile != currentNode.getNodeTile() || (currentNodeTile == currentNode.getNodeTile() && currentNode.placedListCount() > 0)))
        {
            if (currentNodeTile == currentNode.getNodeTile() && currentNode.placedListCount() > 0)
            {
                currentNode.removeAllTile(cell);
            }
            canDraw = false;
        }
    }
    /// <summary>
    /// checks if current visition cell has empty places. if it does then based on some other conditions places the time on empty ones.
    /// also checks if places if not empty and has colored tile. then proceeds to remove.
    /// </summary>
    void checkAddOrRemove()
    {
        if (currentBrickTile == currentNode.getBrickTile())
        {
            canDraw = true;
            currentNode.removeTile(cell);
        }
        else if (currentBrickTile != currentNode.getBrickTile())
        {
            if (canDraw)
            {
                //checks if current position cell is far from last placed tile for some reason if not the procced to proccess current tile
                if (currentNode.placedListCount() == 0 || !isDistance(cell, currentNode.lastPlacedPostion()))
                {
                    currentNode.setTiles(cell, currentNodeTile);
                    if (currentNodeTile != currentNode.getNodeTile())
                    {
                        baseKnots othernode = usedBaseKnots.FirstOrDefault(x => x.getBrickTile() == currentBrickTile);
                        if (othernode != null)
                            currentNode.removeOtherTiles(othernode, cell);
                    }
                }
                else// if current cell is far from the last placed cell. checks all the in between tiles in same direction of input and proccess all those tiles as well as current one.
                {
                    setTilesAtDistance(currentBrickTile, currentNodeTile, cell);
                }
            }
        }
    }

    /// <summary>
    /// checks the distance between the previous placed tile and current visiting cell.
    /// </summary>
    bool isDistance(Vector3Int cell, Vector3Int prev)
    {
        if (prev.y == cell.y)
        {
            int diff = cell.x - prev.x;
            if (Mathf.Abs(diff) <= 1)
                return false;
        }
        else if (prev.x == cell.x)
        {
            int diff = cell.y - prev.y;
            if (Mathf.Abs(diff) <= 1)
                return false;
        }
        return true;
    }

    /// <summary>
    /// in case of distance cell. checks all the adjucent tiles between previous placed tile and current visiting cell. only checks in current direction. either on y axis or x axis.
    /// if conditions are met then procced to place tiles on visiting cell and all the cell between visiting cell and previous cell. 
    /// </summary>
    void setTilesAtDistance(Tile tile, Tile tile2, Vector3Int cell)
    {
        if (currentNode.placedListCount() == 0)
            return;
        if (tile == null)
        {
            Vector3Int lastbrick = currentNode.lastPlacedPostion();
            if (lastbrick.y == cell.y || lastbrick.x == cell.x)
            {
                bool horizontal = true;
                int diff;
                bool isPositive = false;
                if (lastbrick.y == cell.y)
                {
                    horizontal = true;
                    diff = cell.x - lastbrick.x;
                    if (cell.x > lastbrick.x)
                        isPositive = true;
                }
                else
                {
                    horizontal = false;
                    diff = cell.y - lastbrick.y;
                    if (cell.y > lastbrick.y)
                        isPositive = true;
                }
                List<nextNodeData> possible = new List<nextNodeData>();
                for (int i = 0; i < Mathf.Abs(diff); i++)
                {
                    Vector3Int posToCheck;
                    if (horizontal)
                    {
                        if (isPositive) posToCheck = new Vector3Int(lastbrick.x + (i + 1), lastbrick.y, 0);
                        else posToCheck = new Vector3Int(lastbrick.x - (i + 1), lastbrick.y, 0);
                    }
                    else
                    {
                        if (isPositive) posToCheck = new Vector3Int(lastbrick.x, lastbrick.y + (i + 1), 0);
                        else posToCheck = new Vector3Int(lastbrick.x, lastbrick.y - (i + 1), 0);
                    }

                    Tile bricktile = brickTileMap.GetTile<Tile>(posToCheck);
                    Tile nodetile = nodeTileMap.GetTile<Tile>(posToCheck);
                    Tile bgtile = BGTileMap.GetTile<Tile>(posToCheck);
                    if (bgtile != null && nodetile == null) possible.Add(new nextNodeData(nodetile, bricktile, posToCheck));
                    else return;
                }

                for (int i = 0; i < possible.Count; i++)
                {
                    currentNode.setTiles(possible[i].position, possible[i].nodeTile);
                    if (tile2 != currentNode.getNodeTile())
                    {
                        baseKnots othernode = usedBaseKnots.FirstOrDefault(x => x.getBrickTile() == possible[i].brickTile);
                        if (othernode != null)
                            currentNode.removeOtherTiles(othernode, possible[i].position);
                    }
                }
            }
        }
    }

    /// <summary>
    /// reset the values, list and all the layers of maps once the level is finished.
    /// </summary>
    void resetLevel()
    {
        for (int j = 0; j < row; j++)
        {
            for (int i = 0; i < col; i++)
            {
                Vector3Int cell = new Vector3Int(j - xStart, (col - i) - yStart, 0);
                nodeTileMap.SetTile(cell, null);
                brickTileMap.SetTile(cell, null);
                BGTileMap.SetTile(cell, null);
                lineTileMap.SetTile(cell, null);
            }
        }
        currentNode = null;
        pressing = false;
        canDraw = false;
        cursor.SetActive(false);
        usedBaseKnots.Clear();
        levelNods.Clear();
        UIInGameManager.Instance.levelCompletedTextStat(false);
    }
    /// <summary>
    /// On every knot formation checks for all the other knot as well to check if level is completed.
    /// </summary>
    void loadNextLevel()
    {
        resetLevel();
        gameManager.Instance.checkNextLevel();
        setLevel();
    }
    #endregion

    #region public methods
    /// <summary>
    /// On every knot formation checks for all the other knot as well to check if level is completed. and invokes loading of next level.
    /// </summary>
    public bool checkCompletion()
    {
        for (int i = 0; i < usedBaseKnots.Count; i++)
        {
            if (!usedBaseKnots[i].checkCompletion())
            {
                return false;
            }
        }
        Debug.Log("Level Completed");
        UIInGameManager.Instance.levelCompletedTextStat(true);
        gameState = GameState.COMPLETED;
        Invoke("loadNextLevel", 1);
        return true;
    }
    public void inputRecieved(inputManager.InputType inputType_, Vector3 pos_)
    {
        if (inputType_ == inputManager.InputType.NONE)
        {
            processStopTouch(pos_);
        }
        if (inputType_ == inputManager.InputType.SWIPING)
        {
            processTouch(pos_);
        }
    }
    #endregion


}
