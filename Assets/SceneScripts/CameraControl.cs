using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    private const float ToolbarWidth = 120f;
    private const float ToolbarHeight = 100f;

    public const string MouseWheelAxis = "Mouse ScrollWheel";

    public float RotationSpeed = 10f;
    public float ScrollZoomSpeed = 2500f;
    public float ClosestZoom = 340f;
    public float FarthestZoom = 2500f;

    private bool isPlaying      = true;
    private bool isZoomingIn    = false;
    private bool isZoomingOut   = false;
    private bool isMovingUp     = false;
    private bool isMovingDown   = false;
    private bool isMovingLeft   = false;
    private bool isMovingRight  = false;

    private void Update()
    {
        if (isPlaying)
        {
            // Continuously rotate the camera around the scene center
            transform.RotateAround(Vector3.zero, Vector3.up, RotationSpeed * Time.deltaTime);
        }
        else
        {
            HandleControls();
        }

    }

    private void HandleControls()
    {
        // Handle up/down movement
        float xAxis = isMovingUp ? 1f : (isMovingDown ? -1f : 0f);
        if (xAxis != 0)
        {
            transform.RotateAround(Vector3.zero, transform.right, Mathf.Sign(xAxis) * RotationSpeed * Time.deltaTime);
        }

        // Handle left/right movement
        float yAxis = isMovingRight ? 1f : (isMovingLeft ? -1f : 0f);
        if (yAxis != 0)
        {
            transform.RotateAround(Vector3.zero, Vector3.up, Mathf.Sign(yAxis) * RotationSpeed * Time.deltaTime);
        }

        // Handle zooming in and out
        float distance = Vector3.Distance(transform.position, Vector3.zero);
        float zoomAxis = Input.GetAxis(MouseWheelAxis);
        if (zoomAxis == 0)
        {
            zoomAxis = isZoomingIn ? 1f : (isZoomingOut ? -1f : 0f);
        }

        if ((zoomAxis > 0 && distance > ClosestZoom) ||
            (zoomAxis < 0 && distance < FarthestZoom))
        {
            transform.position += transform.forward.normalized * (Mathf.Sign(zoomAxis) * ScrollZoomSpeed * Time.deltaTime);
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, Screen.height - ToolbarHeight - 20, ToolbarWidth, ToolbarHeight));

        GUILayout.Box("Camera controls");
        if (GUILayout.Button(isPlaying ? "Pause" : "Play"))
        {
            isPlaying = !isPlaying;
        }

        GUILayoutOption buttonWidth = GUILayout.Width(ToolbarWidth / 3 - 3f);

        GUILayout.BeginHorizontal();
        isZoomingIn     = GUILayout.RepeatButton("+", buttonWidth);
        isMovingUp      = GUILayout.RepeatButton("\u2191", buttonWidth); // Up arrow
        isZoomingOut    = GUILayout.RepeatButton("-", buttonWidth);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        isMovingLeft    = GUILayout.RepeatButton("\u2190", buttonWidth); // Left arrow
        isMovingDown    = GUILayout.RepeatButton("\u2193", buttonWidth); // Down arrow
        isMovingRight   = GUILayout.RepeatButton("\u2192", buttonWidth); // Right arrow
        GUILayout.EndHorizontal();

        if (isZoomingIn || isZoomingOut || isMovingUp || isMovingDown || isMovingLeft || isMovingRight)
        {
            isPlaying = false;
        }

        GUILayout.EndArea();
    }
}
