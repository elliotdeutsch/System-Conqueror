using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DeveloperConsole : MonoBehaviour
{
    public GameObject consoleUI;
    public TMP_InputField consoleInput;
    public TMP_Text consoleHistoryText;

    private Dictionary<string, System.Action> commands;
    private static bool isConsoleOpen = false;
    private static DeveloperConsole instance;

    private void Awake()
    {
        instance = this;
        commands = new Dictionary<string, System.Action>
        {
            { "motherlode", Motherlode }
        };

        if (consoleUI == null)
        {
            Debug.LogError("DeveloperConsole: Console UI is not assigned!");
            enabled = false;
            return;
        }
        if (consoleInput == null)
        {
            Debug.LogError("DeveloperConsole: Console Input is not assigned!");
            enabled = false;
            return;
        }
        if (consoleHistoryText == null)
        {
            Debug.LogError("DeveloperConsole: Console History Text is not assigned!");
            enabled = false;
            return;
        }

        consoleUI.SetActive(false);
        consoleInput.onSubmit.RemoveAllListeners();
        consoleInput.onEndEdit.RemoveAllListeners();
        consoleInput.onEndEdit.AddListener(OnInputSubmit);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }
    }

    private void ToggleConsole()
    {
        isConsoleOpen = !isConsoleOpen;
        consoleUI.SetActive(isConsoleOpen);

        if (isConsoleOpen)
        {
            consoleInput.text = "";
            consoleInput.ActivateInputField();
        }
    }

    private void OnInputSubmit(string input)
    {
        if (!isConsoleOpen) return;
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || !string.IsNullOrWhiteSpace(input))
        {
            ProcessCommand(input);
        }
    }

    private void ProcessCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        string[] parts = input.Split(' ');
        string command = parts[0].ToLower();

        AddToHistory($"> {input}");

        if (commands.ContainsKey(command))
        {
            commands[command].Invoke();
        }
        else
        {
            AddToHistory($"Unknown command: {command}");
            Debug.LogWarning($"Unknown command: {command}");
        }

        consoleInput.text = "";
        consoleInput.ActivateInputField();
    }

    private void Motherlode()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null && playerController.SelectedStars.Any())
        {
            Star lastSelectedStar = playerController.SelectedStars.Last();

            if (lastSelectedStar != null && lastSelectedStar.Owner == playerController.player)
            {
                lastSelectedStar.units += 1000;
                lastSelectedStar.UpdateText();
                AddToHistory($"+1000 units to {lastSelectedStar.starName}. New total: {lastSelectedStar.units}");
                Debug.Log($"+1000 units to {lastSelectedStar.starName}. New total: {lastSelectedStar.units}");
            }
            else
            {
                AddToHistory("Motherlode command failed: The selected star does not belong to the player.");
                Debug.LogWarning("Motherlode command failed: The selected star does not belong to the player.");
            }
        }
        else
        {
            AddToHistory("Motherlode command failed: No star selected.");
            Debug.LogWarning("Motherlode command failed: No star selected.");
        }
    }

    private void AddToHistory(string message)
    {
        if (consoleHistoryText != null)
        {
            consoleHistoryText.text += message + "\n";
        }
    }

    public static bool IsConsoleOpen => isConsoleOpen;
}