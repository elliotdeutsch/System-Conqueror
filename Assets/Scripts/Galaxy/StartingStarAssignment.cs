using System.Collections.Generic;
using UnityEngine;

public class StartingStarAssignment : MonoBehaviour
{
    private List<Star> stars;

    public void Initialize(List<Star> starList)
    {
        stars = starList;
    }

    public void AssignStartingStars(List<Player> players)
    {
        foreach (var player in players)
        {
            Star startingStar = AssignStartingStar(player);
            if (startingStar != null)
            {
                player.Stars.Add(startingStar);
                if (!stars.Contains(startingStar))
                {
                    stars.Add(startingStar); // Ajouter les étoiles de départ à la liste des étoiles
                }
            }
        }
    }

    public Star AssignStartingStar(Player player)
    {
        if (stars.Count > 0)
        {
            Star startingStar = stars[0]; // Choisir une étoile de départ non assignée
            startingStar.Owner = player;
            startingStar.units = 100; // Nombre d'unités de départ
            startingStar.isNeutral = false;

            // Assigner le type d'étoile
            startingStar.starType = Star.StarType.Capital; // Assigner comme étoile mère

            // Ajouter "(Capitale)" au nom de l'étoile
            startingStar.starName += " (Capitale)";

            startingStar.SetInitialSprite();
            StartCoroutine(startingStar.GenerateUnits(15, 5)); // 15 unités toutes les 5 secondes
            stars.Remove(startingStar); // Retirer cette étoile de la liste des étoiles disponibles
            return startingStar;
        }
        return null;
    }
}
