using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
/// <summary>
/// Made by Jared Lee Zhengyu
///Date of Creation : 01/12/2025
///Function of Script: Manages user authentication and database interactions with Firebase.
///Handles user sign-up and sign-in processes, including error handling and storing user data, such as user's best quiz completeion time for Guitar and Piano.
/// </summary>
public class DataBaseManager : MonoBehaviour
{
    /// <summary>
    /// Input field for user email.
    /// </summary>
    public TMP_InputField emailinput;
    /// <summary>
    /// Input field for user password.
    /// </summary>
    public TMP_InputField passwordinput;
    /// <summary>
    /// Text field for displaying error messages.
    /// </summary>
    public TMP_Text errorText;
    /// <summary>
    /// Canvas GameObject for signup/login UI.
    /// </summary>
    public GameObject SignupCanvas;
    /// <summary>
    /// Static variable to hold the current user's UID.
    /// </summary>
    public static string currentUser;
    /// <summary>
    /// Variable to store the user's best time for Guitar quiz.
    /// </summary>
    private float guitBestTime;
    /// <summary>
    /// Variable to store the user's best time for Piano quiz.
    /// </summary>
    private float pianoBestTime;
    /// <summary>
    /// Firebase database reference.
    /// </summary>
    private DatabaseReference mDatabaseRef;
    /// <summary>
    /// Initializes the Firebase database reference.
    /// </summary>
    void Start()
    {
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    /// <summary>
    /// Creates a new user account with the provided email and password.
    /// Also initializes user data in the database with default best times.
    /// </summary>
    public void SignUp()
    {
        errorText.text = "";

        var createTask = FirebaseAuth.DefaultInstance
            .CreateUserWithEmailAndPasswordAsync(emailinput.text, passwordinput.text);

        /// <summary>
        /// Creates player details in the database for the specific user..
        /// </summary>
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
    
    /// <summary>
    /// Signs in an existing user with the provided email and password.
    /// Also retrieves and stores the user's best times from the database if successful.
    /// </summary>
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
                /// <summary>
                /// Retrieves and stores the user's best guitar time from the database.
                /// </summary>
                mDatabaseRef.Child("users").Child(user.UserId).Child("guitBesttime").GetValueAsync().ContinueWithOnMainThread(t =>
                {
                    if (t.IsCompleted && t.Result.Exists)
                    {
                        float.TryParse(t.Result.Value.ToString(), out guitBestTime);
                        Debug.Log("Loaded best time: " + guitBestTime);
                    }
                });
                /// <summary>
                /// Retrieves and stores the user's best piano time from the database.
                /// </summary>
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
    /// <summary>
    /// Handles authentication errors and displays appropriate user-friendly error messages based on error types.
    /// </summary>
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
