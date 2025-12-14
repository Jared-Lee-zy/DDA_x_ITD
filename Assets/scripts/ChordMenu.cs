///Made By Lim Xue Zhi Conan
/// Date Of Creation  : 2025-12-04
/// Function of Script: Manages the chord menu panel visibility.
using UnityEngine;
/// <summary>
/// Manages the chord menu panel visibility.
/// </summary>
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