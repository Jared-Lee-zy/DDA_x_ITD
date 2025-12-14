///Made By Lim Xue Zhi Conan
/// Date Of Creation  : 2025-12-11
/// Function of Script: Manages the piano quiz flow, including question navigation, scoring, timing, audio feedback, and Firebase best-time tracking.
/// </summary>
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;

/// <summary>
/// Manages the flow, scoring, timing, sounds, and Firebase logic
/// for the Piano Quiz in Chordify.
/// </summary>
public class PianoQuizFlowManager : MonoBehaviour
{
    [Header("Quiz Panels")]
    /// <summary>All question panels in order.</summary>
    public GameObject[] questionPanels;

    /// <summary>Result panel shown at the end of the quiz.</summary>
    public GameObject resultPanel;

    int currentQuestion = 0;
    int score = 0;

    float quizStartTime;
    float finalTime;
    float bestFinalTime;
    bool timerRunning = false;

    [Header("UI References")]
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;

    [Header("Firebase")]
    public DatabaseReference mDatabaseRef;
    private FirebaseAuth auth;

    [Header("Sound Question Settings (Q4 & Q5)")]
    /// <summary>AudioSource used for chord preview questions.</summary>
    public AudioSource audioSource;

    /// <summary>Audio clips used for preview questions.</summary>
    public AudioClip[] previewClips;

    [Header("Quiz Sound Effects")]
    /// <summary>Plays when the user answers correctly.</summary>
    public AudioClip correctSFX;

    /// <summary>Plays when the user answers wrongly.</summary>
    public AudioClip wrongSFX;

    /// <summary>Plays when the quiz is completed.</summary>
    public AudioClip quizCompleteSFX;

    /// <summary>Looping background music during the quiz.</summary>
    public AudioClip quizBGM;

    /// <summary>AudioSource dedicated to sound effects.</summary>
    public AudioSource sfxSource;

    /// <summary>AudioSource dedicated to background music.</summary>
    public AudioSource bgmSource;

    [Header("Answer Settings")]
    /// <summary>Expected answer for open-ended questions.</summary>
    public string correctOpenAnswer;

    [Header("Canvas References")]
    public GameObject quizCanvas;
    public GameObject startPanel;

    /// <summary>
    /// Starts the quiz, enables quiz canvas, and begins timer and BGM.
    /// </summary>
    public void StartQuiz()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        quizCanvas.SetActive(true);
        ShowQuestion(0);
        ///summary> Shuffles the order of quiz questions randomly. </summary>
        ShuffleQuestions();
        currentQuestion = 0;

        quizStartTime = Time.time;
        timerRunning = true;

