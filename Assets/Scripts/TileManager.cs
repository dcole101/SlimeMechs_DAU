using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{

    [SerializeField] GameObject[] tilePrefabs;
    [SerializeField] DirectionInfo[] directions;
    [SerializeField] GenerationOptions genOptions;


    GameObject currentTile;


    Tile[,] cityGrid;
    [SerializeField] int gridSize = 10;
    [SerializeField] int tileSize = 100;
    
    void Awake()
    {
        cityGrid = new Tile[gridSize, gridSize];

        GenerateCity();
        if (genOptions.fillOpenSpaces) FillOpenSpaces();
        if (genOptions.connectCrossroads) ConnectCrossroads();
        if (genOptions.checkAdjacentTiles) CheckAdjacentTiles();
        if (genOptions.pruneEdges) PruneEdges();
        if (genOptions.deadEndRemoval) DeadEndRemoval();


    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
    }


    void GenerateCity()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector3 pos = GetTilePosition(x, y);
                GameObject goTile = Instantiate(tilePrefabs[0], pos, Quaternion.identity, transform);
                cityGrid[x, y] = new Tile(goTile, pos, 0);
               
            }
        }

        //outside loop
        int totalTiles = gridSize * gridSize;
        int numCrossings = Mathf.Max(1, Mathf.FloorToInt(totalTiles * 0.1f));
        List<Vector2Int> chosenPositions = new List<Vector2Int>();

        while(chosenPositions.Count < numCrossings)
        {
            int posX = Random.Range(0, gridSize);
            int posY = Random.Range(0, gridSize);
            Vector2Int pos = new Vector2Int(posX, posY);
            if(!chosenPositions.Contains(pos))
            {
                chosenPositions.Add(pos);
                ReplaceTile(posX, posY, 15);
            }
        }

    }

    Vector3 GetTilePosition(int x, int y)
    {
        float offset = gridSize * tileSize / 2f - tileSize / 2f;
        float worldX = x * tileSize - offset;
        float worldY = y * tileSize - offset;
        
       return  new Vector3(worldX, 0, worldY);
    }

    void ReplaceTile(int x, int y, int index)
    { 
        Tile tile = cityGrid[x, y];
        Destroy(tile.tileObject);
        GameObject goTile = Instantiate(tilePrefabs[index], tile.position, Quaternion.identity, transform);
        tile.tileObject = goTile;
        tile.bitValue = index;
    }

    void CheckAdjacentTiles()
    {
        bool isChanged = true;
        int maxAttempts = 100;
        int numAttempts = 0;
        while(isChanged && numAttempts < maxAttempts)
        {
            isChanged = false;
            numAttempts++;
            //loop through every tile in the grid
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Tile currentTile = cityGrid[x, y];
                    if (currentTile.bitValue > 0)
                    {
                        foreach (DirectionInfo dir in directions)
                        {
                            if ((currentTile.bitValue & dir.bit) != 0)
                            {
                                //we are making sure the adj tile is in bounds
                                int myX = x + dir.offset.x;
                                int myY = y + dir.offset.y;
                                if (myX >= 0 && myX < gridSize && myY >= 0 && myY < gridSize)
                                {
                                    Tile adjacentTile = cityGrid[myX, myY];
                                    if (adjacentTile.bitValue == 0)
                                    {
                                        int reqBit = GetOppositeBit(dir.bit);
                                        List<int> availableTilesList = GetAvailableTiles(reqBit);
                                        if (availableTilesList.Count > 0)
                                        {
                                            int randIndex = Random.Range(0, availableTilesList.Count);
                                            int bitVal = availableTilesList[randIndex];
                                            ReplaceTile(myX, myY, bitVal);
                                            isChanged = true;
                                        }
                                    }
                                    else
                                    {
                                        //connect the adj tile by turning on the opposite bit (opp of direction bit)
                                        int requiredBit = GetOppositeBit(dir.bit);
                                        if(!HasConnection(adjacentTile.bitValue, requiredBit))
                                        {
                                            int newBitVal = ApplyConnectionBit(adjacentTile.bitValue, requiredBit);
                                            ReplaceTile(myX, myY, newBitVal);
                                            isChanged = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        
    }

    int GetOppositeBit(int bit)
    {
        switch (bit)
        {
            case 1: return 4;
            case 2: return 8;
            case 4: return 1;
            case 8: return 2;
            default: return 0;
        }
    }

    List<int> GetAvailableTiles(int reqBit)
    {
        List<int> indexList = new List<int>();
        for(int i = 0; i < tilePrefabs.Length; i++)
        {
            bool isAllowed = i != 1 && i != 2 && i != 4 && i != 8;
            bool isConnected = (i & reqBit) == reqBit;
            if (isAllowed && isConnected)
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }

    void ConnectCrossroads()
    {
        List<Vector2Int> crossroads = new List<Vector2Int>();
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if(cityGrid[x, y].bitValue == 15)
                {
                    crossroads.Add(new Vector2Int(x, y));
                }
            }
        }
        if(crossroads.Count < 2)
        { return;  }
        for(int i = 0; i < crossroads.Count - 1; i++)
        {
            Vector2Int curPos = crossroads[i];
            Vector2Int targetPos = crossroads[i + 1];
            Vector2Int prevPos = curPos;
            while (prevPos != targetPos)
            {
                //find the movement directions
                Vector2Int moveDir = Vector2Int.zero;
                if (curPos.x != targetPos.x)
                {
                    int x = curPos.x < targetPos.x ? 1 : -1;
                    moveDir = new Vector2Int(x, 0);
                }
                else if (curPos.y != targetPos.y)
                {
                    int y = curPos.y < targetPos.y ? 1 : -1;
                    moveDir = new Vector2Int(0, y);
                }
                curPos += moveDir;
                //Determine the required bit for the current move
                //south = bit 4
                // west connection = bit 8
                //     [ - 1 - ]
                //     [ 8 - 2 ]
                //     [ - 4 - ]
                int requiredBit = 0;
                if(moveDir.y == -1) { requiredBit = 1; }
                if(moveDir.x == -1) {requiredBit = 2; }
                if(moveDir.y == 1) { requiredBit = 4; } //we are making the north tile to the current tile, so the south tile has to be the 4
                if(moveDir.x == 1) { requiredBit = 8; }
                int curBitVal = cityGrid[curPos.x, curPos.y].bitValue;
                int newBitVal = curBitVal == 0 ? requiredBit : curBitVal | requiredBit;

                //fixing the previous tile
                int prevBitVal = cityGrid[prevPos.x, prevPos.y].bitValue;
                int oppositebit = GetOppositeBit(requiredBit);
                bool isConnected = (prevBitVal & oppositebit) > 0;
                if (!isConnected)
                {
                    //ensure the prev tile connects to this tile
                    prevBitVal |= oppositebit;
                    ReplaceTile(prevPos.x, prevPos.y, prevBitVal);
                }
                ReplaceTile(curPos.x, curPos.y, newBitVal);
                prevPos = curPos;
            }
        }
    }

    void PruneEdges()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                bool isNorth = y == gridSize - 1;
                bool isEast = x == gridSize - 1;
                bool isSouth = y == 0;
                bool isWest = x == 0;
                if (isNorth || isEast || isSouth || isWest)
                {
                    Tile tile = cityGrid[x, y];
                    int bitVal = tile.bitValue;
                    if (isNorth) { bitVal = RemoveConnectionBit(bitVal, 1); }
                    if (isEast) { bitVal = RemoveConnectionBit(bitVal, 2); }
                    if (isSouth) { bitVal = RemoveConnectionBit(bitVal, 4); }
                    if (isWest) { bitVal = RemoveConnectionBit(bitVal, 8); }
                    if (bitVal == 1 || bitVal == 2 || bitVal == 4 || bitVal == 8)
                    {
                        bitVal = 0;
                    }
                    if (bitVal != tile.bitValue)
                    {
                        ReplaceTile(x, y, bitVal);
                    }
                }
            }
        }
    }

    // Adds a directional connection (performs a binary OR)
    int ApplyConnectionBit(int currentValue, int connectionBit)
    {
        return currentValue | connectionBit;
    }

    // Checks whether a specific connection exists (performs a binary AND)
    bool HasConnection(int currentValue, int connectionBit)
    {
        return (currentValue & connectionBit) > 0;
    }

    // Removes a directional connection (clears a bit using a binary AND with NOT)
    int RemoveConnectionBit(int currentValue, int connectionBit)
    {
        return currentValue & ~connectionBit;
    }


    void DeadEndRemoval()
    {
        bool isChanged = true;
        while (isChanged)
        {
            isChanged = false;
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Tile tile = cityGrid[x, y];
                    if (tile.bitValue > 0)
                    {
                        int bitVal = tile.bitValue;
                        foreach (DirectionInfo dir in directions)
                        {
                            Vector2Int myPos = new Vector2Int(x + dir.offset.x, y + dir.offset.y);
                            bool isWithinBounds = myPos.x >= 0 && myPos.x < gridSize && myPos.y >= 0 && myPos.y < gridSize;
                            if (isWithinBounds)
                            {
                                Tile adjacentTile = cityGrid[myPos.x, myPos.y];
                                // If the adjacent tile does NOT have the matching connection, remove this bit
                                if (!HasConnection(adjacentTile.bitValue, GetOppositeBit(dir.bit)))
                                {
                                    bitVal = RemoveConnectionBit(bitVal, dir.bit);
                                }
                            }
                        }
                        // If the tile's bitValue has changed, replace it
                        if (bitVal == 1 || bitVal == 2 || bitVal == 4 || bitVal == 8)
                        {
                            bitVal = 0;
                        }
                        if (bitVal != tile.bitValue)
                        {
                            ReplaceTile(x, y, bitVal);
                            isChanged = true;
                        }
                    }
                }
            }
        }
    }

    void FillOpenSpaces()
    {
        //find center tile?
        for (int y = 1; y < gridSize - 1; y++)
        {
            for (int x = 1; x < gridSize - 1; x++)
            {
                if (cityGrid[x, y].bitValue == 0)
                {
                    bool isLargeArea = true;
                    for (int offsetY = -1; isLargeArea && offsetY <= 1; offsetY++)
                    {
                        for (int offsetX = -1; isLargeArea && offsetX <= 1; offsetX++)
                        {
                                if (offsetX == 0 && offsetY == 0) continue;
                                if (cityGrid[x + offsetX, y + offsetY].bitValue != 0)
                                {
                                    isLargeArea = false;
                                }
                            
                        }
                    }
                    if(isLargeArea)
                    {
                        ReplaceTile(x, y, 15);
                    }
                }
            }
        }
    }



    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //    {
    //        SpawnRandomTile();
    //    }
    //}

    //void SpawnRandomTile()
    //{
    //    if (currentTile != null)
    //    {
    //        Destroy(currentTile);
    //    }
    //    if (tileHistory.Count >= tilePrefabs.Length)
    //    {
    //        tileHistory.Clear();
    //    }
    //    int randomIndex = GetRandomTileIndex();
    //    currentTile = Instantiate(tilePrefabs[randomIndex]);
    //    tileHistory.Add(randomIndex);
    //}

    //int GetRandomTileIndex()
    //{
    //    int index = Random.Range(0, tilePrefabs.Length);

    //    while (tileHistory.Contains(index))
    //    {
    //        index = Random.Range(0, tilePrefabs.Length);
    //    }

    //    return index;
    //}
}
