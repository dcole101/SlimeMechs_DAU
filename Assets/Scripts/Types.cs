using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Tile
{
    public GameObject tileObject;
    public Vector3 position;
    public int bitValue;

    public Tile(GameObject tileObject, Vector3 position, int bitValue)
    {
        this.tileObject = tileObject; 
        this.position = position; 
        this.bitValue = bitValue;

    }
}

[System.Serializable]
public struct DirectionInfo
{
    public int bit;
    public Vector2Int offset;
    public DirectionInfo(int bit, int x, int y)
    {
        this.bit = bit;
        this.offset = new Vector2Int(x, y);
    }
}

[System.Serializable]
public class GenerationOptions
{
    public bool fillOpenSpaces;
        public bool connectCrossroads;
    public bool checkAdjacentTiles;
    public bool pruneEdges;
        public bool deadEndRemoval;

}