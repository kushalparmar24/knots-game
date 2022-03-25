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
   

    Dictionary<int, List<nodeData>> removedNodes= new Dictionary<int, List<nodeData>>();
    bool firstNode;
    Vector3Int firstNodePos;
    Vector3Int lastNodePos;

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
        if ((NodeTile_ != null && NodeTile_ == nodeTile  && cell_ == firstNodePos) || completed)
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

        addline();
        if (placedList.Count == 1)
        {
            gameManager.instance.tilemap.SetTile(firstNodePos, brickTile);
        }
    }

    void addline()
    {
        if (placedList.Count > 1)
        {
            Vector3Int currentCell = placedList[placedList.Count - 1].position;
            Vector3Int prevCell = placedList[placedList.Count - 2].position;
            lineData lineData = gameManager.instance.checkDirection(currentCell, prevCell, placedList[placedList.Count - 2].rotation);
            gameManager.instance.lineTileMap.SetTile(currentCell, lineData.currentTile);
            gameManager.instance.lineTileMap.SetTileFlags(currentCell, TileFlags.None);
            gameManager.instance.lineTileMap.SetColor(currentCell, nodeTile.color);
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, lineData.currentRot), Vector3.one);
            gameManager.instance.lineTileMap.SetTransformMatrix(currentCell, matrix);
            placedList[placedList.Count - 1].rotation = lineData.currentRot;

            if (placedList[placedList.Count - 2].rotation != -1000)
            {
                gameManager.instance.lineTileMap.SetTile(prevCell, lineData.prevTile);
                gameManager.instance.lineTileMap.SetTileFlags(prevCell, TileFlags.None);
                gameManager.instance.lineTileMap.SetColor(prevCell, nodeTile.color);
                Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, lineData.prevRot), Vector3.one);
                gameManager.instance.lineTileMap.SetTransformMatrix(prevCell, matrix2);
                //placedList[placedList.Count - 2].rotation = lineData.prevRot;
            }
        }
        else
        {
            Vector3Int currentCell = placedList[placedList.Count - 1].position;
            lineData lineData = gameManager.instance.checkDirection(currentCell, firstNodePos, -1000);
            gameManager.instance.lineTileMap.SetTile(currentCell, lineData.currentTile);
            gameManager.instance.lineTileMap.SetTileFlags(currentCell, TileFlags.None);
            gameManager.instance.lineTileMap.SetColor(currentCell, nodeTile.color);
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, lineData.currentRot), Vector3.one);
            gameManager.instance.lineTileMap.SetTransformMatrix(currentCell, matrix);
            placedList[placedList.Count - 1].rotation = lineData.currentRot;

            gameManager.instance.lineTileMap.SetTile(firstNodePos, gameManager.instance.nodeline);
            gameManager.instance.lineTileMap.SetTileFlags(firstNodePos, TileFlags.None);
            gameManager.instance.lineTileMap.SetColor(firstNodePos, nodeTile.color);
            Matrix4x4 matrix1 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, lineData.currentRot), Vector3.one);
            gameManager.instance.lineTileMap.SetTransformMatrix(firstNodePos, matrix1);
        }

    }
    void removeline()
    {
        if (placedList.Count == 0)
            return;


        if (placedList.Count > 1)
        {
            Vector3Int currentCell = placedList[placedList.Count - 1].position;
            Vector3Int prevCell = placedList[placedList.Count - 2].position;
            lineData lineData = gameManager.instance.checkDirection(currentCell, prevCell, placedList[placedList.Count - 2].rotation);
            gameManager.instance.lineTileMap.SetTile(currentCell, lineData.currentTile);
            gameManager.instance.lineTileMap.SetTileFlags(currentCell, TileFlags.None);
            gameManager.instance.lineTileMap.SetColor(currentCell, nodeTile.color);
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, lineData.currentRot), Vector3.one);
            gameManager.instance.lineTileMap.SetTransformMatrix(currentCell, matrix);
           // placedList[placedList.Count - 1].rotation = lineData.currentRot;

            //if (placedList[placedList.Count - 2].rotation != -1000)
            //{
            //    gameManager.instance.lineTileMap.SetTile(prevCell, lineData.prevTile);
            //    gameManager.instance.lineTileMap.SetTileFlags(prevCell, TileFlags.None);
            //    gameManager.instance.lineTileMap.SetColor(prevCell, nodeTile.color);
            //    Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, lineData.prevRot), Vector3.one);
            //    gameManager.instance.lineTileMap.SetTransformMatrix(prevCell, matrix2);
            //    placedList[placedList.Count - 2].rotation = lineData.currentRot;
            //}
        }
        else
        {
            Vector3Int currentCell = placedList[placedList.Count - 1].position;
            lineData lineData = gameManager.instance.checkDirection(currentCell, firstNodePos, -1000);
            gameManager.instance.lineTileMap.SetTile(currentCell, lineData.currentTile);
            gameManager.instance.lineTileMap.SetTileFlags(currentCell, TileFlags.None);
            gameManager.instance.lineTileMap.SetColor(currentCell, nodeTile.color);
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, lineData.currentRot), Vector3.one);
            gameManager.instance.lineTileMap.SetTransformMatrix(currentCell, matrix);
            //placedList[placedList.Count - 1].rotation = lineData.currentRot;
        }

    }

    void removefirstnodeline()
    {
        gameManager.instance.lineTileMap.SetTile(firstNodePos, null);
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
        if (placedList.Count > 0 && cell_ == placedList[placedList.Count - 1].position)
        {
            
            return;
        }
        var myKey = placedList.FirstOrDefault(x => x.position == cell_);
        if (myKey != null && myKey.index < placedList.Count - 1)
        {
            //gameManager.instance.end = true;

            while (placedList.Count > myKey.index + 1)
            {
                Debug.Log("removeTile" + id);

                gameManager.instance.tilemap.SetTile(placedList[placedList.Count - 1].position, null);
                gameManager.instance.lineTileMap.SetTile(placedList[placedList.Count - 1].position, null);
                replaceRemovedTiles(placedList.Count - 1);
                placedList.RemoveAt(placedList.Count - 1);
            }
            // gameManager.instance.end = false;
            if (placedList.Count < 1)
                gameManager.instance.tilemap.SetTile(firstNodePos, null);
            completed = false;
            lastNodePos = Vector3Int.one;
            completionCount = 0;
            removeline();
        }
        
    }
    public void removeAllTile(Vector3Int currentPos, bool forceRemove = false)
    {
        if (!isSameNode(currentPos) && !forceRemove)
        {
            //lastNodePos = currentPos;
            //gameManager.instance.tilemap.SetTile(lastNodePos, brickTile);
            //setLastNodeLine();
            setTiles(currentPos, nodeTile);
            setCompletion();
            gameManager.instance.checkCompletion();
            return;
        }
        Debug.Log("removingall");
        for(int i = 0; i<placedList.Count;i++)
        {
           
            gameManager.instance.tilemap.SetTile(placedList[i].position, null);
            gameManager.instance.lineTileMap.SetTile(placedList[i].position, null);
            replaceRemovedTiles((placedList.Count-1)-i);
            completed = false;
            lastNodePos = Vector3Int.one;
            completionCount = 0;

        }
        gameManager.instance.tilemap.SetTile(firstNodePos, null);
        gameManager.instance.lineTileMap.SetTile(firstNodePos, null);

        placedList.Clear();
    }

    public void clearTileOnCountOne()
    {
        if (placedList.Count == 1)
        {
            for (int i = 0; i < placedList.Count; i++)
            {
                gameManager.instance.tilemap.SetTile(placedList[i].position, null);
                gameManager.instance.lineTileMap.SetTile(placedList[i].position, null);

            }

            placedList.Clear();
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
                if (myKey.index != placedList.Count - 1)
                {
                    gameManager.instance.tilemap.SetTile(placedList[placedList.Count - 1].position, null);
                    gameManager.instance.lineTileMap.SetTile(placedList[placedList.Count - 1].position, null);
                }
                if (nodeDatas_ == null)
                    nodeDatas_ = new List<nodeData>();
                nodeDatas_.Add(placedList[placedList.Count - 1]);
                placedList.RemoveAt(placedList.Count - 1);
            }
            if (placedList.Count < 1)
            {
                gameManager.instance.tilemap.SetTile(firstNodePos, null);
                gameManager.instance.lineTileMap.SetTile(firstNodePos, null);
            }
            removeline();
            completed = false;
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
            if(thisBaseKnots.completionCount == thisBaseKnots.placedList.Count)
            {
                thisBaseKnots.completed = true;
            }
        }
       
        removedNodes.Remove(index);
    }
    public void setFirstNodeData(Vector3Int firstNodePos_)
    {
        if (isSameNode(firstNodePos_))
        {
            removeAllTile(firstNodePos_, true);
           // setTiles(firstNodePos_, nodeTile);
        }
        else
        {
            gameManager.instance.tilemap.SetTile(firstNodePos, null);
            firstNodePos = firstNodePos_;
            setAsFirstNode(true);
            removeAllTile(firstNodePos_, true);
            //setTiles(firstNodePos_, nodeTile);
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
    public int rotation;
   

    public nodeData(int index_, Vector3Int position_,int id_)
    {
        index = index_;
        position = position_;
        ID = id_;
        rotation = -1000;
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

[Serializable]
public class lineData
{
    public Tile prevTile, currentTile;
    public int prevRot,currentRot;

    public lineData(Tile prevTile_, Tile currentTile_, int prevRot_, int currentRot_)
    {
        prevTile = prevTile_;
        currentTile = currentTile_;
        prevRot = prevRot_;
        currentRot = currentRot_;
    }
}


