using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    private float score;
    private float highscore;
    private bool isNewHighscore = false; // Flag para verificar si hay un nuevo highscore

    private DatabaseReference databaseReference;
    private FirebaseUser currentUser;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Obtener el usuario actualmente autenticado
        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

        // Cargar el puntaje máximo almacenado previamente para este usuario (si existe)
        if (currentUser != null)
        {
            LoadHighscoreFromFirebase();
        }
        else
        {
            Debug.LogWarning("No hay usuario autenticado.");
        }
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            score += 1 * Time.deltaTime;
            UpdateScoreText();

            if (score > highscore)
            {
                highscore = score;
                isNewHighscore = true; // Indicar que hay un nuevo highscore
                UpdateHighscoreText();
            }
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = ((int)score).ToString();
    }

    void UpdateHighscoreText()
    {
        highscoreText.text = "Highscore: " + ((int)highscore).ToString();
        SaveHighscoreToFirebase(highscore);
    }

    void LoadHighscoreFromFirebase()
    {
        databaseReference.Child("users").Child(currentUser.UserId).Child("highscore").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    float storedHighscore = float.Parse(snapshot.Value.ToString());
                    if (storedHighscore > highscore)
                    {
                        highscore = storedHighscore;
                        UpdateHighscoreText();
                    }
                }
            }
        });
    }

    void SaveHighscoreToFirebase(float newHighscore)
    {
        if (isNewHighscore) // Solo guardar si hay un nuevo highscore
        {
            // Obtener el puntaje almacenado en Firebase
            databaseReference.Child("users").Child(currentUser.UserId).Child("highscore").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot != null && snapshot.Exists)
                    {
                        float storedHighscore = float.Parse(snapshot.Value.ToString());
                        if (newHighscore > storedHighscore)
                        {
                            // Solo actualizar el puntaje si es mayor que el almacenado
                            databaseReference.Child("users").Child(currentUser.UserId).Child("highscore").SetValueAsync(newHighscore);
                        }
                    }
                    else
                    {
                        // Si no hay puntaje almacenado, establecer el nuevo puntaje
                        databaseReference.Child("users").Child(currentUser.UserId).Child("highscore").SetValueAsync(newHighscore);
                    }
                }
            });
        }
    }
}