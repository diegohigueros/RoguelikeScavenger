using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Allows us to reload the scene. 
using UnityEngine.UI;   // Allows UI functionality.

public class Player : MovingObject  // Inherits from movingObject instead of monobehavior. 
{
    // Declare public variables. 
    public int wallDamage = 1;  // Player does a default damage of 1
    public int pointsPerFood = 10;  // Adds 10 foodpoints
    public int pointsPerSoda = 20;  // Adds 20 foodpoints
    public float restartLevelDelay = 1f;    // Restarts the level 
    public Text foodText;   // Prints food to screen

    public AudioClip moveSound1;    // First instance
    public AudioClip moveSound2;    // Second instance  
    public AudioClip eatSound1; // First instance
    public AudioClip eatSound2; // Second instance
    public AudioClip drinkSound1;   // First instance
    public AudioClip drinkSound2;   // Second instance
    public AudioClip gameOverSound; // Game over sound.

    // Declare private variables.
    private Animator animator;  // Stores a reference to animator component.
    private int food;   // Stores the player score during the level before passing back to gameManager. 
    private Vector2 touchOrigin = -Vector2.one;    // Will be used to check if there has been touch input. 

    // Start is called before the first frame update
    protected override void Start() // We are overriding the abstract class method. 
    {
        animator = GetComponent<Animator>();    // Gets reference to animator component.
        food = GameManager.instance.playerFoodPoints;   // Allows us to get the food points while the player is in the level. 
        foodText.text = "Food: " + food;   // Set foodtext to food score.

        base.Start();   // Call base start function.
    }

    // When the player game object is disabled, the value of food will be stored in game manager. 
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;   // Pass the value back to GameManager. 
    }

    // Check if the game is over and should be ended. 
    private void CheckIfGameOver()
    {
        if (food <= 0)   // End the game if the player is out of food. 
        {
            SoundManager.instance.PlayingSingle(gameOverSound); // Play the game over sound.
            SoundManager.instance.musicSource.Stop();   // Stop the music source from playing. 
            GameManager.instance.GameOver();
        }
    }

    // This will allow functionality for player attempting to move. 
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--; // Subtract one food point every time player moves. 
        foodText.text = "Food: " + food;    // Set foodText again.
        base.AttemptMove<T>(xDir, yDir);    // Call base class. 
        RaycastHit2D hit;   // Allow us to reference result of linecast done in move. 
        if(Move(xDir, yDir, out hit))   // If the movement was successful, play appropriate random effect. 
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2); // Pass both move sounds. 
        }

        CheckIfGameOver();  // Ensure the game isn't over. 

        GameManager.instance.playersTurn = false;   // It's not longer the player's turn to move. 
    }

    // Update is called once per frame
    private void Update()
    {
        // If the player's turn is over, don't execute update code. 
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        // Store the direction the player is moving in. 
        int horizontal = 0;
        int vertical = 0;

        // Check the platform for movement purposes. 
#if UNITY_STANDALONE || UNITY_WEBPLAYER // || UNITY_EDITOR  // Allows us to use the keyboard still. 
        horizontal = (int)Input.GetAxisRaw("Horizontal");  // This uses keyboard input. 
        vertical = (int)Input.GetAxisRaw("Vertical");

        // Prevent the player from moving diagonally. 
        if (horizontal != 0)
        {
            vertical = 0;
        }
#else
        // One or more touches have been registered. 
        if(Input.touchCount > 0) {
            Touch myTouch = Input.touches[0];   // Grab only the first touch. 
            if(myTouch.phase == TouchPhase.Began) // Make sure the touch was the beginning of a touch. 
            { 
                touchOrigin = myTouch.position; // Set origin equal to myTouch position.
            }
            else if (myTouch.phase != TouchPhase.Ended && touchOrigin.x >= 0) // If the touch is over or the touch is still inside bounds and changed.
            {
                Vector2 touchEnd = myTouch.position;    // Store the ending position. 
                float x = touchEnd.x - touchOrigin.x;   // Compare the difference between two touches. Gets us a direction to move. 
                float y = touchEnd.y - touchOrigin.y;   // Compare the vertical direction. 
                touchOrigin.x = -1; // Make sure we don't break our conditional. 
                // Check the direction the player swiped in.
                if(Mathf.Abs(x) > Mathf.Abs(y)) // The horizontal swipe was greater than vertical swipe. 
                {
                    horizontal = x > 0 ? 1 : -1;    // Check if the direction was valid. 
                }
                else
                {
                    vertical = y > 0 ? 1 : -1; 
                }
            }
        }
#endif

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);    // We are expecting that the player may encounter a wall. 
        }
    }

    // Override OnCantMove. We want player to hit the wall if they're blocked by wall. 
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;   // Gets reference from component. 
        hitWall.DamageWall(wallDamage); // Allow player to damage wall. 
        animator.SetTrigger("playerChop");  // Set the animation to chopping animation. 
    }

    // Declare a function to restart the level if they collide with exit.
    private void Restart()
    {
        //SceneManager.LoadScene(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        //Application.LoadLevel(Application.loadedLevel); // Restarts the last scence. In other games we would load a different scene. 
    }

    // Decrease the food value for player. 
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");   // Set animation to player being hit.
        food -= loss;   // Subtract the loss. 
        foodText.text = "-" + loss + " Food: " + food;    // Show how many food points lost.
        CheckIfGameOver();  // Check if the game has now ended. 
    }

    // Allow the player to interact with food and soda. 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);   // Call the function 1 second after collision before restart. 
            enabled = false;    // The level is now over.
        }
        else if(other.tag == "Food")
        {
            food += pointsPerFood;  // Add the food points to total.
            foodText.text = "+" + pointsPerFood + " Food: " + food;    // Show how many food points were gained.
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2); // Pass both eat sounds. 
            other.gameObject.SetActive(false);  // Disable the food object and make it disappear. 
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;  // Add soda points to total. 
            foodText.text = "+" + pointsPerSoda + " Food: " + food;    // Show how many food points were gained.
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2); // Pass both drink sounds. 
            other.gameObject.SetActive(false);  // Disable the food object and make it disappear. 
        }
    }

}
