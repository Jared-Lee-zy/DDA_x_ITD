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
        cChordImage.SetActive(false);
        gChordImage.SetActive(false);
        dChordImage.SetActive(false);
        eChordImage.SetActive(false);
    }

    public void HideAChord()
    {
        aChordImage.SetActive(false);
    }

    public void ShowCChord()
    {
        cChordImage.SetActive(true);
        aChordImage.SetActive(false);
        gChordImage.SetActive(false);
        dChordImage.SetActive(false);
        eChordImage.SetActive(false);
    }
    public void HideCChord()
    {
        cChordImage.SetActive(false);
    }
    public void ShowGChord()
    {
        gChordImage.SetActive(true);
        aChordImage.SetActive(false);
        cChordImage.SetActive(false);
        dChordImage.SetActive(false);
        eChordImage.SetActive(false);
    }
    public void HideGChord()
    {
        gChordImage.SetActive(false);
    }
    public void ShowDChord()
    {
        dChordImage.SetActive(true);
        aChordImage.SetActive(false);
        cChordImage.SetActive(false);
        gChordImage.SetActive(false);
        eChordImage.SetActive(false);
    }
    public void HideDChord()
    {
        dChordImage.SetActive(false);
    }
    public void ShowEChord()
    {
        eChordImage.SetActive(true);
        aChordImage.SetActive(false);
        cChordImage.SetActive(false);
        gChordImage.SetActive(false);
        dChordImage.SetActive(false);
    }
    public void HideEChord()
    {
        eChordImage.SetActive(false);
    }
}
