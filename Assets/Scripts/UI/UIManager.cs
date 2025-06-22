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
            StartCoroutine(UpdateTimer());
        }
    }

    public void UpdateSystemInfo(Star star)
    {
        systemNameText.text = "System: " + star.starName;
        unitsText.text = "Units: " + star.units;
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            // Mettre à jour le texte du timer
            if (GameTimer.Instance != null)
            {
                timerText.text = "Time: " + GameTimer.Instance.currentTime + "s";
            }
            yield return new WaitForSeconds(1);
        }
    }
}
