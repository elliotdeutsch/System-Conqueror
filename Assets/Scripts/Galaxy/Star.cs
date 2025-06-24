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
    public string starName;
    public int units;
    public bool isNeutral;
    public string owner;
    public Player Owner { get; set; }

    public bool isCapital;

    public TextMeshPro textMesh;
    public GameObject explosionPrefab;
    public GameObject hoverEffect;
    public GameObject selectedEffect;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private bool isSelected = false;
    public bool isVisible = true;
    public Color neutralColor = Color.white;

    private int lastGeneratedTime;
    public enum StarType { Neutral, Normal, Capital, Conquered }
    public StarType starType;

    private LineManager lineManager;
    private GalaxyManager galaxyManager;

    void Start()
    {
        lineManager = FindObjectOfType<LineManager>();
        galaxyManager = FindObjectOfType<GalaxyManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        hoverEffect = transform.Find("HoverEffect").gameObject;
        selectedEffect = transform.Find("SelectedEffect").gameObject;

        // Ajout du glow sous la planète
        GameObject glow = new GameObject("StarGlow");
        glow.transform.SetParent(transform);
        glow.transform.localPosition = Vector3.zero;
        glow.transform.localScale = Vector3.one * 0.4f;
        SpriteRenderer glowRenderer = glow.AddComponent<SpriteRenderer>();
        if (galaxyManager != null && galaxyManager.haloSprite != null)
        {
            glowRenderer.sprite = galaxyManager.haloSprite;
        }
        else
        {
            Debug.LogError("GalaxyManager or its haloSprite is not assigned!");
        }

        Color glowColor = (Owner != null) ? Owner.Color : Color.white;
        glowColor.a = 1f;
        glowRenderer.color = glowColor;
        glowRenderer.sortingOrder = 3;

        SetColorBasedOnOwner();

        lastGeneratedTime = GameTimer.Instance.currentTime;

        GameTimer.Instance.OnFiveSecondInterval += HandleFiveSecondInterval;

        textMesh.fontSize = 5;

        // Initialement cacher les effets de survol et de sélection
        hoverEffect.SetActive(false);
        selectedEffect.SetActive(false);

        UpdateText();

        // Ajout à la grille spatiale si l'étoile a un propriétaire
        if (Owner != null && SpatialGridManager.Instance != null)
        {
            SpatialGridManager.Instance.AddStar(Owner, this);
        }
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
        if (Owner != null && !isNeutral)
        {
            int unitsPerInterval = 0;
            switch (starType)
            {
                case StarType.Capital:
                    unitsPerInterval = 15;
                    break;
                case StarType.Normal:
                case StarType.Conquered:
                    unitsPerInterval = 2;
                    break;
            }
            if (unitsPerInterval > 0)
            {
                units += unitsPerInterval;
                if (isVisible)
                {
                    UpdateText();
                    // Afficher le texte flottant pour la génération d'unités
                    if (FloatingTextManager.Instance != null)
                    {
                        FloatingTextManager.Instance.ShowUnitGeneration(this, unitsPerInterval);
                    }
                }
            }
        }
    }

    void Update()
    {
        // Supprimer la coroutine GenerateUnits et tout appel à celle-ci
    }

    public void UpdateText()
    {
        if (!isVisible)
        {
            textMesh.text = "";
            return;
        }
        if (Owner != null)
        {
            textMesh.color = Owner.Color;
        }
        else
        {
            textMesh.color = neutralColor;
        }

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

    public void SetVisibility(bool visible)
    {
        isVisible = visible;
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(true);
        }
        // Gérer le glow
        Transform glowEffect = transform.Find("StarGlow");
        if (glowEffect != null)
        {
            SpriteRenderer glowRenderer = glowEffect.GetComponent<SpriteRenderer>();
            if (glowRenderer != null)
            {
                glowRenderer.enabled = visible;
            }
        }
        if (isVisible)
        {
            SetColorBasedOnOwner();
            if (textMesh != null)
            {
                textMesh.color = (Owner != null) ? Owner.Color : neutralColor;
                textMesh.text = $"{starName}\nUnits: {units}";
            }
        }
        else
        {
            // Planète hors champ de vision : gris semi-transparent, pas d'info propriétaire ni unités
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f, 0.05f); // gris semi-transparent
            }
            if (textMesh != null)
            {
                textMesh.color = Color.gray;
                textMesh.text = starName; // Affiche seulement le nom
            }
            // Empêcher toute réapplication de la couleur propriétaire
            // (par exemple via Start ou un autre script)
        }
        // Toujours désactiver les effets si hors vision
        if (!isVisible)
        {
            if (hoverEffect != null) hoverEffect.SetActive(false);
            if (selectedEffect != null) selectedEffect.SetActive(false);
        }
        if (isVisible) UpdateText();
    }

    // Ajout : méthode utilitaire pour savoir si la planète est visible (champ de vision)
    public bool IsInVision() => isVisible;

    public void Conquer(Player attackingOwner, int attackingUnits)
    {
        Player oldOwner = Owner;
        if (attackingUnits >= units)
        {
            units = attackingUnits - units;
            isNeutral = false;
            if (isVisible) UpdateText();

            if (Owner != null)
            {
                Owner.Stars.Remove(this);
                // Retirer de la grille de l'ancien propriétaire
                if (SpatialGridManager.Instance != null)
                {
                    SpatialGridManager.Instance.RemoveStar(Owner, this);
                }
            }
            Owner = attackingOwner;
            if (Owner != null)
            {
                Owner.Stars.Add(this);
                // Ajouter à la grille du nouveau propriétaire
                if (SpatialGridManager.Instance != null)
                {
                    SpatialGridManager.Instance.AddStar(Owner, this);
                }
            }
            if (starType == StarType.Capital)
            {
                starType = StarType.Conquered;
            }
            else
            {
                starType = StarType.Normal;
            }
            // Synchroniser la génération d'unités avec le timer global (plus besoin de coroutine)
            // Si la planète est visible, animation normale, sinon reste grise
            if (isVisible)
            {
                SetColorBasedOnOwner();
                PlayExplosion();
                StartCoroutine(ExplosionAnimation());
            }
            else
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
            }
            if (lineManager != null)
            {
                lineManager.UpdateAllLines(this);
                // lineManager.UpdateAllLines(fromStar); // plus besoin
            }
            if (galaxyManager != null)
            {
                galaxyManager.UpdatePlayerListUI();
                galaxyManager.UpdateFogOfWar();
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerController.UpdateFogOfWarDisplay();
                }
            }
        }
        else
        {
            units -= attackingUnits;
            if (isVisible) UpdateText();
        }
    }
    public void SetColorBasedOnOwner()
    {
        if (Owner != null)
        {
            GetComponent<SpriteRenderer>().color = Owner.Color;
            // Mettre à jour la couleur du halo
            Transform glowEffect = transform.Find("StarGlow");
            if (glowEffect != null)
            {
                SpriteRenderer glowRenderer = glowEffect.GetComponent<SpriteRenderer>();
                if (glowRenderer != null)
                {
                    glowRenderer.color = Owner.Color;
                }
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().color = neutralColor;
            // Mettre à jour la couleur du halo
            Transform glowEffect = transform.Find("StarGlow");
            if (glowEffect != null)
            {
                SpriteRenderer glowRenderer = glowEffect.GetComponent<SpriteRenderer>();
                if (glowRenderer != null)
                {
                    glowRenderer.color = Color.white;
                }
            }
        }
        if (isVisible) UpdateText();
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
