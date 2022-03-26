using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;


/// <summary>
/// SO that hold the color id and also its default tiles
/// </summary>
[CreateAssetMenu(fileName = "colotData", menuName = "colordata")]
public class colorNodeData : ScriptableObject
{
    [SerializeField] List<colorIDData> colorIDDatas;
    [SerializeField] Tile BGTile, halfLineTile, angleLineTile, fullLineTile, nodeLineTile;
    

    #region getters
    public List<colorIDData> colorIDDatasList()
    {
        return colorIDDatas;
    }
    public Tile getBGTile()
    {
        return BGTile;
    }
    public Tile getHalfLineTile()
    {
        return halfLineTile;
    }
    public Tile getAngleLineTile()
    {
        return angleLineTile;
    }
    public Tile getFullLineTile()
    {
        return fullLineTile;
    }
    public Tile getNodeLineTile()
    {
        return nodeLineTile;
    }
    #endregion
}

[Serializable]
public class colorIDData
{
    [SerializeField]  int id;
    [SerializeField]  Tile nodeTile;
    [SerializeField]  Tile bricktile;

    #region getters
    public int getID()
    {
        return id;
    }

    public Tile getNodeTile()
    {
        return nodeTile;
    }
    public Tile getBrickTile()
    {
        return bricktile;
    }
    #endregion
}

public enum colorType { red,blue,green };

