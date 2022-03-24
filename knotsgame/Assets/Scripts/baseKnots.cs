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
    [SerializeField] public Tile nodeTile;
    [SerializeField] public Tile brickTile;
    public List<nodeData> placedList = new List<nodeData>();
    List<nodeData> tobeRemoved = new List<nodeData>();

    Dictionary<int, List<nodeData>> removedNodes= new Dictionary<int, List<nodeData>>();
    bool firstNode;
    Vector3Int firstNodePos;

    public bool completed;
    int completionCount;

    public void resetdict()
    {
        removedNodes.Clear();
    }
    public void setData(int id_, Tile brickTile_, Tile nodeTile_)
    {
        id = id_;
        brickTile = brickTile_;
        nodeTile = nodeTile_;
    }
    public void setTiles(Vector3Int cell_, Tile NodeTile_)
    {
        if (NodeTile_!=null && NodeTile_ == nodeTile)
        {
            return;
        }

        if (placedList.Count >0 && cell_ == placedList[placedList.Count - 1].position)
            return;
        
        if (placedList.Count == 0)
            placedList.Add(new nodeData(0, cell_,id));
        else
            placedList.Add(new nodeData(placedList.Count, cell_, id));

        gameManager.instance.tilemap.SetTile(cell_, brickTile);
        //gameManager.instance.tilemap.SetTileFlags(cell_, TileFlags.None);
        //gameManager.instance.tilemap.SetColor(cell_, brickTile.color);
        //Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, gameManager.instance.checkDirection(cell_, placedList[placedList.Count - 2].position)), Vector3.one);
        //gameManager.instance.tilemap.SetTransformMatrix(cell_, matrix);
    }

    public void removeOtherTiles(baseKnots baseKnots_,Vector3Int cell_)
    {
        baseKnots_.tempRemoveTile(cell_, out List<nodeData> nodeDatas_);
        if(nodeDatas_ != null)
        removedNodes.Add(placedList.Count-1, nodeDatas_);
        //removedNodes.Add(removedNodes.Count, )
    }

    public void removeTile(Vector3Int cell_)
    {
        if (placedList.Count >0 && cell_ == placedList[placedList.Count - 1].position)
            return;
        var myKey = placedList.FirstOrDefault(x => x.position == cell_);
        if (myKey != null && myKey.index < placedList.Count - 1)
        {
            //gameManager.instance.end = true;
          
            while (placedList.Count > myKey.index+1)
            {
                Debug.Log("removeTile"+id);
                
                gameManager.instance.tilemap.SetTile(placedList[placedList.Count - 1].position, null);
                replaceRemovedTiles(placedList.Count - 1);
                placedList.RemoveAt(placedList.Count - 1);
                ;
            }
           // gameManager.instance.end = false;
        }
    }
    public void removeAllTile(Vector3Int currentPos, bool forceRemove = false)
    {
        if (!isSameNode(currentPos) && !forceRemove)
        {
            setCompletion();
            gameManager.instance.checkCompletion();
            return;
        }
        Debug.Log("removingall");
        for(int i = 0; i<placedList.Count;i++)
        {
            gameManager.instance.tilemap.SetTile(placedList[i].position, null);
           
        }
        placedList.Clear();
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

    public void replaceRemovedTiles(int index)
    {
        if (removedNodes == null || removedNodes.Count <= 0)
            return;
       //removedNodes.TryGetValue(removedNodes.Count - 1, out List < nodeData >dataToReplace);
        removedNodes.TryGetValue(index, out List<nodeData> dataToReplace);
        if (dataToReplace == null)
            return;
        for (int i = 0; i < dataToReplace.Count; i++)
        {
            var thisBaseKnots = gameManager.instance.usedBaseKnots.FirstOrDefault(x => x.id == dataToReplace[i].ID);
            thisBaseKnots.setTiles(dataToReplace[(dataToReplace.Count-1)-i].position,null);
            //gameManager.instance.tilemap.SetTile(placedList[placedList.Count - 1].position, thisBaseKnots.brickTile);
        }
        removedNodes.Remove(index);
    }
    public void setFirstNodeData(Vector3Int firstNodePos_)
    {
        if (isSameNode(firstNodePos_))
        {
            removeAllTile(firstNodePos_, true);
        }
        else
        {
            firstNodePos = firstNodePos_;
            setAsFirstNode(true);
            removeAllTile(firstNodePos_, true);
        }
    }

    void setAsFirstNode(bool state)
    {
            firstNode = state;
    }

    bool isSameNode(Vector3Int firstNodePos_)
    {
        if (firstNode && firstNodePos == firstNodePos_)
        {
            return true;
        }
        return false;
    }

    void setCompletion()
    {
        if (placedList.Count == 0)
            return;
        completionCount = placedList.Count;
        completed = true;
    }

    public bool checkCompletion()
    {
        if (placedList.Count == 0)
            return false;
        if (completed)
        {
            if(placedList.Count == completionCount)
            {
                return true;
            }
        }
        return false;
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

[Serializable]
public class nextNodeData
{
    public Tile nodeTile, brickTile;
    public Vector3Int position;

    public nextNodeData(Tile nodeTile_, Tile brickTile_ ,Vector3Int position_)
    {
        nodeTile = nodeTile_;
        brickTile = brickTile_;
        position = position_;
    }
}


