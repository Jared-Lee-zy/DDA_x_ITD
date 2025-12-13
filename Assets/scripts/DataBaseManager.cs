using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public TMP_InputField emailinput;
    public TMP_InputField passwordinput;

    public TMP_Text errorText;

    public GameObject SignupCanvas;

    public static string currentUser;  // NOW STORES UID

    private float guitBestTime;
    private float pianoBestTime;

    private DatabaseReference mDatabaseRef;

    void Start()
    {
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SignUp()
    {
        errorText.text = "";

        var createTask = FirebaseAuth.DefaultInstance
            .CreateUserWithEmailAndPasswordAsync(emailinput.text, passwordinput.text);

        void CreatePlayerDetails(string uid, string email, float guitBesttime, float pianoBesttime)
            {
                player playerinformation = new player(email, guitBesttime, pianoBesttime);

                string json = JsonUtility.ToJson(playerinformation);

                mDatabaseRef.Child("users").Child(uid).SetRawJsonValueAsync(json);
            }

        createTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                HandleAuthErrors(task.Exception);
                return;
            }

            if (task.IsCanceled)
            {
                errorText.text = "User creation cancelled!";
                return;
            }

            if (task.IsCompletedSuccessfully)
            {

                // Save UID globally
                currentUser = task.Result.User.UserId;

                CreatePlayerDetails(task.Result.User.UserId, emailinput.text, 9999f, 9999f);
                errorText.text = "User created successfully!";
                SignupCanvas.SetActive(false);

            }
        });
    }

    public void SignIn()
    {
        errorText.text = "";

        var signInTask = FirebaseAuth.DefaultInstance
            .SignInWithEmailAndPasswordAsync(emailinput.text, passwordinput.text);

        signInTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                HandleAuthErrors(task.Exception);
                return;
            }

            if (task.IsCanceled)
            {
                errorText.text = "Sign in cancelled!";
                return;
            }

            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                mDatabaseRef.Child("users").Child(user.UserId).Child("guitBesttime").GetValueAsync().ContinueWithOnMainThread(t =>
                {
                    if (t.IsCompleted && t.Result.Exists)
                    {
                        float.TryParse(t.Result.Value.ToString(), out guitBestTime);
                        Debug.Log("Loaded best time: " + guitBestTime);
                    }
                });

                mDatabaseRef.Child("users").Child(user.UserId).Child("pianoBesttime").GetValueAsync().ContinueWithOnMainThread(t =>
                {
                    if (t.IsCompleted && t.Result.Exists)
                    {
                        float.TryParse(t.Result.Value.ToString(), out pianoBestTime);
                        Debug.Log("Loaded best time: " + pianoBestTime);
                    }
                });

                // Store UID globally
                currentUser = user.UserId;

                errorText.text = "Login successful!";
                SignupCanvas.SetActive(false);

                Debug.Log("User signed in: " + user.UserId);
            }
        });
    }

    private void HandleAuthErrors(AggregateException exception)
    {
        var baseException = exception.GetBaseException();

        if (baseException is FirebaseException firebaseEx)
        {
            var errorCode = (AuthError)firebaseEx.ErrorCode;

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    errorText.text = "Please enter an email!";
                    break;
                case AuthError.MissingPassword:
                    errorText.text = "Please enter a password!";
                    break;
                case AuthError.WeakPassword:
                    errorText.text = "Password must be at least 6 characters!";
                    break;
                case AuthError.EmailAlreadyInUse:
                    errorText.text = "Email already in use!";
                    break;
                case AuthError.UserNotFound:
                    errorText.text = "User not found!";
                    break;
                case AuthError.InvalidEmail:
                    errorText.text = "Invalid email address!";
                    break;
                case AuthError.WrongPassword:
                    errorText.text = "Wrong password!";
                    break;
                default:
                    errorText.text = "Username or Password incorrect!";
                    break;
            }
        }
        else
        {
            errorText.text = "Error: " + baseException.Message;
        }
    }
}
