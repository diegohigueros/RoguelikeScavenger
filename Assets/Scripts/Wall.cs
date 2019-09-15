using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Declare public variables.
    public Sprite dmgSprite;    // Allows player to see they destroyed the wall. 
    public int hp = 4;      // Hit points
    public AudioClip chopSound1;    // First instance.
    public AudioClip chopSound2;    // Second instance. 

    private SpriteRenderer spriteRenderer; // Renders the sprites. 

    // Awake is called on set up
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    // Gets reference to sprite renderer. 
    }

    // Damages and removes wall from the game. 
    public void DamageWall(int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2); // Pass both chop wall sounds. 
        // Set the sprite of spriteRenderer to damaged sprite. 
        spriteRenderer.sprite = dmgSprite;
        hp -= loss; // Subtract points from wall health so it breaks. 

        if(hp <= 0) // If the wall has no more health, make it dissappear. 
        {
            gameObject.SetActive(false);
        }
    }
}
