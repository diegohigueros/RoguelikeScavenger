using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour  // Allows us to create incomplete class members that must be declared in child classes. 
{
    // Declare public variables.
    public float moveTime = 0.1f;   // Speed of player and enemy movement.
    public LayerMask blockingLayer; // Determines if a space is open to be moved into. 

    // Declare private variables.
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;   // Stores a component reference to RigidBody of component moving. 
    private float inverseMoveTime;  // Makes calculations more efficient for moving. 

    // Start is called before the first frame update
    protected virtual void Start()  // Allows inheriting classes to override start. 
    {
        boxCollider = GetComponent<BoxCollider2D>();    // Gets boxcollider reference. 
        rb2D = GetComponent<Rigidbody2D>(); // Gets rigidbody2d reference. 
        inverseMoveTime = 1f / moveTime; // Allows us to multiply instead of divide computationally. 
    }

    // Returns true if we are moving. 
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)   // Out keyword allows us to return two values. 
    {
        Vector2 start = transform.position; // Vector3 that's cast and disregards z direction. 
        Vector2 end = start + new Vector2(xDir, yDir);  // Calculates the end position. 

        boxCollider.enabled = false; // Ensures we won't hit our own collider. 
        hit = Physics2D.Linecast(start, end, blockingLayer);    // Checks collisions on blocking layer.
        boxCollider.enabled = true; // Reenable the box collider. 

        // If we didn't hit anything, we will start moving the sprite. 
        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;    // Returns true per the movement was made successfully. 
        }

        return false;   // Returns false by default because the movement couldn't be made. 
    }

    // Attempts to specify the unit we expect to interact with if blocked. 
    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component  // Sets t = to component. Our player and enemy will inherit from this. 
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);   // Returns two values per out keyword. One value to canMove and the other to hit. 

        if(hit.transform == null)
        {
            return; // Since nothing was hit, return and don't execute hit instructions. 
        }

        T hitComponent = hit.transform.GetComponent<T>();   // Get the component that was hit. 

        // We can't move and the object can't be hit
        if(!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);   // Calls the functionality for us not being able to move. 
        }
    }

    // Allows us to move smoothly from one space to the next. 
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude; // Cheaper to use than just sqrMagnitude.     

        // Check that the sqrRemainingDistance is greater than the float epsilon. We will find a new position closer to end. 
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);    // Moves us closer to end. Moves in a straight line. Sets new position. 
            rb2D.MovePosition(newPosition); // Move us to the new position. 
            sqrRemainingDistance = (transform.position - end).sqrMagnitude; // Calculate remaining distance again. 
            yield return null;  // Allows us to wait for 1 frame before terminating the loop. 
        }
    }

    // Abstract again. This function will be overwritten. 
    protected abstract void OnCantMove<T>(T component) where T : Component; // Sets T = to component. Both enemy and player will inherit from this class. 
}
