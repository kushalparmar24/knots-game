using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class gameManager : MonoBehaviour
{
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

    private void Awake()
    {
        instance = this;

        currentNode.brickTile = brickTile;

        for (int i = 0; i < colorNodeData.colorIDDatas.Count; i++)
        {
            baseKnots cbaseKnots = new baseKnots();
            cbaseKnots.setData(colorNodeData.colorIDDatas[i].id, colorNodeData.colorIDDatas[i].nodeTile);
            usedBaseKnots.Add(cbaseKnots);
        }
        
        //testm();
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
                    bgtilemap.SetTileFlags(cell, TileFlags.None);
                    bgtilemap.SetColor(cell, Color.white);
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
            Tile tile2 = nodeGrid.GetTile<Tile>(cell);
            for (int i = 0; i < colorNodeData.colorIDDatas.Count; i++)
            {
                if (tile2 == colorNodeData.colorIDDatas[i].nodeTile)
                {
                    currentNode = usedBaseKnots[i];
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
            }
           
        } 
        if (Input.GetMouseButtonUp(0))
        {
            canDraw = false;
            // InputType.NONE
            pressing = false;
            for (int i = 0; i < usedBaseKnots.Count; i++)
            {
                usedBaseKnots[i].resetdict();
            }
        }
        
        if (pressing && !end)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(point);
            Tile tile = tilemap.GetTile<Tile>(cell);
            Tile tile2 = nodeGrid.GetTile<Tile>(cell);

            if (tile2 != null && canDraw && ( tile2 != currentNode.brickTile || (tile2 == currentNode.brickTile && currentNode.placedList.Count > 1)))
            {
                canDraw = false;
                return;
            }

            if (tile == currentNode.brickTile)
            {
                canDraw = true;
                currentNode.removeTile(cell);
            }
            else if (tile != currentNode.brickTile)
            {
                if (canDraw)
                {
                    currentNode.setTiles(cell);
                    baseKnots othernode = usedBaseKnots.FirstOrDefault(x => x.brickTile == tile);
                    if (othernode != null)
                        currentNode.removeOtherTiles(othernode, cell);
                }

            }
        }
    }
}
