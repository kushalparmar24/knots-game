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
    [SerializeField] public Tile halfLineTile, angleLineTile, fullLineTile, nodeLineTile;
    [SerializeField] public Tilemap brickTileMap, BGTileMap, nodeTileMap, lineTileMap;
    [SerializeField] colorNodeData colorNodeData;
    [SerializeField] Tile  BGTile;
    [SerializeField] GameObject cursor;
    List<baseKnots> usedBaseKnots = new List<baseKnots>();
    baseKnots currentNode;
    bool pressing, canDraw;
    int[,] level;
    int xStart;
    int yStart;
    Dictionary<int, List<Vector3Int>> levelNods;
    FileManager fileManager = new FileManager();
   
    Vector3Int currentPos, prevPos, cell;
    Tile currentBrickTile, currentNodeTile, currentWallTile;


    public List<baseKnots> getUsedBaseKnots()
    {
        return usedBaseKnots;
    }
    public static levelManager Instance { get { return instance; } }

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
    void setLevel()
    {
        gameState = GameState.LOADING;
        UIInGameManager.Instance.setLevelText();
        level = gameManager.Instance.getlevelData()[gameManager.Instance.currentLevel];
        levelNods = new Dictionary<int, List<Vector3Int>>();

        xStart = level.GetLength(1)/2;
        yStart = level.GetLength(0)/2;
        for (int j = 0; j < level.GetLength(1); j++)
        {
            for (int i = 0; i < level.GetLength(0); i++)
            {
                Vector3Int cell = new Vector3Int(j-xStart, (level.GetLength(0) - i)-yStart, 0);
                BGTileMap.SetTile(cell, BGTile);
                if (level[i, j] < 100)
                    continue;
                if (!levelNods.ContainsKey(level[i, j]))
                {
                    List<Vector3Int> temp = new List<Vector3Int>();
                    temp.Add(new Vector3Int(i, j, 0));
                    levelNods.Add(level[i, j], temp);

                    nodeTileMap.SetTile(cell, getNodetile(level[i, j]).getNodeTile());
                    createKnotFromID(level[i, j]);
                }
                else
                {
                   // levelNods.TryGetValue(level[i, j], out List<Vector3Int> temp);
                    //temp.Add(new Vector3Int(i, j, 0));
                    nodeTileMap.SetTile(cell, getNodetile(level[i, j]).getNodeTile());
                }
            }
        }
        gameState = GameState.PLAYING;
    }

    void createKnotFromID(int id_)
    {
        colorIDData colorIDDatas = colorNodeData.colorIDDatasList().FirstOrDefault(x => x.getID() == id_);
        baseKnots cbaseKnots = new baseKnots(colorIDDatas.getID(), colorIDDatas.getBrickTile(), colorIDDatas.getNodeTile());
        usedBaseKnots.Add(cbaseKnots);
    }

    colorIDData getNodetile(int id_)
    {
        return colorNodeData.colorIDDatasList().FirstOrDefault(x => x.getID() == id_);
    }


    public void inputRecieved(inputManager.InputType inputType_, Vector3 pos_)
    {
        if(inputType_ == inputManager.InputType.NONE)
        {
            processStopTouch(pos_);
        }
        if (inputType_ == inputManager.InputType.SWIPING)
        {
            processTouch(pos_);
        }
    }
    void processTouch(Vector3 pos_)
    {
        cell = brickTileMap.WorldToCell(pos_);
        currentPos = cell;
        cursor.transform.position = new Vector2(pos_.x, pos_.y); ;
        if (currentPos == prevPos)
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

    void setTileMapCells()
    {
        currentBrickTile = brickTileMap.GetTile<Tile>(cell);
        currentNodeTile = nodeTileMap.GetTile<Tile>(cell);
        currentWallTile = BGTileMap.GetTile<Tile>(cell);
    }

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
                else
                {
                    setTilesAtDistance(currentBrickTile, currentNodeTile, cell);
                }
            }
        }
    }

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

    void resetLevel()
    {
        for (int j = 0; j < level.GetLength(1); j++)
        {
            for (int i = 0; i < level.GetLength(0); i++)
            {
                Vector3Int cell = new Vector3Int(j - xStart, (level.GetLength(0) - i) - yStart, 0);
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

    void loadNextLevel()
    {
        resetLevel();
        gameManager.Instance.checkNextLevel();
        setLevel();
    }
}
