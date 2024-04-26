using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LogOutButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private string _sceneToLoad = "SampleScene";
    
    public void OnPointerClick(PointerEventData eventData)
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene(_sceneToLoad);
    }
}