using System.Collections.Generic;
using UnityEngine;

/* 
Ce script permet de générer des noms d'étoiles de manière 
procédurale en combinant des préfixes et des suffixes prédéfinis.
La méthode GenerateStarName choisit aléatoirement un préfixe et 
un suffixe dans leurs listes respectives, puis les combine pour créer un nom 
unique. Cette fonctionnalité est utile pour créer des noms variés et 
intéressants pour les étoiles dans un jeu, ajoutant ainsi de la diversité et de l'immersion.
*/

public class StarNameGenerator : MonoBehaviour
{
    private List<string> prefixes = new List<string> { "Alpha", "Beta", "Delta", "Epsilon", "Zeta", "Vega", "Sirius", "Altair" };
    private List<string> suffixes = new List<string> { "ari", "os", "ion", "a", "or", "us", "ius", "ix" };

    public string GenerateStarName()
    {
        string prefix = prefixes[Random.Range(0, prefixes.Count)];
        string suffix = suffixes[Random.Range(0, suffixes.Count)];
        return prefix + suffix;
    }
}
