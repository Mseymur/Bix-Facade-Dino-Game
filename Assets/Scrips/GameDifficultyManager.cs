using UnityEngine;

public class GameDifficultyManager : MonoBehaviour
{
    public static float CurrentSpeed = 3f;
    public static float CurrentSpawnInterval = 1.5f;

    public float startSpeed = 3f;
    public float maxSpeed = 8f;

    public float startInterval = 1.5f;
    public float minInterval = 0.8f;

    public float speedUpStartTime = 90f;
    public float speedUpDuration = 60f;

    private float elapsedTime = 0f;

    void Start()
    {
        CurrentSpeed = startSpeed;
        CurrentSpawnInterval = startInterval;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= speedUpStartTime)
        {
            float t = Mathf.InverseLerp(speedUpStartTime, speedUpStartTime + speedUpDuration, elapsedTime);
            CurrentSpeed = Mathf.Lerp(startSpeed, maxSpeed, t);
            CurrentSpawnInterval = Mathf.Lerp(startInterval, minInterval, t);
        }
    }
}
