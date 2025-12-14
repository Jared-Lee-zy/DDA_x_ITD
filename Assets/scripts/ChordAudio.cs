using UnityEngine;
/// <summary>
///Made By Lim Xue Zhi Conan
/// Date Of Creation  : 7/12/2025
/// Function of Script: Plays the audio for a specific chord when triggered.
/// Attaches an AudioSource component at runtime to play the chord sound.
/// Removes the AudioSource after playback to clean up.
/// </summary>
public class ChordAudio : MonoBehaviour
{
    public AudioClip chordClip;


    /// <summary>
    /// Plays the chord audio clip.     
    /// </summary>
    public void PlayChord()
    {
        if (chordClip == null)
        {
            Debug.LogWarning("Chord clip is missing!");
            return;
        }

    /// <summary>
    /// Creates a temporary AudioSource to play the chord clip and destroys it after playback.  
    /// </summary>
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.playOnAwake = false;
        tempSource.clip = chordClip;

        tempSource.Play();
        
        Destroy(tempSource, chordClip.length);
    }
}
