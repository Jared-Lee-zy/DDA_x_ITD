using System; 
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro; 
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public TMP_Text errorText;

    public void SignUp()
    {
        errorText.text = "";

        var createTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(emailInput.text, passwordInput.text);
        createTask.ContinueWithOnMainThread(task =>
        {
        
        });
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
