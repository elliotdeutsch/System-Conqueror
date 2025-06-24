using UnityEngine;

/* 
Ce script permet de contrôler une caméra en vue 2D dans Unity, 
offrant la possibilité de se déplacer dans l'espace de jeu en utilisant le clavier 
ou la souris, et de zoomer en avant ou en arrière en utilisant la molette de la souris. 
Les limites de déplacement et de zoom sont configurables via l'inspecteur de Unity, 
ce qui offre une grande flexibilité pour adapter ce script à différents types de jeux 
ou de scènes. 
*/

public class CameraController : MonoBehaviour
{
    // Vitesse de déplacement de la caméra
    public float panSpeed = 20f;
    // Vitesse de zoom de la caméra
    public float zoomSpeed = 5f;
    // Limite de zoom minimum et maximum
    public float minZoom = 5f;
    public float maxZoom = 20f;
    // Limites de déplacement de la caméra
    public Vector2 panLimit;

    // Origine du clic pour le déplacement de la caméra
    private Vector3 dragOrigin;
    private bool isDragging = false;
    private GalaxyManager galaxyManager;

    void Awake()
    {
        galaxyManager = FindObjectOfType<GalaxyManager>();
        // Set camera background to solid black
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = Color.black;
        }
    }

    void Start()
    {
        // Centrer la caméra sur le joueur au démarrage
        CenterOnPlayer();
    }

    void Update()
    {
        // Gérer le mouvement et le zoom de la caméra
        HandleMovement();
        HandleZoom();
        HandleRefocus();
    }

    void HandleRefocus()
    {
        // Touche R pour refocus sur le joueur
        if (Input.GetKeyDown(KeyCode.R))
        {
            CenterOnPlayer();
        }
    }

    public void UpdatePanLimits()
    {
        if (galaxyManager != null)
        {
            panLimit = new Vector2(galaxyManager.mapWidth / 2, galaxyManager.mapHeight / 2);
        }
    }

    public void CenterOnPlayer()
    {
        if (galaxyManager != null && galaxyManager.controlledPlayer != null && galaxyManager.controlledPlayer.Stars.Count > 0)
        {
            // Centrer sur la première planète du joueur (généralement la capitale)
            Star playerStar = galaxyManager.controlledPlayer.Stars[0];
            if (playerStar != null)
            {
                Vector3 targetPosition = playerStar.transform.position;
                targetPosition.z = transform.position.z; // Garder la même profondeur
                transform.position = targetPosition;
                Debug.Log($"Caméra centrée sur {playerStar.starName}");
            }
        }
    }

    void HandleMovement()
    {
        Vector3 pos = transform.position;

        // Déplacement avec les touches ZQSD ou les touches fléchées
        if (Input.GetKey(KeyCode.W))
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        // Déplacement avec le clic droit de la souris
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(dragOrigin) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos += difference;
            dragOrigin = Input.mousePosition;
        }

        // Limiter le déplacement de la caméra aux bornes définies
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);

        transform.position = pos;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
    }
}
