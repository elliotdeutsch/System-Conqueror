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
    private bool isVisible = true;
    public Color neutralColor = Color.white;

    private int lastGeneratedTime;
    public enum StarType { Neutral, Normal, Capital, Conquered }
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

        UpdateText();
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
                case StarType.Capital:
                    unitsPerInterval = 15;
                    break;
                case StarType.Normal:
                    unitsPerInterval = 2;
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
        // Ne plus appeler UpdateText() ici car SetVisibility() gère déjà l'affichage
        // UpdateText() est appelé uniquement quand nécessaire dans SetVisibility()
    }

    private void UpdateText()
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
    }

    // Ajout : méthode utilitaire pour savoir si la planète est visible (champ de vision)
    public bool IsInVision() => isVisible;

    public void Conquer(Star fromStar, int attackingUnits)
    {
        if (attackingUnits >= units)
        {
            units = attackingUnits - units;
            isNeutral = false;

            if (Owner != null)
            {
                Owner.Stars.Remove(this);
            }
            Owner = fromStar.Owner;
            if (Owner != null)
            {
                Owner.Stars.Add(this);
            }
            if (starType == StarType.Capital)
            {
                starType = StarType.Conquered;
            }
            // Si la planète est visible, animation normale, sinon reste grise
            if (isVisible)
            {
                SetColorBasedOnOwner();
                PlayExplosion();
                StartCoroutine(GenerateUnits(2, 5));
                StartCoroutine(ExplosionAnimation());
            }
            else
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
            }
            LineManager lineManager = FindObjectOfType<LineManager>();
            if (lineManager != null)
            {
                lineManager.UpdateAllLines(this);
                lineManager.UpdateAllLines(fromStar);
            }
            GalaxyManager galaxyManager = FindObjectOfType<GalaxyManager>();
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
        }
    }
    public void SetColorBasedOnOwner()
    {
        if (Owner != null)
        {
            GetComponent<SpriteRenderer>().color = Owner.Color;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = neutralColor;
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
