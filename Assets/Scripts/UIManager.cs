using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI systemNameText;
    public TextMeshProUGUI unitsText;

    public void UpdateSystemInfo(Star star)
    {
        systemNameText.text = "System: " + star.starName;
        unitsText.text = "Units: " + star.units;
    }
}
