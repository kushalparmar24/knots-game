using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class gameManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile brickTile;
    bool end = false;

    Dictionary<int, Vector3Int> placedpos = new Dictionary<int, Vector3Int>();
    private void Update()
    {


        if (Input.GetMouseButton(0) && !end)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(point);
            Tile tile = tilemap.GetTile<Tile>(cell);
            if (tile == brickTile )
            {
                if (placedpos.ContainsValue(cell))
                {
                    var myKey = placedpos.FirstOrDefault(x => x.Value == cell).Key;
                    
                   
                    if (myKey != null && myKey < placedpos.Count-1)
                    {
                        end = true;
                        //for (int i = myKey; i < placedpos.Count - 1; i++)
                        //{
                        //    tilemap.SetTile(placedpos[i], null);
                        //    placedpos.Remove(i);
                        //}

                        while(placedpos.Count > myKey)
                        {
                            tilemap.SetTile(placedpos[placedpos.Count-1], null);
                            placedpos.Remove(placedpos.Count-1);
                        }
                        end = false;
                    }
                }
            }
            else if (tile != brickTile)
            {
               
                tilemap.SetTile(cell, brickTile);
                if (placedpos.Count == 0)
                    placedpos.Add(0, cell);
                else
                    placedpos.Add(placedpos.Count, cell);
               
            }
            
        }
    }
}
