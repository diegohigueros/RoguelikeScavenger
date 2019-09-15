using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Declare public variables.
    public AudioSource efxSource;   // Holds reference to audio effects. 
    public AudioSource musicSource; // Holds reference to music.
    public static SoundManager instance = null; // Sound manager is in all scope. 

    public float lowPitchRange = .95f;  // Represents minus 5% of original pitch.
    public float highPitchRange = 1.05f;    // Represents plus 5% of original pitch.
     
    // Awake is called upon start up
    void Awake()
    {
        if(instance == null)
        {
            instance = this;    // Set the game object if it doesn't exist. 
        }
        else if(instance != null)
        {
            Destroy(gameObject);    // If it does exist, don't create a duplicate. 
        }

        DontDestroyOnLoad(gameObject);    // Don't destroy sound manager on load. 
    }

    // Play the single audio clip. 
    public void PlayingSingle(AudioClip clip)
    {
        efxSource.clip = clip;  // Set the clip.
        efxSource.Play(); // Play the clip.
    }

    // Randomize the effects clips.
    public void RandomizeSfx(params AudioClip[] clips)  // Params key word allows multiple parameters separated by comma. 
    {
        int randomIndex = Random.Range(0, clips.Length); // Get a random index. Will use to choose which effect to play. 
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);    // Get a randomPitch between high and low. 

        efxSource.pitch = randomPitch;  // Set pitch to randomPitch.
        efxSource.clip = clips[randomIndex];    // Set clip to random clip.
        efxSource.Play();   // Play the clip. 
    }
}
