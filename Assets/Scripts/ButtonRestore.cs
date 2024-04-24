using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRestore : MonoBehaviour
{
    [SerializeField]
    private Button _restoreButton;


    [SerializeField]
    private TMP_InputField _email;

    private DatabaseReference mDatabaseRef;

    void Reset()
    {
        _restoreButton = GetComponent<Button>();
        _email = GameObject.Find("inputEmail").GetComponent<TMP_InputField>();

    }
    // Start is called before the first frame update
    private void Start()
    {
        _restoreButton.onClick.AddListener(HandleResetButtonClicked);
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void HandleResetButtonClicked()
    {
        Debug.Log("HandleResetButtonClicked");
        string email = _email.text;

        StartCoroutine(ResetPassword(email));
    }

    private IEnumerator ResetPassword(string email)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var resetTask = auth.SendPasswordResetEmailAsync(email);

        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.IsCanceled)
        {
            Debug.LogError("SendPasswordResetEmailAsync was canceled");
        }
        else if (resetTask.IsFaulted)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + resetTask.Exception);
        }
        else
        {
            Debug.Log("Correo de restablecimiento de contraseña enviado correctamente a: " + email);

            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                auth.SendPasswordResetEmailAsync(email).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Debug.Log("Password reset email sent successfully.");
                });
            }
        }
    }
}