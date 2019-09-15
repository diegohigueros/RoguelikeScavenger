using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // Allows us to use UI elements
// using UnityEngine.SceneManagement;  // OnLevelWasLoaded will no longer work but this will

public class GameManager : MonoBehaviour
{
    // Declare public variables. 
    public float levelStartDelay = 2f;   // Time to wait before starting levels in seconds. 
    public float turnDelay = .1f;   // How long the game waits between turns. 
    public BoardManager boardScript;
    public static GameManager instance = null;  // Creates a singleton. Only one instance allowed. 
    public int playerFoodPoints = 100;  // Default food points of 100. 
    [HideInInspector] public bool playersTurn = true;   // Sets the players turn automatically but 

    // Declare private variables. 
    private Text levelText; // Displays the current level number.
    private GameObject levelImage;  // Stores a reference to level image. 
    private int level = 1;  // We want to test level 3 because that's when enemies appear. Testing worked so now set to level 1. 
    private List<Enemy> enemies;    // List of all enemies.
    private bool enemiesMoving; // Determins if enemies are moving.
    private bool doingSetup = true;    // Prevents player from moving during setup. 

    // We will get and store a component reference to our board manager script and init game function. 
    void Awake()
    {
        if(instance == null)    // If an instance hasn't been created, we will assign it to this. 
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);    // If the gameObject created isn't this one, destroy it because we don't want two instances. 
        }

        DontDestroyOnLoad(gameObject);    // When we load a new scene, we don't want to destroy this. 
        enemies = new List<Enemy>();    // Create a new list of enemies. 
        boardScript = GetComponent<BoardManager>(); // Sets the reference. 
        InitGame(); // Start the game. 
    }

    // Called every time a scene is loaded. Add to level number and start level.
    private void OnLevelWasLoaded(int index)
    {
        level++;    // Increment level.
        InitGame(); // Start level again. 
    }

    // Initialize the game and board. 
    private void InitGame()
    {
        doingSetup = true;  // Prevent player from moving. 
        levelImage = GameObject.Find("LevelImage"); // Get reference to levelImage.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();  // Sets leveltext component to leveltext variable.
        levelText.text = "Day " + level;    // Print the correct string. 
        levelImage.SetActive(true); // Activate the level image.
        Invoke("HideLevelImage", levelStartDelay);  // Hide image and wait 2 seconds before level.

        enemies.Clear();    // Clear the board of any enemies. 
        boardScript.SetUpScene(level);  // Sets up level 3. 
    }

    // Turns off the level image. 
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false; // Allow the player to move again. 
    }

    // Disables gameManager when the game is over. 
    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";  // print out game over message. 
        levelImage.SetActive(true); // Set black background again.
        enabled = false;
    }

    // Move our enemies one at a time in sequence. 
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;   // Sets enemies moving true.
        yield return new WaitForSeconds(turnDelay); // Wait for .1 seconds.
        if(enemies.Count == 0)  // Check if no enemies have been responded yet. 
        {
            yield return new WaitForSeconds(turnDelay); // Wait for .1 seconds.
        }

        // Loop through list and allow enemies to move. 
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy(); // Move each enemy on the board. 
            yield return new WaitForSeconds(enemies[i].moveTime); // Wait for .1 seconds. Wait before calling next one. 
        }

        playersTurn = true;     // It's the player's turn again. 
        enemiesMoving = false;  // Enemies are no longer moving. 
    }

    // Update is called once per frame
    void Update()
    {
        if(playersTurn || enemiesMoving || doingSetup)
        {
            return; // Don't execute code since enemies currently moving or it's the players turn. 
        }

        StartCoroutine(MoveEnemies());  // Start moving enemies. 
    }

    // Add enemies to the list. 
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);    // Adds enemy to list. 
    }
}
