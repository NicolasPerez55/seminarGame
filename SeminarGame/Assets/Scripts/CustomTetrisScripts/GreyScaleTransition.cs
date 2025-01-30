using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class GreyscaleTransition : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;

    private float transitionStartTime;
    private bool isTransitioning = false;
    private float transitionDuration = 120f; // 2 minutes (from 6 to 8 minutes)

    void Start()
    {
        // Get the Color Grading effect from the PostProcessVolume
        postProcessVolume.profile.TryGetSettings(out colorGrading);

        // Ensure saturation starts at 0 (fully colored)
        colorGrading.saturation.value = 0;

        // Start the greyscale transition after 6 minutes
        Invoke("StartGreyscaleTransition", 300f); // 360 seconds = 6 minutes
    }

    void Update()
    {
        if (isTransitioning)
        {
            // Calculate how far we are into the transition
            float elapsed = Time.time - transitionStartTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration); // Normalize between 0 and 1

            // Lerp saturation from 0 to -100 (full greyscale)
            colorGrading.saturation.value = Mathf.Lerp(0, -100, t);
        }
    }

    void StartGreyscaleTransition()
    {
        isTransitioning = true;
        transitionStartTime = Time.time; // Mark the time when the transition starts
    }
}
