using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour
{
    public const string ZoomAxis = "Mouse ScrollWheel";

    public float RotationSpeed = 10f;
    public float ZoomSpeed = 2500f;
    public float ClosestZoom = 340f;
    public float FarthestZoom = 2500f;

    private void Update()
    {
        // Continuously rotate the camera around the scene center
        transform.RotateAround(Vector3.zero, Vector3.up, RotationSpeed * Time.deltaTime);

        // Handle zooming in and out
        float distance = Vector3.Distance(transform.position, Vector3.zero);
        float scroll = Input.GetAxis(ZoomAxis);
        if ((scroll > 0 && distance > ClosestZoom) ||
            (scroll < 0 && distance < FarthestZoom))
        {
            transform.position += transform.forward.normalized * (ZoomSpeed * Time.deltaTime * Mathf.Sign(scroll));
        }
    }
}
