using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Database;

public class ButtonSignup : MonoBehaviour
{
    [SerializeField]
    private Button _registrationButton;

    private Coroutine _signupCoroutine;

    [SerializeField]
    private TMP_InputField _usernameInputField;

    private DatabaseReference mDatabaseRef;

    void Reset()
    {
        _registrationButton = GetComponent<Button>();
        _usernameInputField = GameObject.Find("inputUsername").GetComponent<TMP_InputField>();
    }
    private void Start()
    {
        _registrationButton.onClick.AddListener(HandleRegisterButtonClicked);
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void HandleRegisterButtonClicked()
    {
        Debug.Log("HandleRegisterButtonClicked");
        string email = GameObject.Find("inputEmail").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("inputPassword").GetComponent<TMP_InputField>().text;
        _signupCoroutine = StartCoroutine(RegisterUser(email, password));


    }
    private IEnumerator RegisterUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.IsCanceled)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled");
        }
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error" + registerTask.Exception);
        }
        else
        {
            AuthResult result = registerTask.Result;
            Debug.LogFormat("Firebase user created succesfully: ´{0} ({1})", result.User.DisplayName, result.User.UserId);

            mDatabaseRef.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(_usernameInputField.text);
        }

    }
}