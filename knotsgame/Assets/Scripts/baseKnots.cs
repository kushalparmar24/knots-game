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
    [SerializeField] Tile nodeTile;
    [SerializeField] Tile brickTile;
    List<nodeData> placedList = new List<nodeData>();
    Dictionary<int, List<nodeData>> removedNodes= new Dictionary<int, List<nodeData>>();
    bool firstNode;
    Vector3Int firstNodePos;
    public bool completed;
    int completionCount;

    public baseKnots(int id_, Tile brickTile_, Tile nodeTile_)
    {
        id = id_;
        brickTile = brickTile_;
        nodeTile = nodeTile_;
    }
    public int placedListCount()
    {
        return placedList.Count;
    }
    public Vector3Int lastPlacedPostion()
    {
        return placedList[placedList.Count-1].position;
    }
    public Tile getNodeTile()
    {
        return nodeTile;
    }
    public Tile getBrickTile()
    {
        return brickTile;
    }

    public void resetdict()
    {
        removedNodes.Clear();
    }
   
    public void setTiles(Vector3Int cell_, Tile NodeTile_)
    {
        if ((NodeTile_ != null && NodeTile_ == nodeTile  && cell_ == firstNodePos) || completed || (placedList.Count > 0 && cell_ == placedList[placedList.Count - 1].position))
        {
            return;
        }
        
        if (placedList.Count == 0)
            placedList.Add(new nodeData(0, cell_,id));
        else
            placedList.Add(new nodeData(placedList.Count, cell_, id));

        levelManager.Instance.brickTileMap.SetTile(cell_, brickTile);
        addline();
        if (placedList.Count == 1)
            levelManager.Instance.brickTileMap.SetTile(firstNodePos, brickTile);
    }

    void addline()
    {
        if (placedList.Count > 1)
        {
            Vector3Int currentCell = placedList[placedList.Count - 1].position;
            Vector3Int prevCell = placedList[placedList.Count - 2].position;
            lineData lineData = checkDirection(currentCell, prevCell, placedList[placedList.Count - 2].rotation);
            genericLinePlacer(currentCell, nodeTile.color, lineData.currentRot, lineData.currentTile);
            placedList[placedList.Count - 1].rotation = lineData.currentRot;

            if (placedList[placedList.Count - 2].rotation != -1000)
            {
                genericLinePlacer(prevCell, nodeTile.color, lineData.prevRot, lineData.prevTile);
            }
        }
        else
        {
            Vector3Int currentCell = placedList[placedList.Count - 1].position;
            lineData lineData = checkDirection(currentCell, firstNodePos, -1000);
            genericLinePlacer(currentCell, nodeTile.color, lineData.currentRot, lineData.currentTile);
            placedList[placedList.Count - 1].rotation = lineData.currentRot;

            genericLinePlacer(firstNodePos, nodeTile.color, lineData.currentRot, levelManager.Instance.nodeLineTile);
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
            lineData lineData = checkDirection(currentCell, prevCell, placedList[placedList.Count - 2].rotation);
            genericLinePlacer(currentCell, nodeTile.color, lineData.currentRot, lineData.currentTile);
        }
        else
        {
            Vector3Int currentCell = placedList[placedList.Count - 1].position;
            lineData lineData = checkDirection(currentCell, firstNodePos, -1000);
            genericLinePlacer(currentCell, nodeTile.color, lineData.currentRot, lineData.currentTile);
        }

    }

    void genericLinePlacer(Vector3Int pos_,Color color_,int rot_, Tile tile_)
    {
        levelManager.Instance.lineTileMap.SetTile(pos_, tile_);
        levelManager.Instance.lineTileMap.SetTileFlags(pos_, TileFlags.None);
        levelManager.Instance.lineTileMap.SetColor(pos_, nodeTile.color);
        Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f,rot_), Vector3.one);
        levelManager.Instance.lineTileMap.SetTransformMatrix(pos_, matrix2);
    }

    public void removeOtherTiles(baseKnots baseKnots_,Vector3Int cell_)
    {
        baseKnots_.tempRemoveTile(cell_, out List<nodeData> nodeDatas_);
        if(nodeDatas_ != null)
        removedNodes.Add(placedList.Count-1, nodeDatas_);
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
            while (placedList.Count > myKey.index + 1)
            {
                Debug.Log("removeTile" + id);

                setGroupTile(placedList[placedList.Count - 1].position, null);
                replaceRemovedTilesOnOtherNodes(placedList.Count - 1);
                placedList.RemoveAt(placedList.Count - 1);
            }
            if (placedList.Count < 1)
                levelManager.Instance.brickTileMap.SetTile(firstNodePos, null);
            completed = false;
            completionCount = 0;
            removeline();
        }
        
    }
    public void removeAllTile(Vector3Int currentPos, bool forceRemove = false)
    {
        if (!isSameNode(currentPos) && !forceRemove)
        {
            setTiles(currentPos, nodeTile);
            setCompletion();
            levelManager.Instance.checkCompletion();
            return;
        }
        for(int i = 0; i<placedList.Count;i++)
        {
            setGroupTile(placedList[i].position, null);
            replaceRemovedTilesOnOtherNodes((placedList.Count-1)-i);
            completed = false;
            completionCount = 0;
        }
        setGroupTile(firstNodePos, null);
        placedList.Clear();
    }

    void tempRemoveTile(Vector3Int cell_, out List<nodeData> nodeDatas_)
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
                    setGroupTile(placedList[placedList.Count - 1].position, null);
                }
                if (nodeDatas_ == null)
                    nodeDatas_ = new List<nodeData>();
                nodeDatas_.Add(placedList[placedList.Count - 1]);
                placedList.RemoveAt(placedList.Count - 1);
            }
            if (placedList.Count < 1)
            {
                setGroupTile(firstNodePos, null);
            }
            removeline();
            completed = false;
        }
    }

    void replaceRemovedTilesOnOtherNodes(int index)
    {
        if (removedNodes == null || removedNodes.Count <= 0)
            return;
        removedNodes.TryGetValue(index, out List<nodeData> dataToReplace);
        if (dataToReplace == null)
            return;
        for (int i = 0; i < dataToReplace.Count; i++)
        {
            var otherBaseKnots = levelManager.Instance.getUsedBaseKnots().FirstOrDefault(x => x.id == dataToReplace[i].ID);
            otherBaseKnots.setTiles(dataToReplace[(dataToReplace.Count-1)-i].position,null);
            if(otherBaseKnots.completionCount == otherBaseKnots.placedList.Count)
            {
                otherBaseKnots.completed = true;
            }
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
            setGroupTile(firstNodePos, null);
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

    lineData checkDirection(Vector3Int current_, Vector3Int prev_, int prevRot_)
    {
        lineData lineData = new lineData(null, null, prevRot_, 0);
        if (current_.x > prev_.x) lineData.currentRot = 90; //right
        else if (current_.x < prev_.x) lineData.currentRot = -90;//left
        else if (current_.y > prev_.y) lineData.currentRot = 180;//up
        else if (current_.x < prev_.x) lineData.currentRot = 0; //down
        lineData.currentTile = levelManager.Instance.halfLineTile;
        if (lineData.prevRot != -1000)
            checkPrevDirection(lineData);
        return lineData;
    }
    lineData checkPrevDirection(lineData lineData_)
    {
        int diff = lineData_.prevRot - lineData_.currentRot;
        if (diff == 0) lineData_.prevTile = levelManager.Instance.fullLineTile;
        else if (diff == 90)
        {
            lineData_.prevTile = levelManager.Instance.angleLineTile;
            if (lineData_.prevRot == 0)
                lineData_.prevRot = 90;
            else if (lineData_.prevRot == 180)
                lineData_.prevRot = -90;
            else if (lineData_.prevRot == 90)
                lineData_.prevRot = 180;
        }
        else if (diff == -270)
        {
            lineData_.prevTile = levelManager.Instance.angleLineTile;
            lineData_.prevRot = 0;
        }
        else if (diff == 270) lineData_.prevTile = levelManager.Instance.angleLineTile;
        else if (diff == -90) lineData_.prevTile = levelManager.Instance.angleLineTile;
        else if (diff == 180) lineData_.prevTile = levelManager.Instance.fullLineTile;
        return lineData_;
    }

    void setGroupTile(Vector3Int pos_, Tile tile)
    {
        levelManager.Instance.brickTileMap.SetTile(pos_, tile);
        levelManager.Instance.lineTileMap.SetTile(pos_, tile);
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


