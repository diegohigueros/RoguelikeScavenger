using System.Collections;
using System;   // Allows us to use serializable attribute. 
using System.Collections.Generic;   // Allows us to use lists. 
using UnityEngine;
using Random = UnityEngine.Random;  // Allows us to randomize board. 

public class BoardManager : MonoBehaviour
{
    // Declare serializable variables
    [Serializable] 
    public class Count
    {
        public int maximum;
        public int minimum;

        // Assignment Constructor with two parameters. 
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Declare public variables.  
    public int columns = 8;     // Board size will be 8 x 8 
    public int rows = 8;        // Board size will be 8 x 8 
    public Count wallCount = new Count(5, 9);   // We will have a min of 5 and max of 9 walls per level. 
    public Count foodCount = new Count(1, 5);   // We will have a min of 1 and max of 5 walls per level. 
    public GameObject exit;     // Only one exit per level.
    public GameObject[] floorTiles; // Array of floor tiles. 
    public GameObject[] wallTiles; // Array of wall tiles. 
    public GameObject[] foodTiles; // Array of food tiles. 
    public GameObject[] enemyTiles; // Array of enemy tiles. 
    public GameObject[] outerWallTiles; // Array of outer wall tiles. 

    // Declare private variables.
    private Transform boardHolder;  // Something to keep hierarchy clean. All items will be a child of it. 
    private List<Vector3> gridPositions = new List<Vector3>();  // Holds all different possible positions and whether an object has spawned there. 
    
    void InitializeList()
    {
        gridPositions.Clear();  // Clear the list. 

        // Use a pair of nested for loops to fill gameboard.
        for(int x = 1; x < columns - 1; x++)
        {
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));   // Fill up the board. 
            }
        }
    }

    // Sets up the outer wall and the floor of the gameboard. 
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;    // Creates new game object board.

        // Fill up the entire board tiles, including the outer wall objects. 
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];  // Set the object equal to a random floor tile. 
                // Check if we're in an outer wall position. If we are, use an outer wall tile.
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)]; // Set the object equal to a random outer wall tile. 
                }
                // Instantiate the object. 
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; // z index is 0 per 2D. Quaternian.identity means it won't rotate. 
                instance.transform.SetParent(boardHolder);  // Set the parent to boardholder. 
            }
        }
    }

    // Returns a random vector3 position. 
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count); // Creates a random index based on the count of grid positions. 
        Vector3 randomPosition = gridPositions[randomIndex];    // Get a random position based on the random index. 
        gridPositions.RemoveAt(randomIndex);    // Remove it so no two positions are duplicated. 
        return randomPosition;  // Returns position. 
    }

    // The tiles are actually spawned at the random position.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);   // Controls how many objects are spawned. 
        // Spawn the number of objects specified. 
        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();  // Call the RandomPosition function. 
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];   // Get a random tile sprite. 
            Instantiate(tileChoice, randomPosition, Quaternion.identity);   // Quaternion.identity means no rotate. Spawn the tile. 
        }

    }

    // This function will be called by game manager and will set up the entire board. Calls all other functions to set up board. 
    public void SetUpScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);  // Layout walls
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);  // Layout food
        int enemyCount = (int)Mathf.Log(level, 2f);  // The difficulty level will scale up logarithmatically. Level 2 = 1, Level 4 = 2, Level 8 = 3. 
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);   // Layout enemies. 
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity); // Exit will always be at the exact same position in top right corner. 
    }

    // Update is called once per frame
    void Update()
    {
        // Added this extra code to help the player escape from the executable.
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
