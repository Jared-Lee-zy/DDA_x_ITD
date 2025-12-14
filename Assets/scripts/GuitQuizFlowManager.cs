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
///Made By Lim Xue Zhi Conan and Jared Lee Zhengyu
/// Date Of Creation  : 09/12/2025
/// Function of Script: Manages the guitar quiz flow, including question navigation, scoring, timing, audio feedback, and Firebase best-time tracking.
/// </summary>
public class GuitQuizFlowManager : MonoBehaviour
{
    /// <summary>
    /// Array of quiz question panels.
    /// </summary>
    public GameObject[] questionPanels;

    /// <summary>
    /// Panel shown when the quiz ends.
    /// </summary>
    public GameObject resultPanel;

    /// <summary>
    /// Index of the current question.
    /// </summary>
    int currentQuestion = 0;

    /// <summary>
    /// User's quiz score.
    /// </summary>
    int score = 0;

    /// <summary>
    /// Time when the quiz started.
    /// </summary>
    float quizStartTime;

    /// <summary>
    /// Final time taken to complete the quiz.
    /// </summary>
    float finalTime;

    /// <summary>
    /// Best recorded completion time.
    /// </summary>
    float bestFinalTime;

    /// <summary>
    /// Indicates whether the quiz timer is running.
    /// </summary>
    bool timerRunning = false;

    /// <summary>
    /// Displays the user's best completion time.
    /// </summary>
    public TextMeshProUGUI bestTimeText;

    /// <summary>
    /// Displays the current or final timer value.
    /// </summary>
    public TextMeshProUGUI timerText;

    /// <summary>
    /// Firebase database reference.
    /// </summary>
    public DatabaseReference mDatabaseRef;

    /// <summary>
    /// Firebase authentication instance.
    /// </summary>
    private FirebaseAuth auth;

    /// <summary>
    /// Displays the final quiz score.
    /// </summary>
    public TextMeshProUGUI resultText;

    /// <summary>
    /// Audio source used for chord preview sounds.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Audio clips used for chord preview buttons.
    /// </summary>
    public AudioClip[] previewClips;

    /// <summary>
    /// Audio source used for feedback and UI sounds.
    /// </summary>
    public AudioSource sfxSource;

    /// <summary>
    /// Sound played when the user answers correctly.
    /// </summary>
    public AudioClip correctSound;

    /// <summary>
    /// Sound played when the user answers incorrectly.
    /// </summary>
    public AudioClip wrongSound;

    /// <summary>
    /// Sound played when the quiz is completed.
    /// </summary>
    public AudioClip quizCompleteSound;

    /// <summary>
    /// Audio source used for quiz background music.
    /// </summary>
    public AudioSource bgmSource;

    /// <summary>
    /// Background music played during the quiz.
    /// </summary>
    public AudioClip quizBGM;

    /// <summary>
    /// Correct answer string for open-ended questions.
    /// </summary>
    public string correctOpenAnswer;

    /// <summary>
    /// Canvas containing the quiz UI.
    /// </summary>
    public GameObject quizCanvas;

    /// <summary>
    /// Panel shown before starting the quiz.
    /// </summary>
    public GameObject startPanel;

    /// <summary>
    /// Starts the quiz, initializes timer, and plays background music.
    /// </summary>
    public void StartQuiz()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        ///summary> Shuffles the order of quiz questions randomly. </summary>
        ShuffleQuestions();
        currentQuestion = 0;

        quizCanvas.SetActive(true);
        ShowQuestion(0);

        quizStartTime = Time.time;
        timerRunning = true;

