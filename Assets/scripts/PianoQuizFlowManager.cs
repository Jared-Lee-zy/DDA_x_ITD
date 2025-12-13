using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;

public class PianoQuizFlowManager : MonoBehaviour
{
    public GameObject[] questionPanels; // size = 7
    public GameObject resultPanel;

    int currentQuestion = 0;
    int score = 0;

    float quizStartTime;
    float finalTime;
    float bestFinalTime;
    bool timerRunning = false;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI timerText;
    public DatabaseReference mDatabaseRef;
    private FirebaseAuth auth;

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

        quizStartTime = Time.time;
        timerRunning = true;

    }

    public void UpdateText(string newText)
    {
        if (timerText != null)
        {
            timerText.text = newText.ToString();
        }
        else
        {
            Debug.LogWarning("Cannot update text — reference is missing.");
        }
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
        auth = FirebaseAuth.DefaultInstance;
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;


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

            Invoke(nameof(NextQuestion), 1.5f);
        }
        else
        {
            feedback.text = "Wrong! Try again.";
        }


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

            Invoke(nameof(NextQuestion), 1.5f);
        }
        else
        {
            feedback.text = "Wrong! Try again.";
        }

    }


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

    async void EndQuiz()
    {
        timerRunning = false;
        finalTime = Time.time - quizStartTime;
        finalTime = Mathf.Round(finalTime * 100f) / 100f;

        foreach (var p in questionPanels)
            p.SetActive(false);

        resultPanel.SetActive(true);

        resultText.text = "Your Score: " + score + " / " + questionPanels.Length;
        timerText.text = "Time: " + finalTime + " seconds";

        await UpdateBestTime(finalTime);

        FetchCurrentBestTime();
    }


    public void UpdateBestFinalTime(float newBestTime)
    {
        bestFinalTime = newBestTime;
        bestTimeText.text = "Best Time: " + bestFinalTime + " seconds";
    }

    public async Task<bool> UpdateBestTime(float newTime)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.Log("No user logged in.");
            return false;
        }

        newTime = Mathf.Round(newTime * 100f) / 100f;

        try
        {
            var snapshot = await mDatabaseRef
                .Child("users")
                .Child(user.UserId)
                .GetValueAsync();

            string playerData = snapshot.GetRawJsonValue();
            player existingPlayer = JsonUtility.FromJson<player>(playerData);

            if (existingPlayer == null)
                return false;

            float existingBestTime = existingPlayer.pianoBesttime;

            if (existingBestTime == 0 || newTime < existingBestTime)
            {
                existingPlayer.pianoBesttime = newTime;
                string updatedJson = JsonUtility.ToJson(existingPlayer);

                await mDatabaseRef
                    .Child("users")
                    .Child(user.UserId)
                    .SetRawJsonValueAsync(updatedJson);

                Debug.Log("Best time updated to: " + newTime);
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("UpdateBestTime error: " + e.Message);
        }

        return false;
    }
    
    public void FetchCurrentBestTime()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.Log("User not logged in.");
            return;
        }

        try
        {
            var snapshot = mDatabaseRef.Child("users").Child(user.UserId).GetValueAsync();
            snapshot.ContinueWithOnMainThread(Task =>
            {
                if (Task.IsFaulted || Task.IsCanceled)
                {
                    Debug.Log("Unable to fetch best time.");
                }

                if (Task.IsCompleted)
                {
                    string playerData = Task.Result.GetRawJsonValue();

                    player objective = JsonUtility.FromJson<player>(playerData);

                    bestFinalTime = objective.pianoBesttime;

                    UpdateBestFinalTime(bestFinalTime);

                    Debug.Log("Fetched best time: " + bestFinalTime);
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to fetch best time: " + e.Message);
        }
    }
}

