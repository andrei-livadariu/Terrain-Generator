﻿using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private const float ToolbarWidth = 120f;
    private const float ToolbarHeight = 100f;
#if !UNITY_WEBPLAYER
    private const float QuitButtonWidth = 50f;
#endif

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

#if !UNITY_WEBPLAYER
    private Rect quitButtonPosition;
#endif
    private Rect cameraToolbarPosition;

    private void Awake()
    {
#if !UNITY_WEBPLAYER
        quitButtonPosition = new Rect(Screen.width - QuitButtonWidth - 20, 20, QuitButtonWidth, Screen.height);
#endif

#if UNITY_WEBPLAYER
        cameraToolbarPosition = new Rect(Screen.width - ToolbarWidth - 20, 20, ToolbarWidth, ToolbarHeight);
#else
        cameraToolbarPosition = new Rect(20, Screen.height - ToolbarHeight - 20, ToolbarWidth, ToolbarHeight);
#endif
    }

    private void Update()
    {
#if !UNITY_WEBPLAYER
        // Quit the application
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif

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
#if !UNITY_WEBPLAYER
        GUILayout.BeginArea(quitButtonPosition);
        if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }
        GUILayout.EndArea();
#endif

        GUILayout.BeginArea(cameraToolbarPosition);

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

        if (isMovingLeft || isMovingRight)
        {
            isPlaying = false;
        }

        // "Play" basically means "continuously move right"
        if (isPlaying)
        {
            isMovingRight = true;
        }

        GUILayout.EndArea();
    }
}
