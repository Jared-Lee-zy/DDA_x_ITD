using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizFlowManager : MonoBehaviour
{
    public GameObject[] questionPanels; // size = 7
    public GameObject resultPanel;

    int currentQuestion = 0;
    int score = 0;

    public TextMeshProUGUI resultText;

    [Header("Sound Question Settings (Q4 & Q5)")]
    public AudioSource audioSource;           
    public AudioClip[] previewClips;        

    public string correctOpenAnswer;   

    public GameObject quizCanvas;
public GameObject startPanel;   // Optional: the panel with the “Start Quiz” button

// Called when user presses Start Quiz button
public void StartQuiz()
{
    if (startPanel != null)
        startPanel.SetActive(false);

    quizCanvas.SetActive(true);
    ShowQuestion(0);
}

// Called when user presses Exit Quiz button
public void ExitQuiz()
{
    quizCanvas.SetActive(false);

    if (startPanel != null)
        startPanel.SetActive(true);

    // Reset score + question index if quiz restarts later
    currentQuestion = 0;
    score = 0;
}



    void Start()
    {
        ShowQuestion(0);
    }

    public void ShowQuestion(int index)
    {
        // Hide everything
        foreach (var p in questionPanels)
            p.SetActive(false);

        resultPanel.SetActive(false);

        // Show the target question
        questionPanels[index].SetActive(true);
        currentQuestion = index;
    }

    // Called by MCQ buttons for Q1–5 (still works the same)
    public void AnswerMCQ(bool isCorrect)
    {
        GameObject qp = questionPanels[currentQuestion];
        TextMeshProUGUI feedback = qp.transform.Find("FeedbackText").GetComponent<TextMeshProUGUI>();

        if (isCorrect)
        {
            feedback.text = "Correct!";
            score++;
        }
        else
        {
            feedback.text = "Wrong!";
        }

        Invoke(nameof(NextQuestion), 1.5f);
    }

    // Called by the open-answer questions (Q6–7)
public void SubmitOpenAnswer(TMP_InputField inputField)
{
    string userAnswer = inputField.text.Trim().ToLower();
    string expected = correctOpenAnswer.Trim().ToLower();

    GameObject qp = questionPanels[currentQuestion];
    TextMeshProUGUI feedback = qp.transform.Find("FeedbackText").GetComponent<TextMeshProUGUI>();

    if (userAnswer == expected)
    {
        feedback.text = "Correct!";
        score++;
    }
    else
    {
        feedback.text = "Wrong!";
    }

    Invoke(nameof(NextQuestion), 1.5f);
}

    // --------------------------------------------------
    // NEW: Plays one of the 4 sound preview buttons
    // --------------------------------------------------
    public void PlayPreviewSound(int index)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not assigned in Inspector!");
            return;
        }

        if (previewClips == null || previewClips.Length == 0)
        {
            Debug.LogWarning("Preview clips array is empty!");
            return;
        }

        if (index < 0 || index >= previewClips.Length)
        {
            Debug.LogWarning("PreviewSound index is out of range");
            return;
        }

        audioSource.Stop();
        audioSource.clip = previewClips[index];
        audioSource.Play();
    }
    // --------------------------------------------------

    void NextQuestion()
    {
        currentQuestion++;

        if (currentQuestion >= questionPanels.Length)
        {
            EndQuiz();
        }
        else
        {
            ShowQuestion(currentQuestion);
        }
    }

    void EndQuiz()
    {
        foreach (var p in questionPanels)
            p.SetActive(false);

        resultPanel.SetActive(true);
        resultText.text = "Your Score: " + score + " / " + questionPanels.Length;
    }
}

