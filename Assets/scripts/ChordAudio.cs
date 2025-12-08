using UnityEngine;

public class ChordAudio : MonoBehaviour
{
    public AudioClip chordClip;

    public void PlayChord()
    {
        if (chordClip == null)
        {
            Debug.LogWarning("Chord clip is missing!");
            return;
        }

        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.playOnAwake = false;
        tempSource.clip = chordClip;

        tempSource.Play();
        
        Destroy(tempSource, chordClip.length);
    }
}
