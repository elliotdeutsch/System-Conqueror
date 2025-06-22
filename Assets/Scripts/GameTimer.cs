using System;
using System.Collections;
using UnityEngine;

/* 
Ce script gère un minuteur global dans le jeu, incrémentant le temps 
chaque seconde. Il utilise le pattern Singleton pour s'assurer 
qu'il n'y a qu'une seule instance du minuteur et utilise des coroutines 
pour gérer l'incrémentation du temps de manière asynchrone. De plus, 
il fournit un mécanisme d'événements pour exécuter du code supplémentaire 
à des intervalles réguliers (toutes les cinq secondes).
*/

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    public int currentTime { get; private set; }
    public event Action OnFiveSecondInterval;

    private bool isRunning = false;
    private float accumulatedTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentTime = 0;
        isRunning = false; // S'assurer qu'il ne tourne pas au démarrage
        // Ne pas démarrer automatiquement, attendre l'appel explicite
    }

    public void ResetAndStartTimer()
    {
        currentTime = 0;
        accumulatedTime = 0f;
        StartTimer();
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(IncrementTimer());
        }
    }

    IEnumerator IncrementTimer()
    {
        while (true)
        {
            // Utiliser Time.deltaTime pour respecter le timeScale
            accumulatedTime += Time.deltaTime;

            // Incrémenter le timer chaque seconde réelle (pas affecté par timeScale)
            if (accumulatedTime >= 1f)
            {
                currentTime++;
                accumulatedTime -= 1f;

                if (currentTime % 5 == 0)
                {
                    OnFiveSecondInterval?.Invoke();
                }
            }

            yield return null;
        }
    }
}
