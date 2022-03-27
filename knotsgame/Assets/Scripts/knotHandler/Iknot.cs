using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// baseknot class uses this interface. For not there is not need for the interface segragation. 
/// however, its provision for for expandsion if more knots are getting added in the game that might have different condition for tile placement and removal. 
/// we can implement the different logic for those and keeping same signature methods. 
/// </summary>
interface IKnot
{
    void setTiles(Vector3Int cell_, Tile NodeTile_);
    void removeTile(Vector3Int cell_);
    void removeAllTile(Vector3Int currentPos, bool forceRemove = false);
    void removeOtherTiles(baseKnots baseKnots_, Vector3Int cell_);
}
