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

    // Référence au prefab d'explosion
    public GameObject explosionPrefab;
    public GameObject hoverEffect;
    public GameObject selectedEffect;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private bool isSelected = false;
    public Color playerTextColor = Color.green;
    public Color enemyTextColor = Color.red;
    public Color neutralTextColor = Color.white;
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
            textMesh.color = enemyTextColor;
        }
        else if (owner == "Player")
        {
            textMesh.color = playerTextColor;
        }
        else
        {
            textMesh.color = neutralTextColor;
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
            GalaxyManager galaxyManager = FindObjectOfType<GalaxyManager>();
            if (galaxyManager != null)
            {
                galaxyManager.UpdateAllLines(this);
                galaxyManager.UpdateAllLines(fromStar);
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
                spriteRenderer.color = new Color(0f, 1f, 0f, 0.5f); // Vert semi-transparent
            }
            else if (owner == "Enemy")
            {
                spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f); // Rouge semi-transparent
            }
            else
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Blanc semi-transparent
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
