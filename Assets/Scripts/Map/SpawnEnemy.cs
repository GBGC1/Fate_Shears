using System.Collections;
using Script.Manager.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Map
{
    public class SpawnEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        private float spawnIntervalTo = 7;
        private float spawnIntervalFrom = 10;
        [SerializeField] private int maxEnemyCount = 5;

        private bool isSpawning = false;
        private Collider2D collider2d;
        private int currentEnemyCount;
        private Rect cameraRect;

        void Awake()
        {
            collider2d = GetComponent<Collider2D>();
            currentEnemyCount = 0;
        }

        void OnEnable()
        {
            EventBus.Instance().Subscribe<CameraRectEventData>(OnCameraRect);
        }

        public void StartSpawnCoroutine()
        {
            while (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(TrySpawn());
            }
        }

        private IEnumerator TrySpawn()
        {
            while (isSpawning)
            {
                Vector2 spawnPos = RandomPos();

                if (currentEnemyCount < maxEnemyCount && !cameraRect.Contains(spawnPos))
                {
                    Spawn(spawnPos);
                }

                yield return new WaitForSeconds(Random.Range(spawnIntervalTo, spawnIntervalFrom));
            }
        }

        private void Spawn(Vector2 spawnPos)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
            enemy.name = enemy.name.Substring(0, "Enemy".Length);
            StartCoroutine(enemy.GetComponentInChildren<EnemyStateController>().EnemyAppearance());
            currentEnemyCount++;
        }

        public Vector2 RandomPos()
        {
            Bounds bounds = collider2d.bounds;
            float locationX = Random.Range(bounds.min.x + 1, bounds.max.x - 1);
            float locationY = bounds.max.y + 1;

            return new Vector2(locationX, locationY);
        }

        private void OnCameraRect(CameraRectEventData @event)
        {
            cameraRect = @event.cameraRect;
        }


        public void StopSpawn()
        {
            isSpawning = false;
        }
    }
}