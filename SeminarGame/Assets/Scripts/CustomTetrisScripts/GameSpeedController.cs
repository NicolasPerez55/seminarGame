using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    private float startTime;
    [SerializeField] private float slowStartTime = 360f; // 6 minutes
    [SerializeField] private float stopTime = 480f; // 8 minutes
    private float initialTimeScale = 1f;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;

        if (elapsedTime >= slowStartTime && elapsedTime <= stopTime)
        {
            float progress = (elapsedTime - slowStartTime) / (stopTime - slowStartTime);
            Time.timeScale = Mathf.Lerp(initialTimeScale, 0f, progress);
        }
        else if (elapsedTime > stopTime)
        {
            Time.timeScale = 0f; // Fully stop the game
        }
    }
}
