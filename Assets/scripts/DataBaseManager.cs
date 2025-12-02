using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public TMP_InputField emailinput;
    public TMP_InputField passwordinput;

    public TMP_Text errorText;

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

                var uid = task.Result.User.UserId;
                Debug.Log($"User signed up successfully: {uid}");
            }
        });
    }

    public void SignIn()
    {
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

                        case AuthError.WeakPassword:
                            errorText.text = "Please enter a password 6 characters or longer!";
                            break;

                        case AuthError.EmailAlreadyInUse:
                            errorText.text = "The email address is already in use by another account!";
                            break;

                        case AuthError.InvalidEmail:
                            errorText.text = "The email address is invalid!";
                            break;

                        case AuthError.WrongPassword:
                            errorText.text = "The paddword is incorrect!";
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
                Debug.Log("Can't sign in due to error!!!");
                return;
            }

            if (task.IsCompletedSuccessfully)
            {
                errorText.text = "User successfully signed in!";

                var uid = task.Result.User.UserId;
                Debug.Log($"User signed in successfully: {uid}");
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
