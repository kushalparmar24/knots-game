using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Linq;

[Serializable]
public class baseKnots 
{
     [SerializeField] int id;

    [SerializeField] public Tile brickTile;
    public List<nodeData> placedList = new List<nodeData>();
    List<nodeData> tobeRemoved = new List<nodeData>();

    Dictionary<int, List<nodeData>> removedNodes= new Dictionary<int, List<nodeData>>();

    public void resetdict()
    {
        removedNodes.Clear();
    }
    public void setData(int id_, Tile brickTile_)
    {
        id = id_;
        brickTile = brickTile_;
    }
    public void setTiles(Vector3Int cell_)
    {
        Debug.Log("setTile" + id);
        if (placedList.Count >0 && cell_ == placedList[placedList.Count - 1].position)
            return;
        Debug.Log("setTile"+id);
        gameManager.instance.tilemap.SetTile(cell_, brickTile);
        if (placedList.Count == 0)
            placedList.Add(new nodeData(0, cell_,id));
        else
            placedList.Add(new nodeData(placedList.Count, cell_, id));
    }

    public void removeOtherTiles(baseKnots baseKnots_,Vector3Int cell_)
    {
        baseKnots_.tempRemoveTile(cell_, out List<nodeData> nodeDatas_);
        if(nodeDatas_ != null)
        removedNodes.Add(removedNodes.Count, nodeDatas_);
        //removedNodes.Add(removedNodes.Count, )
    }

    public void removeTile(Vector3Int cell_)
    {
        if (placedList.Count >0 && cell_ == placedList[placedList.Count - 1].position)
            return;
        var myKey = placedList.FirstOrDefault(x => x.position == cell_);
        if (myKey != null && myKey.index < placedList.Count - 1)
        {
            gameManager.instance.end = true;
          
            while (placedList.Count > myKey.index)
            {
                Debug.Log("removeTile"+id);
                
                gameManager.instance.tilemap.SetTile(placedList[placedList.Count - 1].position, null);
                placedList.RemoveAt(placedList.Count - 1);
                replaceRemovedTiles();
            }
            gameManager.instance.end = false;
        }
    }
    public void tempRemoveTile(Vector3Int cell_, out List<nodeData> nodeDatas_)
    {
        nodeDatas_ = null;
        var myKey = placedList.FirstOrDefault(x => x.position == cell_);
        if (myKey != null && myKey.index <= placedList.Count - 1)
        {
            while (placedList.Count > myKey.index)
            {
                Debug.Log("tempRemoveTile");
                if (myKey.index != placedList.Count-1)
                    gameManager.instance.tilemap.SetTile(placedList[placedList.Count - 1].position, null);
                if (nodeDatas_ == null)
                    nodeDatas_ = new List<nodeData>();
                nodeDatas_.Add(placedList[placedList.Count - 1]);
                placedList.RemoveAt(placedList.Count - 1);
            }
        }
    }

    public void replaceRemovedTiles()
    {
        if (removedNodes == null || removedNodes.Count <= 0)
            return;
        removedNodes.TryGetValue(removedNodes.Count - 1, out List < nodeData >dataToReplace);
        if (dataToReplace == null)
            return;
        for (int i = 0; i < dataToReplace.Count; i++)
        {
            var thisBaseKnots = gameManager.instance.usedBaseKnots.FirstOrDefault(x => x.id == dataToReplace[i].ID);
            thisBaseKnots.setTiles(dataToReplace[(dataToReplace.Count-1)-i].position);
            //gameManager.instance.tilemap.SetTile(placedList[placedList.Count - 1].position, thisBaseKnots.brickTile);
        }
        removedNodes.Remove(removedNodes.Count-1);
    }
}

[Serializable]
public class nodeData
{
    public int index;
    public Vector3Int position;
    public int ID;
   

    public nodeData(int index_, Vector3Int position_,int id_)
    {
        index = index_;
        position = position_;
        ID = id_;
    }
}


