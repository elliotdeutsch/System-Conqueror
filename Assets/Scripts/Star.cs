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
    public Sprite selectedMotherPlanetSprite;
    public Sprite motherBaseAlliedSprite;
    public Sprite motherBaseEnemySprite;


    // Référence au prefab d'explosion
    public GameObject explosionPrefab;

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
        SetInitialSprite();
        AdjustColliderSize();

        lastGeneratedTime = GameTimer.Instance.currentTime;

        switch (starType)
        {
            case StarType.MotherBaseAllied:
            case StarType.MotherBaseEnemy:
                StartCoroutine(GenerateUnits(15, 5)); // 15 unités toutes les 5 secondes
                break;
            case StarType.ConqueredAllied:
            case StarType.ConqueredEnemy:
                StartCoroutine(GenerateUnits(2, 5)); // 2 unités toutes les 5 secondes
                break;
        }

        textMesh.fontSize = 5;
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
        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        SetSpriteBasedOnOwner();
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

            SetSpriteBasedOnOwner();

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
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void SetInitialSprite()
    {
        if (spriteRenderer != null)
        {
            switch (starType)
            {
                case StarType.MotherBaseAllied:
                    spriteRenderer.sprite = motherBaseAlliedSprite;
                    break;
                case StarType.MotherBaseEnemy:
                    spriteRenderer.sprite = motherBaseEnemySprite;
                    break;
                case StarType.ConqueredAllied:
                    spriteRenderer.sprite = playerPlanetSprite;
                    break;
                case StarType.ConqueredEnemy:
                    spriteRenderer.sprite = enemyPlanetSprite;
                    break;
                default:
                    spriteRenderer.sprite = neutralPlanetSprite;
                    break;
            }
        }
    }

    private void SetSpriteBasedOnOwner()
    {
        if (spriteRenderer != null)
        {
            if (isSelected && owner == "Player")
            {
                if (starType != StarType.MotherBaseAllied)
                {
                    spriteRenderer.sprite = selectedPlayerPlanetSprite;
                }
                else
                {
                    spriteRenderer.sprite = selectedMotherPlanetSprite;
                }
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
            float spriteRadius = spriteRenderer.bounds.size.x / 2;
            circleCollider.radius = spriteRadius;
        }
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
