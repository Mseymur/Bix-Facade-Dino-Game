using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector2.left * GameDifficultyManager.CurrentSpeed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
