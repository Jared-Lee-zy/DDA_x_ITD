using UnityEngine;

public class ChordButton : MonoBehaviour
{
    public GameObject aChordImage;
    public GameObject cChordImage;
    public GameObject gChordImage;
    public GameObject dChordImage;
    public GameObject eChordImage;


    public void ShowAChord()
    {
        aChordImage.SetActive(true);
    }

    public void HideAChord()
    {
        aChordImage.SetActive(false);
    }

    public void ShowCChord()
    {
        cChordImage.SetActive(true);
    }
    public void HideCChord()
    {
        cChordImage.SetActive(false);
    }
    public void ShowGChord()
    {
        gChordImage.SetActive(true);
    }
    public void HideGChord()
    {
        gChordImage.SetActive(false);
    }
    public void ShowDChord()
    {
        dChordImage.SetActive(true);
    }
    public void HideDChord()
    {
        dChordImage.SetActive(false);
    }
    public void ShowEChord()
    {
        eChordImage.SetActive(true);
    }
    public void HideEChord()
    {
        eChordImage.SetActive(false);
    }
}
