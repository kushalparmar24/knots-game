using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "colotData", menuName = "colordata")]
public class colorNodeData : ScriptableObject
{
    public List<colorIDData> colorIDDatas;
}

[Serializable]
public class colorIDData
{
    public int id;
    public Tile nodeTile;
    public colorType colorType;
}

public enum colorType { red,blue,green };

