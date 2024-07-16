using UnityEngine;
using TMPro;

/* 
Ce script gère les unités qui se déplacent entre les étoiles dans 
le jeu. Il permet d'initialiser les unités avec des informations sur 
leur origine, leur destination et leur nombre. Le script met également
à jour l'affichage du nombre d'unités au-dessus de chaque unité pendant
le jeu. Cela garantit que les informations pertinentes sur les unités
sont toujours visibles pour le joueur, ce qui est crucial pour la gestion
stratégique des déplacements des unités dans le jeu.
*/

public class Unit : MonoBehaviour
{
    public Star fromStar;
    public string owner;
    public Star targetStar;
    public int units;
    public TextMeshPro textMesh;

    public void Initialize(Star from, Star target, int unitCount)
    {
        fromStar = from;
        targetStar = target;
        units = unitCount;
        owner = from.Owner.Name; // Assurez-vous de définir le propriétaire correctement

        if (textMesh != null)
        {
            textMesh.text = units.ToString();
        }
    }


    void Update()
    {
        if (textMesh != null)
        {
            textMesh.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        }
    }
}
