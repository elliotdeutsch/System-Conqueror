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
        StartCoroutine(IncrementTimer());
    }

    IEnumerator IncrementTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            currentTime++;

            if (currentTime % 5 == 0)
            {
                OnFiveSecondInterval?.Invoke();
            }
        }
    }
}
