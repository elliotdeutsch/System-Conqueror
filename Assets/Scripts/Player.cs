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
        Name = name;
        Color = color;
        IsAI = isAI;
        Stars = new List<Star>();
    }
}
