using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ObstacleType
    {
        public GameObject prefab;
        public float yPosition;
    }

    public ObstacleType[] obstacles;
    public float spawnX = 10f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= GameDifficultyManager.CurrentSpawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        if (obstacles.Length == 0) return;

        int index = Random.Range(0, obstacles.Length);
        GameObject obstacle = Instantiate(
            obstacles[index].prefab,
            new Vector3(spawnX, obstacles[index].yPosition, 0),
            Quaternion.identity
        );

        obstacle.AddComponent<ObstacleMover>();
    }
}
