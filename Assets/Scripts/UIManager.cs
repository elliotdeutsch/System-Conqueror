using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI systemNameText;
    public TextMeshProUGUI unitsText;
    public TextMeshProUGUI timerText;

    void Start()
    {
        // Démarrer la coroutine pour mettre à jour le timer
        StartCoroutine(UpdateTimer());
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
            timerText.text = "Time: " + GameTimer.Instance.currentTime + "s";
            yield return new WaitForSeconds(1);
        }
    }
}
