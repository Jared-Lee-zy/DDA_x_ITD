///Made By Lim Xue Zhi Conan
/// Date Of Creation  : 2025-12-10
/// Function of Script: Manages the tutorial flow by showing tutorial panels one at a time.
using UnityEngine;
/// <summary>
/// Manages the tutorial flow by showing tutorial panels one at a time.
/// Allows navigation using Back and Next buttons, and exits on the final panel.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    /// <summary>
    /// All tutorial panels in order.
    /// </summary>
    public GameObject[] tutorialPanels;

    /// <summary>
    /// The tutorial canvas that contains all tutorial panels.
    /// </summary>
    public GameObject tutorialCanvas;

    /// <summary>
    /// The panel to show after the tutorial ends (e.g. start menu or AR screen).
    /// </summary>
    public GameObject mainMenuPanel;

    /// <summary>
    /// Current tutorial page index.
    /// </summary>
    private int currentIndex = 0;

    /// <summary>
    /// Initializes the tutorial by showing the first panel.
    /// </summary>
    void Start()
    {
        ShowPanel(0);
    }

    /// <summary>
    /// Shows the tutorial panel at the given index.
    /// </summary>
    /// <param name="index">Index of the tutorial panel to show</param>
    public void ShowPanel(int index)
    {
        if (index < 0 || index >= tutorialPanels.Length)
            return;

        foreach (GameObject panel in tutorialPanels)
        {
            panel.SetActive(false);
        }

        tutorialPanels[index].SetActive(true);
        currentIndex = index;
    }

    /// <summary>
    /// Moves to the next tutorial panel.
    /// </summary>
    public void NextPanel()
    {
        if (currentIndex < tutorialPanels.Length - 1)
        {
            ShowPanel(currentIndex + 1);
        }
    }

    /// <summary>
    /// Moves to the previous tutorial panel.
    /// </summary>
    public void PreviousPanel()
    {
        if (currentIndex > 0)
        {
            ShowPanel(currentIndex - 1);
        }
    }

    /// <summary>
    /// Exits the tutorial and shows the main menu or AR start screen.
    /// </summary>
    public void ExitTutorial()
    {
        tutorialCanvas.SetActive(false);

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }
}
