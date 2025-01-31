using System.Collections;
using UnityEngine;

public class Man : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float fadeDuration = 60f; // Duration of fade-in in seconds (1 minute)
    private float fadeStartTime = 390f; // 6.5 minutes in seconds
    private float fadeEndTime = 450f; // 7.5 minutes in seconds

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            Color startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
            spriteRenderer.color = startColor;
        }
    }

    void Update()
    {
        float elapsedTime = Time.time;
        
        if (elapsedTime >= fadeStartTime && elapsedTime <= fadeEndTime)
        {
            float t = (elapsedTime - fadeStartTime) / fadeDuration; // Normalized time (0 to 1)
            float newAlpha = Mathf.Lerp(0, 120f / 255f, t);
            
            if (spriteRenderer != null)
            {
                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
                spriteRenderer.color = newColor;
            }
        }
    }
}
