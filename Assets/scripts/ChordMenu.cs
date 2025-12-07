using UnityEngine;

public class ChordMenu : MonoBehaviour
{
    public GameObject chordPanel;

    public void ShowChordPanel()
    {
        chordPanel.SetActive(true);
    }

    public void HideChordPanel()
    {
        chordPanel.SetActive(false);
    }
}