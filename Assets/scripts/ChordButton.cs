///Made By Lim Xue Zhi Conan
/// Date Of Creation  : 2025-12-04
/// Function of Script: Manages the display of chord images and tracks progress.
using UnityEngine;
using TMPro;
/// <summary>
/// Manages the display of chord images and tracks progress.
/// Shows one chord image at a time and updates the progress text
/// as the user views different chords. 
/// </summary>       

public class ChordButton : MonoBehaviour
{
    /// <summary>
    /// Chord image GameObjects.
    /// </summary>
    public GameObject aChordImage;
    public GameObject cChordImage;
    public GameObject gChordImage;
    public GameObject dChordImage;
    public GameObject eChordImage;

    /// <summary>
    /// Text element to show learning progress. 
    public TextMeshProUGUI progressText;

    /// <summary>
    /// Total number of chords and viewed count.
    /// </summary>
    private int totalChords = 5;
    private int viewedCount = 0;


    /// <summary>
    /// Tracks which chords have been viewed.
    /// </summary>
    private bool viewedA = false;
    private bool viewedC = false;
    private bool viewedG = false;
    private bool viewedD = false;
    private bool viewedE = false;


    /// <summary>
    /// Initializes the progress text on start.
    /// </summary>
    void Start()
    {
        UpdateProgressText();
    }
    /// <summary>
    /// Updates the progress text display.      
    /// </summary>
    private void UpdateProgressText()
    {
        progressText.text = $"Chords Learnt: {viewedCount} / {totalChords}";
    }


    /// <summary>
    /// Shows the A chord image and updates progress if not viewed before.
    /// </summary>
    public void ShowAChord()
    {
        ShowOnly(aChordImage);

        if (!viewedA)
        {
            viewedA = true;
            viewedCount++;
            UpdateProgressText();
        }
    }

    /// <summary>
    /// Hides the A chord image.
    /// </summary>
    public void HideAChord()
    {
        aChordImage.SetActive(false);
    }

    public void ShowCChord()
    {
        ShowOnly(cChordImage);

        if (!viewedC)
        {
            viewedC = true;
            viewedCount++;
            UpdateProgressText();
        }
    }


    public void HideCChord()
    {
        cChordImage.SetActive(false);
    }

    public void ShowGChord()
    {
        ShowOnly(gChordImage);

        if (!viewedG)
        {
            viewedG = true;
            viewedCount++;
            UpdateProgressText();
        }
    }

    public void HideGChord()
    {
        gChordImage.SetActive(false);
    }

    public void ShowDChord()
    {
        ShowOnly(dChordImage);

        if (!viewedD)
        {
            viewedD = true;
            viewedCount++;
            UpdateProgressText();
        }
    }

    public void HideDChord()
    {
        dChordImage.SetActive(false);
    }
    public void ShowEChord()
    {
        ShowOnly(eChordImage);

        if (!viewedE)
        {
            viewedE = true;
            viewedCount++;
            UpdateProgressText();
        }
    }
    
    public void HideEChord()
    {
        eChordImage.SetActive(false);
    }


    /// <summary>
    /// Hides all chord images and shows only the specified one.
    /// </summary>
    private void ShowOnly(GameObject chordToShow)
    {
        aChordImage.SetActive(false);
        cChordImage.SetActive(false);
        gChordImage.SetActive(false);
        dChordImage.SetActive(false);
        eChordImage.SetActive(false);

        chordToShow.SetActive(true);
    }
}

