using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 
Ce script gère l'envoi et le déplacement des unités entre les étoiles dans le jeu.
Il vérifie les conditions nécessaires avant de déplacer les unités, initialise 
les unités avec les informations correctes, et utilise une coroutine pour animer
le mouvement des unités. Le script gère également la logique de conquête des 
étoiles ennemies ou le renforcement des étoiles alliées. Cela permet de simuler les 
mouvements stratégiques des unités et les conflits dans le jeu.
*/
public class UnitManager : MonoBehaviour
{
    public GameObject unitPrefab;
    public PlayerController playerController;
    public GalaxyManager galaxyManager;
    public float unitSpeed = 2.0f;

    private PathFinding pathFinding;

    void Start()
    {
        pathFinding = FindObjectOfType<PathFinding>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab is not assigned in the inspector!");
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned in the inspector or could not be found!");
        }

        if (galaxyManager == null)
        {
            galaxyManager = FindObjectOfType<GalaxyManager>();
        }
    }

    public IEnumerator MoveUnits(Star fromStar, List<Star> path, int unitsToSend)
    {
        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab is not assigned!");
            yield break;
        }

        if (fromStar == null)
        {
            Debug.LogError("FromStar is null!");
            yield break;
        }

        if (path == null || path.Count == 0)
        {
            Debug.LogError("Path is null or empty!");
            yield break;
        }

        // Règle différente pour IA et joueur :
        if (fromStar.Owner != null && galaxyManager != null && fromStar.Owner == galaxyManager.controlledPlayer)
        {
            // Joueur : peut envoyer 100% de ses unités
            unitsToSend = Mathf.Min(unitsToSend, fromStar.units);
        }
        else
        {
            // IA : doit laisser au moins 10 unités
            unitsToSend = Mathf.Min(unitsToSend, fromStar.units - 10);
        }

        if (unitsToSend <= 0)
        {
            Debug.LogWarning("Not enough units to send!");
            yield break;
        }

        fromStar.units -= unitsToSend;
        if (fromStar.isVisible) fromStar.UpdateText();

        GameObject unitInstance = ObjectPooler.Instance.GetFromPool("Unit", fromStar.transform.position, Quaternion.identity);
        if (unitInstance == null)
        {
            Debug.LogError("Could not get a unit from the pool. Check the pooler configuration.");
            yield break;
        }

        Unit unitScript = unitInstance.GetComponent<Unit>();
        if (unitScript == null)
        {
            Debug.LogError("Unit script is not found on the unitPrefab!");
            yield break;
        }
        SpriteRenderer unitSpriteRenderer = unitInstance.GetComponent<SpriteRenderer>();
        if (unitSpriteRenderer != null)
        {
            unitSpriteRenderer.sortingOrder = 1;

            // Ajouter l'effet de glow/neon seulement si activé
            if (UnitGlowSettings.Instance != null && UnitGlowSettings.Instance.EnableGlow)
            {
                Material glowMaterial = new Material(unitSpriteRenderer.material);
                glowMaterial.EnableKeyword("_EMISSION");

                // Couleur d'émission basée sur le propriétaire de l'unité
                Color emissionColor = Color.white; // Couleur par défaut
                if (fromStar.Owner != null)
                {
                    emissionColor = fromStar.Owner.Color;
                }

                // Intensité de l'émission depuis les paramètres
                float emissionIntensity = UnitGlowSettings.Instance.GlowIntensity;
                glowMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);

                // Appliquer le matériau avec émission
                unitSpriteRenderer.material = glowMaterial;
            }
        }
        // Ajoutez un GameObject enfant pour TextMeshPro
        GameObject textObject = new GameObject("UnitText");
        textObject.transform.SetParent(unitInstance.transform);
        TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 5;
        textMesh.text = unitsToSend.ToString();

        // Ajouter l'effet de glow sur le texte
        textMesh.enableAutoSizing = false;
        textMesh.fontSize = 5;

        // Couleur du texte basée sur le propriétaire
        Color textColor = Color.white;
        if (fromStar.Owner != null)
        {
            textColor = fromStar.Owner.Color;
        }
        textMesh.color = textColor;

        // Effet de glow sur le texte seulement si activé
        if (UnitGlowSettings.Instance != null && UnitGlowSettings.Instance.EnableTextGlow)
        {
            textMesh.fontMaterial.EnableKeyword("_EMISSION");
            float textEmissionIntensity = UnitGlowSettings.Instance.TextGlowIntensity;
            textMesh.fontMaterial.SetColor("_EmissionColor", textColor * textEmissionIntensity);
        }

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0.5f, 0);
        rectTransform.sizeDelta = new Vector2(1, 1);

        unitScript.textMesh = textMesh; // Associez le composant texte à l'unité
        unitScript.Initialize(fromStar, path[path.Count - 1], unitsToSend, fromStar.Owner);

        // Créer un effet de glow alternatif avec un sprite enfant
        CreateNeonGlow(unitInstance, fromStar.Owner);

        for (int i = 1; i < path.Count; i++)
        {
            Star currentStar = path[i];
            Vector3 direction = currentStar.transform.position - unitInstance.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            unitInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Ajuster l'angle pour que l'unité pointe dans la direction du mouvement

            while (unitInstance.transform.position != currentStar.transform.position)
            {
                // Vérifier si l'unité doit être visible selon le fog of war
                bool shouldBeVisible = true;
                if (galaxyManager != null && !galaxyManager.showFarStars && galaxyManager.controlledPlayer != null)
                {
                    // Vérifier si la position actuelle de l'unité est dans le champ de vision
                    bool inVision = false;
                    if (SpatialGridManager.Instance != null)
                    {
                        var nearbyStars = SpatialGridManager.Instance.GetStarsInRadius(galaxyManager.controlledPlayer, unitInstance.transform.position, 10f);
                        inVision = nearbyStars.Count > 0;
                    }
                    else
                    {
                        // Fallback: ancienne méthode si la grille n'est pas dispo
                        foreach (Star owned in galaxyManager.controlledPlayer.Stars)
                        {
                            if (Vector3.Distance(unitInstance.transform.position, owned.transform.position) < 10f)
                            {
                                inVision = true;
                                break;
                            }
                        }
                    }
                    shouldBeVisible = inVision;
                }

                // Masquer/afficher l'unité selon la visibilité
                if (unitSpriteRenderer != null)
                {
                    unitSpriteRenderer.enabled = shouldBeVisible;
                }
                if (textMesh != null)
                {
                    textMesh.enabled = shouldBeVisible;
                }

                // Masquer/afficher l'effet de glow aussi
                Transform glowEffect = unitInstance.transform.Find("NeonGlow");
                if (glowEffect != null)
                {
                    SpriteRenderer glowRenderer = glowEffect.GetComponent<SpriteRenderer>();
                    if (glowRenderer != null)
                    {
                        glowRenderer.enabled = shouldBeVisible;
                    }
                }

                unitInstance.transform.position = Vector3.MoveTowards(unitInstance.transform.position, currentStar.transform.position, unitSpeed * Time.deltaTime);
                yield return null;
            }

            if (currentStar.Owner == fromStar.Owner && currentStar != path[path.Count - 1])
            {
                continue;
            }
            else if (currentStar.Owner == fromStar.Owner && currentStar == path[path.Count - 1])
            {
                currentStar.units += unitsToSend;
                if (currentStar.isVisible) currentStar.UpdateText();
                ObjectPooler.Instance.ReturnToPool("Unit", unitInstance);
                yield break;
            }
            else if (currentStar.Owner != unitScript.owner)
            {
                if (unitsToSend > currentStar.units)
                {
                    currentStar.Conquer(unitScript.owner, unitsToSend);
                    ObjectPooler.Instance.ReturnToPool("Unit", unitInstance);
                    yield break;
                }
                else
                {
                    currentStar.units -= unitsToSend;
                    if (currentStar.isVisible) currentStar.UpdateText();
                    ObjectPooler.Instance.ReturnToPool("Unit", unitInstance);
                    yield break;
                }
            }
        }

        ObjectPooler.Instance.ReturnToPool("Unit", unitInstance);
    }


    public void SendUnits(Star fromStar, Star toStar, int unitsToSend)
    {
        if (pathFinding == null)
        {
            Debug.LogError("PathFinding component not found!");
            return;
        }

        List<Star> path = pathFinding.FindPath(fromStar, toStar); // Utiliser galaxyManager ici
        if (path.Count > 0)
        {
            StartCoroutine(MoveUnits(fromStar, path, unitsToSend));
        }
    }

    void CreateNeonGlow(GameObject unitInstance, Player owner)
    {
        // Crée un GameObject enfant pour le halo
        GameObject glow = new GameObject("NeonGlow");
        glow.transform.SetParent(unitInstance.transform);
        glow.transform.localPosition = Vector3.zero;
        glow.transform.localScale = Vector3.one * 2.5f; // Ajuste la taille selon l'effet voulu

        // Ajoute un SpriteRenderer pour le halo
        SpriteRenderer glowRenderer = glow.AddComponent<SpriteRenderer>();
        // Charge le sprite de halo
        if (galaxyManager != null && galaxyManager.glowSprite != null)
        {
            glowRenderer.sprite = galaxyManager.glowSprite;
        }
        else
        {
            Debug.LogError("GalaxyManager or its glowSprite is not assigned!");
        }

        // Couleur du halo = couleur du propriétaire, très saturée
        Color glowColor = owner != null ? owner.Color : Color.white;
        glowColor.a = 1f; // Transparence
        glowRenderer.color = glowColor;

        // S'assure que le halo est derrière l'unité
        glowRenderer.sortingOrder = 0;
    }
}
