using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class gameManager : MonoBehaviour
{
    public Tile strighttile,ankletile;
    public colorNodeData colorNodeData;
    public static gameManager instance;
    public Tilemap tilemap,bgtilemap,nodeGrid;
    public Tile brickTile,bgtile, bluetile, redtile;
    public bool end = false;
    public List<baseKnots> usedBaseKnots = new List<baseKnots>();


    baseKnots currentNode =  new baseKnots();
    public GameObject test;

    public enum InputType { NONE, SWIPING };

    bool pressing;
    bool canDraw;

    Vector3Int currentPos;
    Vector3Int prevPos;
    int[,] level;
    Dictionary<int, List<Vector3Int>> levelNods;

    private void Awake()
    {
        instance = this;
        FileManager fileManager = new FileManager();
        // level = new int[5,5];

        level = fileManager.ReadLevel();
        levelNods = new Dictionary<int, List<Vector3Int>>();
        for (int j = 0; j < level.GetLength(1); j++)
        {
            for (int i = 0; i < level.GetLength(0); i++)
            {
                Vector3Int cell = new Vector3Int(j, level.GetLength(0) -i, 0);
                bgtilemap.SetTile(cell, bgtile);
                if (level[i, j] < 100)
                    continue;
                if (!levelNods.ContainsKey(level[i, j]))
                {
                    List<Vector3Int> temp = new List<Vector3Int>();
                    //temp.Add(new Vector3Int(i, j, 0));
                    levelNods.Add(level[i, j], temp);
                    Vector3Int cell1 = new Vector3Int(j, level.GetLength(0) - i, 0);
                    bgtilemap.SetTile(cell1, getNodetile(level[i, j]));
                }
                else
                {
                    //levelNods.TryGetValue(level[i, j], out List<Vector3Int> temp);
                    //temp.Add(new Vector3Int(i, j, 0));
                    Vector3Int cell2 = new Vector3Int(j, level.GetLength(0) - i, 0);
                    bgtilemap.SetTile(cell2, getNodetile(level[i, j]));
                }
            }
        }

        




            currentNode.brickTile = brickTile;

        for (int i = 0; i < colorNodeData.colorIDDatas.Count; i++)
        {
            baseKnots cbaseKnots = new baseKnots();
            cbaseKnots.setData(colorNodeData.colorIDDatas[i].id, colorNodeData.colorIDDatas[i].bricktile, colorNodeData.colorIDDatas[i].nodeTile);
            usedBaseKnots.Add(cbaseKnots);
        }

        //testm();
    }

    Tile getNodetile(int id_)
    {
        return colorNodeData.colorIDDatas.FirstOrDefault(x => x.id == id_).nodeTile;
    }

    void testm()
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Vector3Int cell = new Vector3Int( i, j, 0);
                Tile tile = tilemap.GetTile<Tile>(cell);
                bgtilemap.SetTile(cell, bgtile);

                if (i == 0)
                {
                    bgtilemap.SetTile(cell, brickTile);
                    bgtilemap.SetTileFlags(cell, TileFlags.None);
                    bgtilemap.SetColor(cell, Color.white);
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                    bgtilemap.SetTransformMatrix(cell, matrix);
                    
                }
                //Instantiate(test, cell, Quaternion.identity);

            }
        }
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(point);
            currentPos = cell;
            Tile tile2 = nodeGrid.GetTile<Tile>(cell);
            for (int i = 0; i < colorNodeData.colorIDDatas.Count; i++)
            {
                if (tile2 == colorNodeData.colorIDDatas[i].nodeTile)
                {
                    currentNode = usedBaseKnots[i];
                    currentNode.setFirstNodeData(cell);
                    canDraw = true;
                    pressing = true;
                }
            }
            if(!pressing)
            {
                Tile tile = tilemap.GetTile<Tile>(cell);
               baseKnots possibleNode = usedBaseKnots.FirstOrDefault(x => x.brickTile == tile);
                if (possibleNode != null)
                    currentNode = possibleNode;
                pressing = true;
                canDraw = true;
            }
            prevPos = currentPos;
           
        } 
        if (Input.GetMouseButtonUp(0))
        {
            canDraw = false;
            // InputType.NONE
            pressing = false;
           // currentNode.setAsFirstNode(false);
            for (int i = 0; i < usedBaseKnots.Count; i++)
            {
                usedBaseKnots[i].resetdict();
            }
            prevPos = new Vector3Int(-10000, -100000, -1);
        }
        
        if (pressing )//&& !end)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(point);
            currentPos = cell;
            Tile tile = tilemap.GetTile<Tile>(cell);
            Tile tile2 = nodeGrid.GetTile<Tile>(cell);
            Tile tile3 = bgtilemap.GetTile<Tile>(cell);
            if (prevPos == currentPos)
                return;

            if (tile3 == null)
            {
                // canDraw = false;
                return;
            }
            else
            {


                if (tile2 != null && canDraw && (tile2 != currentNode.nodeTile || (tile2 == currentNode.nodeTile && currentNode.placedList.Count > 0)))
                {
                    if (tile2 == currentNode.nodeTile && currentNode.placedList.Count > 0)
                    {
                        currentNode.removeAllTile(cell);
                    }
                    canDraw = false;
                   // if (tile2 == currentNode.nodeTile && tile == null)
                     //   canDraw = true;
                   // return;
                }

                //if(!canDraw && tile == null )
                //{
                //    Vector3Int lastbrick = currentNode.placedList[currentNode.placedList.Count - 1].position;

                //    if (lastbrick.y == cell.y)
                //    {
                //        List<Vector3Int> possible = new List<Vector3Int>();
                //        int diff = cell.x - lastbrick.x;
                //        bool isPositive = false;
                //        if (cell.x > lastbrick.x)
                //            isPositive = true;
                //        for (int i = 0; i < Mathf.Abs(diff); i++)
                //        {
                //            Vector3Int posToCheck;
                //            if (isPositive)
                //            {
                //                posToCheck = new Vector3Int(lastbrick.x+ (i+1), lastbrick.y, 0);
                //            }
                //            else
                //            {
                //                posToCheck = new Vector3Int(lastbrick.x - (i + 1), lastbrick.y, 0);
                //            }
                            
                //            Tile posibletile = tilemap.GetTile<Tile>(posToCheck);
                //            Tile posibletile2 = nodeGrid.GetTile<Tile>(posToCheck);
                //            Tile posibletile3 = bgtilemap.GetTile<Tile>(posToCheck);
                //            if (posibletile3 != null && posibletile2 == null && posibletile == null)
                //            {
                //                possible.Add(posToCheck);
                //                canDraw = true;
                //                Debug.Log("fas gaya");
                //            }
                //            else
                //            {
                //                canDraw = false;
                //                break;
                //            }
                //        }
                //        if(canDraw)
                //        {
                //            for (int i = 0; i < possible.Count; i++)
                //            {
                //               // currentNode.setTiles(possible[i], tile2);
                //            }
                //        }
                //    }
                //}

                if (tile == currentNode.brickTile)
                {
                    canDraw = true;
                    currentNode.removeTile(cell);
                }
                else if (tile != currentNode.brickTile )
                {
                    if (canDraw)
                    {
                        
                        if (currentNode.placedList.Count ==0 || !isDiff(cell, currentNode.placedList[currentNode.placedList.Count - 1].position))
                        {
                            Debug.Log("settile");
                            currentNode.setTiles(cell, tile2);
                            if (tile2 != currentNode.nodeTile)
                            {
                                baseKnots othernode = usedBaseKnots.FirstOrDefault(x => x.brickTile == tile);
                                if (othernode != null)
                                    currentNode.removeOtherTiles(othernode, cell);
                            }
                        }
                        else
                        {
                            Debug.Log("testmethod");
                            testmethod(tile, tile2, cell);
                        }
                        
                    }
                }
            }
            Debug.Log("pressed");
            prevPos = currentPos;
            canDraw = true;
        }
    }

    bool isDiff(Vector3Int cell , Vector3Int prev)
    {
        if (prev.y == cell.y)
        {
           int diff = cell.x - prev.x;
             if (Mathf.Abs(diff) <= 1)
               return false;
        }
        else if(prev.x == cell.x)
        {
           int diff = cell.y - prev.y;
            if (Mathf.Abs(diff) <= 1)
                return false;
        }
        return true;
    }

    void testmethod(Tile tile, Tile tile2, Vector3Int cell)
    {
        if (currentNode.placedList.Count == 0)
            return;

        Debug.Log("testmethod01");
        if (tile == null)
        {
            Vector3Int lastbrick = currentNode.placedList[currentNode.placedList.Count - 1].position;
            Debug.Log("testmethod02");
            if (lastbrick.y == cell.y || lastbrick.x == cell.x)
            {
                bool horizontal = true;
                int diff;
                bool isPositive = false;
                if (lastbrick.y == cell.y)
                {
                    horizontal = true;
                    diff = cell.x - lastbrick.x;
                   // if (Mathf.Abs(diff) <= 1)
                     //   return;
                    if (cell.x > lastbrick.x)
                        isPositive = true;
                }
                else
                {
                    horizontal = false;
                    diff = cell.y - lastbrick.y;
                   // if (Mathf.Abs(diff) <= 1)
                     //   return;

                    if (cell.y > lastbrick.y)
                        isPositive = true;
                }
                Debug.Log("testmethod03");
                List<nextNodeData> possible = new List<nextNodeData>();
                Debug.Log("testmethod04");
                for (int i = 0; i < Mathf.Abs(diff); i++)
                {
                    Vector3Int posToCheck;
                    if (horizontal)
                    {
                        if (isPositive)
                        {
                            posToCheck = new Vector3Int(lastbrick.x + (i + 1), lastbrick.y, 0);
                        }
                        else
                        {
                            posToCheck = new Vector3Int(lastbrick.x - (i + 1), lastbrick.y, 0);
                        }
                    }
                    else
                    {
                        if (isPositive)
                        {
                            posToCheck = new Vector3Int(lastbrick.x, lastbrick.y + (i + 1), 0);
                        }
                        else
                        {
                            posToCheck = new Vector3Int(lastbrick.x, lastbrick.y - (i + 1), 0);
                        }
                    }


                    Tile posibletile = tilemap.GetTile<Tile>(posToCheck);
                    Tile posibletile2 = nodeGrid.GetTile<Tile>(posToCheck);
                    Tile posibletile3 = bgtilemap.GetTile<Tile>(posToCheck);
                    if (posibletile3 != null && posibletile2 == null)// && posibletile == null)
                    {
                        possible.Add(new nextNodeData(posibletile2,posibletile,posToCheck));
                        //canDraw = true;
                    }
                    else
                    {
                        // canDraw = false;
                        return;
                        // break;
                    }
                }

                for (int i = 0; i < possible.Count; i++)
                {
                    Debug.Log("testmettile");
                    currentNode.setTiles(possible[i].position, possible[i].nodeTile);
                    if (tile2 != currentNode.nodeTile)
                    {
                        baseKnots othernode = usedBaseKnots.FirstOrDefault(x => x.brickTile == possible[i].brickTile);
                        if (othernode != null)
                            currentNode.removeOtherTiles(othernode, possible[i].position);
                    }
                }

            }
        }
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
        return true;
    }

    public int checkDirection(Vector3Int current_, Vector3Int prev_)
    {
        int Rotation = 0;
        if (current_.x > prev_.x)
        {
            Rotation = 90;
            //right
        }
        else if(current_.x < prev_.x)
        {
            Rotation = -90;
            //left
        }
        else if (current_.y > prev_.y)
        {
            Rotation = 180;
            //up
        }
        else if (current_.x < prev_.x)
        {
            Rotation = 0;
            //down
        }
        return Rotation;
       // return UnityEngine.Matrix4x4;
    }
}
