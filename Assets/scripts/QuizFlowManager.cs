using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;

public class QuizFlowManager : MonoBehaviour
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

    private IEnumerator Updatedelay()
    {
        yield return new WaitForSeconds(2f);
    }
    void EndQuiz()
    {
        timerRunning = false;
        finalTime = Time.time - quizStartTime;

        foreach (var p in questionPanels)
            p.SetActive(false);


        resultPanel.SetActive(true);
        UpdateBestTime(finalTime);
        StartCoroutine(Updatedelay());

        resultText.text = "Your Score: " + score + " / " + questionPanels.Length;
        finalTime = Mathf.Round(finalTime * 100f) / 100f;
        timerText.text = "Time: " + finalTime + " seconds";
        FetchCurrentBestTime();
    }


    public void UpdateBestFinalTime(float newBestTime)
    {
        bestFinalTime = newBestTime;
        bestTimeText.text = "Best Time: " + bestFinalTime + " seconds";
    }

    public async void UpdateBestTime(float newTime)
    {
        string uid = DataBaseManager.currentUser;
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (string.IsNullOrEmpty(user.UserId))
        {
            Debug.Log("No UID found — user not logged in.");
            return;
        }

        newTime = Mathf.Round(newTime * 100f) / 100f; // round to 2 decimals

        try
        { 
            var snapshotReference = mDatabaseRef.Child("users").Child(user.UserId).GetValueAsync();
            await snapshotReference.ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    return;
                }

                if (task.IsCompleted)
                {
                    string playerData = task.Result.GetRawJsonValue();
                    player existingPlayer = JsonUtility.FromJson<player>(playerData);
                   
                    if (existingPlayer != null)
                    {
                        float existingBestTime = existingPlayer.guitBesttime;
                        if (newTime < existingBestTime)
                        {
                            
                            // Update best time in the player object
                            existingPlayer.guitBesttime = newTime;

                            // Convert back to JSON
                            string updatedJson = JsonUtility.ToJson(existingPlayer);

                            // Update the database
                            mDatabaseRef.Child("users").Child(user.UserId).SetRawJsonValueAsync(updatedJson);
                            Debug.Log("Best time updated to: " + newTime);
                        }
                        else
                        {
                            Debug.Log("New time is not better than existing best time.");
                        }
                    }
                }
            });
        }catch (System.Exception e)
        {
            Debug.LogError("Exception while updating best time: " + e.Message);
        }
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

                    bestFinalTime = objective.guitBesttime;

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

