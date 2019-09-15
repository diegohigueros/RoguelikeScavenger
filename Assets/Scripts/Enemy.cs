using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class inherits from moving object
public class Enemy : MovingObject
{
    // Declare public variables.
    public int playerDamage;    // The damage done to player.
    public AudioClip enemyAttack1;  // First instance.
    public AudioClip enemyAttack2;  // Second instance.

    // Declare private variables.  
    private Animator animator;
    private Transform target;   // Stores the player position and tells enemy where to move towards. 
    private bool skipMove;  // Skips every other turn. 

    // Start is called before the first frame update
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);  // Have enemy script add itself to list of enemies in GameManager. 
        animator = GetComponent<Animator>();    // Gets a reference to animator component
        target = GameObject.FindGameObjectWithTag("Player").transform; // Finds the player
        base.Start();   // Call start function in base class. 
    }

    // Overrides enemy. Enemy will interact with player component. 
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // Make sure the enemy is only moving every other turn. 
        if(skipMove)
        {
            skipMove = false;
            return;
        }

        // Call the base attempt move function. 
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;    // Ensure we skip turn.
    }

    // Ensure we move the enemy on enemy turn.
    public void MoveEnemy()
    {
        // Store our movement direction. 
        int xDir = 0;
        int yDir = 0;
        // Check if the player and enemy are in same column.
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) {
            yDir = target.position.y > transform.position.y ? 1 : -1;   // If the player is above, move up. If below, move down. 
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;   // If the player is left, move left. If right, move right. 
        }
        AttemptMove<Player>(xDir, yDir);    // Call movement function with new directions.
    }

    // Allows us to hit the player
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player; // Allow us to hit the player. 
        hitPlayer.LoseFood(playerDamage);   // Player loses health. 
        animator.SetTrigger("enemyAttack");    // Sets enemy attack trigger. 
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2); // Pass both enemy attack sounds. 
    }
}
