using System.Collections;
using UnityEngine;
using TMPro;
/* 
Ce script gère l'affichage des informations dans l'interface utilisateur 
du jeu. Il permet de mettre à jour dynamiquement le nom du système 
et le nombre d'unités associées à une étoile sélectionnée via la méthode 
UpdateSystemInfo. De plus, il affiche un timer en continu grâce à la 
coroutine UpdateTimer, qui met à jour l'affichage chaque seconde avec le 
temps écoulé fourni par le GameTimer. Ce type de script est crucial pour informer
le joueur des états actuels et des informations pertinentes pendant le jeu.
*/

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI systemNameText;
    public TextMeshProUGUI unitsText;
    public TextMeshProUGUI timerText;

    private bool timerStarted = false;
    private Star lastDisplayedStar;
    private int lastDisplayedUnits = -1;
    private int lastDisplayedTime = -1;

    void Start()
    {
        // Ne pas démarrer automatiquement, attendre que le jeu soit lancé
        if (timerText != null)
        {
            timerText.text = "Time: 0s";
        }

        if (GameTimeController.Instance != null)
        {
            GameTimeController.Instance.OnPlayPauseChanged += OnPlayPauseChanged;
        }

        // Vérifier si le jeu est déjà en cours au démarrage de l'UI
        if (GameTimeController.Instance != null && GameTimeController.Instance.IsPlaying)
        {
            OnPlayPauseChanged(true);
        }

        // Start a coroutine to handle UI updates efficiently.
        StartCoroutine(UpdateUI());
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        // Se désabonner des événements
        if (GameTimeController.Instance != null)
        {
            GameTimeController.Instance.OnPlayPauseChanged -= OnPlayPauseChanged;
        }
    }

    void OnPlayPauseChanged(bool isPlaying)
    {
        if (isPlaying && !timerStarted)
        {
            timerStarted = true;
            // The UpdateUI coroutine now handles the timer display.
        }
    }

    public void SetDisplayedStar(Star newStar)
    {
        lastDisplayedStar = newStar;
        // Force an immediate update for the new star system name
        if (newStar != null)
        {
            systemNameText.text = "System: " + newStar.starName;
            unitsText.text = "Units: " + newStar.units;
            lastDisplayedUnits = newStar.units;
        }
        else
        {
            systemNameText.text = "System: N/A";
            unitsText.text = "Units: N/A";
            lastDisplayedUnits = -1;
        }
    }

    private IEnumerator UpdateUI()
    {
        while (true)
        {
            // Update star info only if values changed
            if (lastDisplayedStar != null && lastDisplayedStar.units != lastDisplayedUnits)
            {
                unitsText.text = "Units: " + lastDisplayedStar.units;
                lastDisplayedUnits = lastDisplayedStar.units;
            }

            // Update timer only if value changed
            if (timerStarted && GameTimer.Instance != null && GameTimer.Instance.currentTime != lastDisplayedTime)
            {
                lastDisplayedTime = GameTimer.Instance.currentTime;
                timerText.text = "Time: " + lastDisplayedTime + "s";
            }

            yield return new WaitForSeconds(0.1f); // Check for updates 10 times per second
        }
    }
}
