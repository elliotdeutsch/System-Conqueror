using UnityEngine;
using TMPro;
using System.Collections;

public class Star : MonoBehaviour
{
    public string starName = "Unnamed Star"; // Nom de l'étoile
    public int units = 25; // Nombre d'unités
    public bool isNeutral = true; // Si l'étoile est neutre ou contrôlée
    public string owner; // Propriétaire de l'étoile
    public TextMeshPro textMesh;

    // Sprites pour les différents états
    public Sprite neutralPlanetSprite;
    public Sprite enemyPlanetSprite;
    public Sprite playerPlanetSprite;
    public Sprite selectedPlayerPlanetSprite;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private bool isSelected = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        SetInitialSprite();
        AdjustColliderSize();

        if (owner == "Player")
        {
            StartCoroutine(GenerateUnits());
        }

        // Configurer la taille de la police
        textMesh.fontSize = 5; // Ajustez cette valeur selon vos besoins
    }

    void Update()
    {
        if (textMesh != null)
        {
            // Mise à jour du texte
            textMesh.text = $"{starName}\nUnits: {units}";
        }
    }

    void OnMouseEnter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Change la couleur pour rendre le sprite plus sombre
        }
    }

    void OnMouseExit()
    {
        SetSpriteBasedOnOwner(); // Reviens à la couleur originale
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        SetSpriteBasedOnOwner();
    }

    public void Conquer(Star fromStar, int attackingUnits)
    {
        if (attackingUnits > units)
        {
            int remainingUnits = attackingUnits - units; // Les unités restantes après la conquête
            units = remainingUnits; // Les unités excédentaires deviennent les nouvelles unités de la planète conquise
            isNeutral = false;
            owner = fromStar.owner; // L'étoile est conquise par le propriétaire de l'étoile attaquante
            SetInitialSprite();

            if (owner == "Player")
            {
                StartCoroutine(GenerateUnits()); // Démarrer la génération d'unités
            }

            // Ajouter une animation d'explosion
            StartCoroutine(ExplosionAnimation());
        }
        else
        {
            // Les unités attaquantes ont été détruites
            units -= attackingUnits;
        }
    }

    public IEnumerator GenerateUnits()
    {
        while (owner == "Player")
        {
            yield return new WaitForSeconds(1f);
            units++;
        }
    }

    private IEnumerator ExplosionAnimation()
    {
        // Ajouter une animation d'explosion ici
        // Exemple simple d'un changement de couleur rapide pour simuler une explosion
        for (int i = 0; i < 5; i++)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                SetSpriteBasedOnOwner();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void SetInitialSprite()
    {
        if (spriteRenderer != null)
        {
            if (owner == "Player")
            {
                spriteRenderer.sprite = playerPlanetSprite;
            }
            else if (owner == "Enemy")
            {
                spriteRenderer.sprite = enemyPlanetSprite;
            }
            else
            {
                spriteRenderer.sprite = neutralPlanetSprite;
            }
        }
    }

    private void SetSpriteBasedOnOwner()
    {
        if (spriteRenderer != null)
        {
            if (isSelected && owner == "Player")
            {
                spriteRenderer.sprite = selectedPlayerPlanetSprite;
            }
            else
            {
                if (owner == "Player")
                {
                    spriteRenderer.sprite = playerPlanetSprite;
                }
                else if (owner == "Enemy")
                {
                    spriteRenderer.sprite = enemyPlanetSprite;
                }
                else
                {
                    spriteRenderer.sprite = neutralPlanetSprite;
                }
            }
        }
    }

    private void AdjustColliderSize()
    {
        if (circleCollider != null && spriteRenderer != null)
        {
            // Ajuster le rayon du colliseur pour correspondre à la taille du sprite
            float spriteRadius = spriteRenderer.bounds.size.x / 2;
            circleCollider.radius = spriteRadius;
        }
    }
}
