using UnityEngine;
using TMPro;
using System.Collections;

/* 
Ce script gère les propriétés et les comportements des étoiles dans le jeu.
Il permet de suivre le nombre d'unités sur chaque étoile, de gérer les effets
visuels pour le survol et la sélection, et de générer des unités à intervalles
réguliers si l'étoile est contrôlée par un joueur ou un ennemi. Le script prend
également en charge les animations d'explosion et ajuste l'apparence de l'étoile
en fonction de son propriétaire. Cette gestion détaillée des étoiles est essentielle
pour représenter l'état et les interactions des éléments clés dans le jeu.
*/

public class Star : MonoBehaviour
{
    public string starName = "Unnamed Star"; // Nom de l'étoile
    public int units = 25; // Nombre d'unités
    public bool isNeutral = true; // Si l'étoile est neutre ou contrôlée
    public string owner; // Propriétaire de l'étoile
    public TextMeshPro textMesh;

    // Référence au prefab d'explosion
    public GameObject explosionPrefab;
    public GameObject hoverEffect;
    public GameObject selectedEffect;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private bool isSelected = false;
    public Color playerColor = Color.green;
    public Color enemyColor = Color.red;
    public Color neutralColor = Color.white;


    private int lastGeneratedTime;
    public enum StarType { Neutral, MotherBaseAllied, MotherBaseEnemy, ConqueredAllied, ConqueredEnemy }
    public StarType starType;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        hoverEffect = transform.Find("HoverEffect").gameObject;
        selectedEffect = transform.Find("SelectedEffect").gameObject;

        SetColorBasedOnOwner();

        lastGeneratedTime = GameTimer.Instance.currentTime;

        GameTimer.Instance.OnFiveSecondInterval += HandleFiveSecondInterval;

        textMesh.fontSize = 5;

        // Initialement cacher les effets de survol et de sélection
        hoverEffect.SetActive(false);
        selectedEffect.SetActive(false);
    }

    void OnDestroy()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnFiveSecondInterval -= HandleFiveSecondInterval;
        }
    }

    void HandleFiveSecondInterval()
    {
        if (owner != "Neutral")
        {
            int unitsPerInterval = 0;
            switch (starType)
            {
                case StarType.MotherBaseAllied:
                case StarType.MotherBaseEnemy:
                    unitsPerInterval = 15;
                    break;
                case StarType.ConqueredAllied:
                case StarType.ConqueredEnemy:
                    if (GameTimer.Instance.currentTime - lastGeneratedTime >= 5)
                    {
                        unitsPerInterval = 2;
                    }
                    break;
            }

            if (unitsPerInterval > 0)
            {
                units += unitsPerInterval;
                lastGeneratedTime = GameTimer.Instance.currentTime;
            }
        }
    }



    void Update()
    {
        // Mettre à jour la couleur du texte en fonction du propriétaire
        if (owner == "Enemy")
        {
            textMesh.color = enemyColor;
        }
        else if (owner == "Player")
        {
            textMesh.color = playerColor;
        }
        else
        {
            textMesh.color = neutralColor;
        }

        // Mise à jour du texte sans changer la position
        textMesh.text = $"{starName}\nUnits: {units}";
    }

    void OnMouseEnter()
    {
        hoverEffect.SetActive(true);
    }

    void OnMouseExit()
    {
        hoverEffect.SetActive(false);
    }
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        SetColorBasedOnOwner();
    }

    public void Conquer(Star fromStar, int attackingUnits)
    {
        if (attackingUnits >= units)
        {
            units = attackingUnits - units;
            isNeutral = false;
            owner = fromStar.owner;

            // Définir le type de l'étoile conquise
            if (owner == "Player")
            {
                starType = StarType.ConqueredAllied;
            }
            else if (owner == "Enemy")
            {
                starType = StarType.ConqueredEnemy;
            }

            SetColorBasedOnOwner();

            // Appeler l'explosion avant de démarrer la génération d'unités
            PlayExplosion();

            // Démarrer la génération d'unités pour les planètes conquises
            StartCoroutine(GenerateUnits(2, 5));

            // Mettre à jour les lignes entre toutes les planètes connectées
            LineManager lineManager = FindObjectOfType<LineManager>();

            if (lineManager != null)
            {
                lineManager.UpdateAllLines(this);
                lineManager.UpdateAllLines(fromStar);
            }

            StartCoroutine(ExplosionAnimation());
        }
        else
        {
            units -= attackingUnits;
        }
    }

    private void SetColorBasedOnOwner()
    {
        if (spriteRenderer != null)
        {
            if (owner == "Player")
            {
                spriteRenderer.color = playerColor; // Utilisez la couleur du texte du joueur pour la planète
                textMesh.color = playerColor; // Utilisez la couleur du texte du joueur
            }
            else if (owner == "Enemy")
            {
                spriteRenderer.color = enemyColor; // Utilisez la couleur du texte de l'ennemi pour la planète
                textMesh.color = enemyColor; // Utilisez la couleur du texte de l'ennemi
            }
            else
            {
                spriteRenderer.color = neutralColor; // Utilisez la couleur du texte neutre pour la planète
                textMesh.color = neutralColor; // Utilisez la couleur du texte neutre
            }
        }
    }



    public IEnumerator GenerateUnits(int unitsPerInterval = 1, int interval = 1)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (owner != "Neutral")
            {
                int currentTime = GameTimer.Instance.currentTime;
                int elapsedIntervals = (currentTime - lastGeneratedTime) / interval;
                if (elapsedIntervals > 0)
                {
                    units += unitsPerInterval * elapsedIntervals;
                    lastGeneratedTime += elapsedIntervals * interval;
                }
            }
        }
    }

    private IEnumerator ExplosionAnimation()
    {
        for (int i = 0; i < 5; i++)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                SetColorBasedOnOwner();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    // Fonction pour définir la couleur initiale de chaque étoile
    public void SetInitialSprite()
    {
        if (spriteRenderer != null)
        {
            SetColorBasedOnOwner();
        }
    }

    // Fonction pour définir la couleur en fonction du propriétaire
    private void SetSpriteBasedOnOwner()
    {
        SetColorBasedOnOwner();
    }


    public void PlayExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2.0f); // Détruire l'animation d'explosion après 2 secondes
        }
        else
        {
            Debug.LogWarning("Explosion prefab is not assigned.");
        }
    }


}
