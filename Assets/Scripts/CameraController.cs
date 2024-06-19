using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public Vector2 panLimit;

    private Vector3 dragOrigin;
    private bool isDragging = false;

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        Vector3 pos = transform.position;

        // Déplacement avec les touches ZQSD
        if (Input.GetKey("z") || Input.GetKey(KeyCode.W))
        {
            pos.y += panSpeed * Time.deltaTime;
            //print in console the position of the camera :
}
        if (Input.GetKey("s") || Input.GetKey(KeyCode.S))
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("q") || Input.GetKey(KeyCode.A))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.GetKey(KeyCode.D))
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
