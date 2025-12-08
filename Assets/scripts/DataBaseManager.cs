using System;
using System.Collections.Generic;
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

    public static string currentUser;

    public int highScore = 0;

    public void SignUp()
    {
        errorText.text = "";

        var createTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(emailinput.text, passwordinput.text);
        createTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                var baseException = task.Exception.GetBaseException();

                if (baseException is FirebaseException)
                {
                    var firebaseException = baseException as FirebaseException;
                    var errorCode = (AuthError)firebaseException.ErrorCode;

                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            errorText.text = "Please enter an email address!";
                            break;

                        case AuthError.MissingPassword:
                            errorText.text = "Please enter a password!";
                            break;

                        case AuthError.WeakPassword:
                            errorText.text = "Please enter a password 6 characters or longer!";
                            break;

                        case AuthError.EmailAlreadyInUse:
                            errorText.text = "The email address is already in use by another account!";
                            break;

                        case AuthError.InvalidEmail:
                            errorText.text = "The email address is invalid!";
                            break;

                        default:
                            errorText.text = $"Unknown Firebase exception: {errorCode}";
                            break;

                    }
                    return;
                }

                errorText.text = $"Unknown exception when signing up: {baseException.Message}";
                return;
            }

            if (task.IsCanceled)
            {
                errorText.text = "User creation cancelled!";
                return;
            }

            if (task.IsCompletedSuccessfully)
            {
                errorText.text = "User created successfully, please sign in!";

                FirebaseUser newUser = task.Result.User;
                string uid = newUser.UserId;
                string email = newUser.Email;

                currentUser = email;
                
                mDatabaseRef.Child("users").Child(uid).Child("email").SetValueAsync(email);
                mDatabaseRef.Child("users").Child(uid).Child("highscore").SetValueAsync(highScore);

                Debug.Log($"User signed up successfully: {uid}");

                SignupCanvas.SetActive(false);
            }
        });
    }

    public void SignIn()
    {
        errorText.text = "";

        var signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(emailinput.text, passwordinput.text);
        signInTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                var baseException = task.Exception.GetBaseException();

                if (baseException is FirebaseException)
                {
                    var firebaseException = baseException as FirebaseException;
                    var errorCode = (AuthError)firebaseException.ErrorCode;

                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            errorText.text = "Please enter an email address!";
                            break;

                        case AuthError.MissingPassword:
                            errorText.text = "Please enter a password!";
                            break;

                        case AuthError.InvalidEmail:
                            errorText.text = "The email address is invalid!";
                            break;

                        case AuthError.UserNotFound:
                            errorText.text = "The email address is not found!";
                            break;

                        case AuthError.WrongPassword:
                            errorText.text = "The password is incorrect!";
                            break;

                        default:
                            errorText.text = $"Unknown Firebase exception: {errorCode}";
                            break;

                    }
                    return;
                }

                errorText.text = $"Unknown exception when signing in: {baseException.Message}";
                Debug.LogError(errorText.text);
                return;
            }

            if (task.IsCanceled)
            {
                Debug.Log("Can't sign in due to error!!!");
                return;
            }

            if (task.IsCompletedSuccessfully)
            {
                errorText.text = "User successfully signed in!";

                FirebaseUser user = task.Result.User;
                string uid = user.UserId;
                string email = user.Email;

                currentUser = email;

                Debug.Log($"User signed in successfully: {uid}");
                SignupCanvas.SetActive(false);
            }
        });

        Debug.Log("HAHAHAHHA");
    }
    DatabaseReference mDatabaseRef;
    void Start()
    {

        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

    }


    // Update is called once per frame
    void Update()
    {

    }
}
