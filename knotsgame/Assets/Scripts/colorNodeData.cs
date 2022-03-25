using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "colotData", menuName = "colordata")]
public class colorNodeData : ScriptableObject
{
    [SerializeField] List<colorIDData> colorIDDatas;

    public List<colorIDData> colorIDDatasList()
    {
        return colorIDDatas;
    }
}

[Serializable]
public class colorIDData
{
    [SerializeField]  int id;
    [SerializeField]  Tile nodeTile;
    [SerializeField]  Tile bricktile;

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
}

public enum colorType { red,blue,green };

