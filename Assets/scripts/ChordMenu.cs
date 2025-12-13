using UnityEngine;

public class ChordMenu : MonoBehaviour
{
    public GameObject chordPanel;

    public void ToggleChordPanel()
    {
        if (chordPanel != null)
        {
            chordPanel.SetActive(!chordPanel.activeSelf);
        }
    }
}