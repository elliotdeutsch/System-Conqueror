using UnityEngine;
using System;

public class GameTimeController : MonoBehaviour
{
    public static GameTimeController Instance { get; private set; }

    public enum GameSpeed
    {
        Paused = 0,
        Normal = 1,
        Fast = 2,
        VeryFast = 3
    }

    [Header("Time Control")]
    public GameSpeed currentSpeed = GameSpeed.Normal;

    [Header("Speed Multipliers")]
    public float normalSpeed = 1f;
    public float fastSpeed = 2f;
    public float veryFastSpeed = 3f;

    // Événements pour l'UI
    public event Action<GameSpeed> OnSpeedChanged;
    public event Action<bool> OnPlayPauseChanged;

    private bool isPlaying = false;
    private bool gameStarted = false;

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
        Pause();
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        // Espace pour play/pause
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePlayPause();
        }

        // Touches 1, 2, 3 pour les vitesses
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSpeed(GameSpeed.Normal);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSpeed(GameSpeed.Fast);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSpeed(GameSpeed.VeryFast);
        }
    }

    public void TogglePlayPause()
    {
        if (isPlaying)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }

    public void Play()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            SetSpeed(currentSpeed);
            OnPlayPauseChanged?.Invoke(true);
        }
    }

    public void Pause()
    {
        isPlaying = false;
        Time.timeScale = 0f;
        OnPlayPauseChanged?.Invoke(false);
    }

    public void SetSpeed(GameSpeed speed)
    {
        if (speed == GameSpeed.Paused)
        {
            Pause();
            return;
        }

        currentSpeed = speed;
        isPlaying = true;

        switch (speed)
        {
            case GameSpeed.Normal:
                Time.timeScale = normalSpeed;
                break;
            case GameSpeed.Fast:
                Time.timeScale = fastSpeed;
                break;
            case GameSpeed.VeryFast:
                Time.timeScale = veryFastSpeed;
                break;
        }

        OnSpeedChanged?.Invoke(speed);
        OnPlayPauseChanged?.Invoke(true);
    }

    public void SetSpeedNormal() => SetSpeed(GameSpeed.Normal);
    public void SetSpeedFast() => SetSpeed(GameSpeed.Fast);
    public void SetSpeedVeryFast() => SetSpeed(GameSpeed.VeryFast);

    public bool IsPlaying => isPlaying;
    public GameSpeed CurrentSpeed => currentSpeed;

    public void StartGame()
    {
        gameStarted = true;
        Play();
    }

    public bool HasGameStarted => gameStarted;
}