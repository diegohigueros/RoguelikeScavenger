using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    // Create public variables. 
    public GameObject gameManager;

    // Awake is called on initial start up
    void Awake()
    {
        // Check if a gameManager exists and if not, create one.
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);   // Creates gameManager. 
        }
    }
}
