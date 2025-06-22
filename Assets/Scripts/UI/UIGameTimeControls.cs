using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameTimeControls : MonoBehaviour
{
    [Header("Play/Pause Button")]
    public Button playPauseButton;
    public Image playPauseIcon;
    public Sprite playSprite;
    public Sprite pauseSprite;

    [Header("Speed Buttons")]
    public Button speed1Button;
    public Button speed2Button;
    public Button speed3Button;

    [Header("Speed Button Colors")]
    public Color normalButtonColor = Color.white;
    public Color selectedButtonColor = Color.green;
    public Color disabledButtonColor = Color.gray;

    [Header("Status Display")]
    public TextMeshProUGUI statusText;

    private GameTimeController timeController;

    void Start()
    {
        timeController = GameTimeController.Instance;
        if (timeController == null)
        {
            Debug.LogError("GameTimeController not found!");
            return;
        }

        SetupButtons();
        SubscribeToEvents();
        UpdateUI();
    }

    void SetupButtons()
    {
        // Play/Pause button
        if (playPauseButton != null)
        {
            playPauseButton.onClick.AddListener(() => timeController.TogglePlayPause());
        }

        // Speed buttons
        if (speed1Button != null)
        {
            speed1Button.onClick.AddListener(() => timeController.SetSpeedNormal());
        }
        if (speed2Button != null)
        {
            speed2Button.onClick.AddListener(() => timeController.SetSpeedFast());
        }
        if (speed3Button != null)
        {
            speed3Button.onClick.AddListener(() => timeController.SetSpeedVeryFast());
        }
    }

    void SubscribeToEvents()
    {
        timeController.OnSpeedChanged += OnSpeedChanged;
        timeController.OnPlayPauseChanged += OnPlayPauseChanged;
    }

    void OnDestroy()
    {
        if (timeController != null)
        {
            timeController.OnSpeedChanged -= OnSpeedChanged;
            timeController.OnPlayPauseChanged -= OnPlayPauseChanged;
        }
    }

    void OnSpeedChanged(GameTimeController.GameSpeed newSpeed)
    {
        UpdateUI();
    }

    void OnPlayPauseChanged(bool isPlaying)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        UpdatePlayPauseButton();
        UpdateSpeedButtons();
        UpdateStatusText();
    }

    void UpdatePlayPauseButton()
    {
        if (playPauseButton == null || playPauseIcon == null) return;

        bool isPlaying = timeController.IsPlaying;

        // Update icon
        playPauseIcon.sprite = isPlaying ? pauseSprite : playSprite;

        // Update button interactability
        playPauseButton.interactable = true;
    }

    void UpdateSpeedButtons()
    {
        var currentSpeed = timeController.CurrentSpeed;
        bool isPlaying = timeController.IsPlaying;

        // Update speed1 button
        if (speed1Button != null)
        {
            var colors = speed1Button.colors;
            colors.normalColor = (currentSpeed == GameTimeController.GameSpeed.Normal) ? selectedButtonColor : normalButtonColor;
            speed1Button.colors = colors;
            speed1Button.interactable = true;
        }

        // Update speed2 button
        if (speed2Button != null)
        {
            var colors = speed2Button.colors;
            colors.normalColor = (currentSpeed == GameTimeController.GameSpeed.Fast) ? selectedButtonColor : normalButtonColor;
            speed2Button.colors = colors;
            speed2Button.interactable = true;
        }

        // Update speed3 button
        if (speed3Button != null)
        {
            var colors = speed3Button.colors;
            colors.normalColor = (currentSpeed == GameTimeController.GameSpeed.VeryFast) ? selectedButtonColor : normalButtonColor;
            speed3Button.colors = colors;
            speed3Button.interactable = true;
        }
    }

    void UpdateStatusText()
    {
        if (statusText == null) return;

        string status = "";
        if (!timeController.IsPlaying)
        {
            status = "PAUSÉ";
        }
        else
        {
            switch (timeController.CurrentSpeed)
            {
                case GameTimeController.GameSpeed.Normal:
                    status = "x1";
                    break;
                case GameTimeController.GameSpeed.Fast:
                    status = "x2";
                    break;
                case GameTimeController.GameSpeed.VeryFast:
                    status = "x3";
                    break;
            }
        }

        statusText.text = status;
    }
}