        PlayBGM();
    }

    /// <summary>
    /// Updates the timer text UI.
    /// </summary>
    public void UpdateText(string newText)
    {
        if (timerText != null)
            timerText.text = newText.ToString();
    }

    /// <summary>
    /// Exits the quiz and resets state.
    /// </summary>
    public void ExitQuiz()
    {
        StopBGM();
        quizCanvas.SetActive(false);

        if (startPanel != null)
            startPanel.SetActive(true);

        currentQuestion = 0;
        score = 0;
    }

    /// <summary>
    /// Initializes Firebase and shows the first question.
    /// </summary>
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
    /// Displays a specific question panel.
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
        TextMeshProUGUI feedback = qp.transform.Find("FeedbackText").GetComponent<TextMeshProUGUI>();

        if (isCorrect)
        {
            feedback.text = "Correct!";
            score++;
            PlaySFX(correctSound);
            Invoke(nameof(NextQuestion), 1.5f);
        }
        else
        {
            feedback.text = "Wrong! Try again.";
            PlaySFX(wrongSound);
        }
    }

    /// <summary>
    /// Handles open-ended question submissions.
    /// </summary>
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
            PlaySFX(correctSound);
            Invoke(nameof(NextQuestion), 1.5f);
        }
        else
        {
            feedback.text = "Wrong! Try again.";
            PlaySFX(wrongSound);
        }
    }

    /// <summary>
    /// Plays a chord preview sound.
    /// </summary>
    public void PlayPreviewSound(int index)
    {
        if (audioSource == null || previewClips == null || index < 0 || index >= previewClips.Length)
            return;

        audioSource.Stop();
        audioSource.clip = previewClips[index];
        audioSource.Play();
    }

    /// <summary>
    /// Advances to the next question or ends the quiz.
    /// </summary>
    void NextQuestion()
    {
        currentQuestion++;

        if (currentQuestion >= questionPanels.Length)
            EndQuiz();
        else
            ShowQuestion(currentQuestion);
    }

    /// <summary>
    /// Ends the quiz session, stops the timer, calculates the final completion time,
    /// displays the result screen, and updates the user's best time in Firebase.
    /// This method is asynchronous because it awaits the completion of the best time update operation.
    /// </summary>
    async void EndQuiz()
    {
        timerRunning = false;
        finalTime = Mathf.Round((Time.time - quizStartTime) * 100f) / 100f;
        /// Hide all question panels.
        foreach (var p in questionPanels)
            p.SetActive(false);

        StopBGM();
        PlaySFX(quizCompleteSound);
        /// Show result panel with score and time.
        resultPanel.SetActive(true);
        resultText.text = "Your Score: " + score + " / " + questionPanels.Length;
        timerText.text = "Time: " + finalTime + " seconds";
        /// Update best time in Firebase if applicable.
        await UpdateBestTime(finalTime);
        /// Fetch the current best time.
        FetchCurrentBestTime();
    }

    /// <summary>
    /// Updates the best final time display for the local UI element.
    /// </summary>
    public void UpdateBestFinalTime(float newBestTime)
    {
        bestFinalTime = newBestTime;
        bestTimeText.text = "Best Time: " + bestFinalTime + " seconds";
    }

    /// <summary>
    /// Compares the new completion time with the stored best time in Firebase and updates if necessary.
    /// </summary>
    public async Task<bool> UpdateBestTime(float newTime)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
            return false;

        newTime = Mathf.Round(newTime * 100f) / 100f;
        /// Retrieve existing player data from Firebase.
        var snapshot = await mDatabaseRef.Child("users").Child(user.UserId).GetValueAsync();
        player existingPlayer = JsonUtility.FromJson<player>(snapshot.GetRawJsonValue());
        /// Update best guitar time if the new time is better.
        if (existingPlayer != null && (existingPlayer.guitBesttime == 0 || newTime < existingPlayer.guitBesttime))
        {
            existingPlayer.guitBesttime = newTime;
            await mDatabaseRef.Child("users").Child(user.UserId)
                .SetRawJsonValueAsync(JsonUtility.ToJson(existingPlayer));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Fetches the user's best guitar time from Firebase.
    /// </summary>
    public void FetchCurrentBestTime()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
            return;

        mDatabaseRef.Child("users").Child(user.UserId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    player objective = JsonUtility.FromJson<player>(task.Result.GetRawJsonValue());
                    UpdateBestFinalTime(objective.guitBesttime);
                }
            });
    }

    /// <summary>
    /// Plays a one-shot sound effect.
    /// </summary>
    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Starts quiz background music.
    /// </summary>
    void PlayBGM()
    {
        if (bgmSource != null && quizBGM != null)
        {
            bgmSource.clip = quizBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// Stops quiz background music.
    /// </summary>
    void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
}
