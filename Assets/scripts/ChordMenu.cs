using UnityEngine;
///Made By Lim Xue Zhi Conan
/// Date Of Creation  : 7/12/2025
/// Function of Script: Manages the chord menu panel visibility.
public class ChordMenu : MonoBehaviour
{
    public GameObject chordPanel;
    /// <summary>
    ///Toggles the visibility of the chord panel.
    /// </summary>
    public void ToggleChordPanel()
    {
        if (chordPanel != null)
        {
            chordPanel.SetActive(!chordPanel.activeSelf);
        }
    }
}