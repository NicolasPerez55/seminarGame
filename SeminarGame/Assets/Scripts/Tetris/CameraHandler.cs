using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Vector2 velocity;

    public int zoomIndex = 0;
    public float zoomScale = 0.66f;

    public float damp = .96f;
    public float spring = .3f;

    private void Update()
    {
        velocity -= velocity * damp * Time.deltaTime;

        velocity -= (Vector2)transform.position * spring * Time.deltaTime;

        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public void Shake(Vector2 direction)
    {
        velocity += direction;
    }

    public void IncrementZoom()
    {
        zoomIndex++;
        if (zoomIndex <= 5)
            Camera.main.orthographicSize += zoomScale;
    }
}
