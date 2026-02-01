using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{

    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] DirectionInfo[] directions;


    GameObject currentTile;


    Tile[,] cityGrid;
    [SerializeField] int gridSize = 10;
    [SerializeField] int tileSize = 100;
    
    void Start()
    {
        cityGrid = new Tile[gridSize, gridSize];
      
        GenerateCity();
        CheckAdjacentTiles();

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
