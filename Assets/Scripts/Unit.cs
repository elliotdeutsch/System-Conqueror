using UnityEngine;
using TMPro;

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

        // Assurez-vous que le texte est mis à jour
        if (textMesh != null)
        {
            textMesh.text = units.ToString();
        }
    }

    void Update()
    {
        // Assurez-vous que le texte suit l'unité
        if (textMesh != null)
        {
            textMesh.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        }
    }
}
