using UnityEngine;
using TMPro;

public class UnitSendSettingsUI : MonoBehaviour
{
    public PlayerController playerController;
    public TMP_InputField keyAInput;
    public TMP_InputField keyEInput;

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (keyAInput != null)
        {
            keyAInput.text = playerController.UnitsForKeyA.ToString();
            keyAInput.onEndEdit.AddListener(OnKeyAChanged);
        }

        if (keyEInput != null)
        {
            keyEInput.text = playerController.UnitsForKeyE.ToString();
            keyEInput.onEndEdit.AddListener(OnKeyEChanged);
        }
    }

    void OnKeyAChanged(string value)
    {
        if (int.TryParse(value, out int amount))
        {
            playerController.SetUnitsForKeyA(amount);
        }
        else if (keyAInput != null)
        {
            keyAInput.text = playerController.UnitsForKeyA.ToString();
        }
    }

    void OnKeyEChanged(string value)
    {
        if (int.TryParse(value, out int amount))
        {
            playerController.SetUnitsForKeyE(amount);
        }
        else if (keyEInput != null)
        {
            keyEInput.text = playerController.UnitsForKeyE.ToString();
        }
    }
}
