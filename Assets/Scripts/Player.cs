using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name { get; set; }
    public Color Color { get; set; }
    public List<Star> Stars { get; set; }
    public bool IsAI { get; set; }
    public float CheckInterval { get; set; } = 5.0f; // Intervalle de vérification pour l'IA

    public Player(string name, Color color, bool isAI)
    {
        // Name = string.IsNullOrEmpty(name) ? GenerateRandomName() : name;
        // add index to name:
        Name = GenerateRandomName();
        Color = color;
        IsAI = isAI;
        Stars = new List<Star>();
    }

    private string GenerateRandomName()
    {
        string[] names = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett", "Kilo", "Lima", "Mike", "November", "Oscar", "Papa", "Quebec", "Romeo", "Sierra", "Tango", "Uniform", "Victor", "Whiskey", "X-Ray", "Yankee", "Zulu" };
        return names[Random.Range(0, names.Length)];
    }
}