        // Start background music
        if (bgmSource != null && quizBGM != null)
        {
            bgmSource.clip = quizBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// Updates the timer text.
    /// </summary>
    public void UpdateText(string newText)
    {
        if (timerText != null)
            timerText.text = newText;
        else
            Debug.LogWarning("Cannot update text — reference is missing.");
    }

    /// <summary>
    /// Exits the quiz and resets values.
    /// </summary>
    public void ExitQuiz()
    {
        quizCanvas.SetActive(false);

        if (startPanel != null)
            startPanel.SetActive(true);

        currentQuestion = 0;
        score = 0;

        if (bgmSource != null)
            bgmSource.Stop();
    }

    void Start()
    {
        ShowQuestion(0);
        auth = FirebaseAuth.DefaultInstance;
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

     /// <summary>
    /// Randomly shuffles the order of quiz questions.
    void ShuffleQuestions()
    {
    for (int i = 0; i < questionPanels.Length; i++)
    {
        int randomIndex = Random.Range(i, questionPanels.Length);

        GameObject temp = questionPanels[i];
        questionPanels[i] = questionPanels[randomIndex];
        questionPanels[randomIndex] = temp;
    }
    }

    /// <summary>
    /// Shows a specific question panel.
    /// </summary>
    public void ShowQuestion(int index)
    {
        foreach (var p in questionPanels)
            p.SetActive(false);

        resultPanel.SetActive(false);
        questionPanels[index].SetActive(true);
        currentQuestion = index;
    }

    /// <summary>
    /// Handles multiple-choice question answers.
    /// </summary>
    public void AnswerMCQ(bool isCorrect)
    {
        GameObject qp = questionPanels[currentQuestion];
        TextMeshProUGUI feedback =
            qp.transform.Find("FeedbackText").GetComponent<TextMeshProUGUI>();

        if (isCorrect)
        {
            feedback.text = "Correct!";
            score++;

            if (sfxSource && correctSFX)
                sfxSource.PlayOneShot(correctSFX);

            Invoke(nameof(NextQuestion), 1.5f);
        }
        else
        {
            feedback.text = "Wrong! Try again.";

            if (sfxSource && wrongSFX)
                sfxSource.PlayOneShot(wrongSFX);
        }
    }

    /// <summary>
    /// Handles open-ended question submission.
    /// </summary>
    public void SubmitOpenAnswer(TMP_InputField inputField)
    {
        string userAnswer = inputField.text.Trim().ToLower();
        string expected = correctOpenAnswer.Trim().ToLower();

        GameObject qp = questionPanels[currentQuestion];
        TextMeshProUGUI feedback =
            qp.transform.Find("FeedbackText").GetComponent<TextMeshProUGUI>();

        if (userAnswer == expected)
        {
            feedback.text = "Correct!";
            score++;

            if (sfxSource && correctSFX)
                sfxSource.PlayOneShot(correctSFX);

            Invoke(nameof(NextQuestion), 1.5f);
        }
        else
        {
            feedback.text = "Wrong! Try again.";

            if (sfxSource && wrongSFX)
                sfxSource.PlayOneShot(wrongSFX);
        }
    }

    /// <summary>
    /// Plays a preview sound for sound-based questions.
    /// </summary>
    public void PlayPreviewSound(int index)
    {
        if (audioSource == null || previewClips == null || previewClips.Length == 0)
            return;

        if (index < 0 || index >= previewClips.Length)
            return;

        audioSource.Stop();
        audioSource.clip = previewClips[index];
        audioSource.Play();
    }

    void NextQuestion()
    {
        currentQuestion++;

        if (currentQuestion >= questionPanels.Length)
            EndQuiz();
        else
            ShowQuestion(currentQuestion);
    }

    /// <summary>
    /// Ends the quiz, shows results, plays completion sound, and updates Firebase.
    /// </summary>
    async void EndQuiz()
    {
        timerRunning = false;
        finalTime = Mathf.Round((Time.time - quizStartTime) * 100f) / 100f;

        foreach (var p in questionPanels)
            p.SetActive(false);

        resultPanel.SetActive(true);

        resultText.text = "Your Score: " + score + " / " + questionPanels.Length;
        timerText.text = "Time: " + finalTime + " seconds";

        if (bgmSource != null)
            bgmSource.Stop();

        if (sfxSource && quizCompleteSFX)
            sfxSource.PlayOneShot(quizCompleteSFX);

        await UpdateBestTime(finalTime);
        FetchCurrentBestTime();

        foreach (var p in questionPanels)
        p.SetActive(false);

    }

    /// <summary>
    /// Updates the displayed best time.
    /// </summary>
    public void UpdateBestFinalTime(float newBestTime)
    {
        bestFinalTime = newBestTime;
        bestTimeText.text = "Best Time: " + bestFinalTime + " seconds";
    }

    /// <summary>
    /// Updates the user's best piano quiz time in Firebase.
    /// </summary>
    public async Task<bool> UpdateBestTime(float newTime)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) return false;

        newTime = Mathf.Round(newTime * 100f) / 100f;

        try
        {
            var snapshot = await mDatabaseRef
                .Child("users")
                .Child(user.UserId)
                .GetValueAsync();

            player existingPlayer =
                JsonUtility.FromJson<player>(snapshot.GetRawJsonValue());

            if (existingPlayer == null) return false;

            if (existingPlayer.pianoBesttime == 0 || newTime < existingPlayer.pianoBesttime)
            {
                existingPlayer.pianoBesttime = newTime;
                await mDatabaseRef
                    .Child("users")
                    .Child(user.UserId)
                    .SetRawJsonValueAsync(JsonUtility.ToJson(existingPlayer));
                return true;
            }
        }
        catch { }

        return false;
    }

    /// <summary>
    /// Fetches and displays the current best piano quiz time.
    /// </summary>
    public void FetchCurrentBestTime()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) return;

        mDatabaseRef.Child("users").Child(user.UserId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted) return;

                player data =
                    JsonUtility.FromJson<player>(task.Result.GetRawJsonValue());

                bestFinalTime = data.pianoBesttime;
                UpdateBestFinalTime(bestFinalTime);
            });
    }
}
