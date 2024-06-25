using System.Collections;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    public int currentTime { get; private set; }

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
        }
    }
}
