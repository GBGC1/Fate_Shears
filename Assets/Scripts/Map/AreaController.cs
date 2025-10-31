using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(Collider2D))]
    public class AreaController : MonoBehaviour
    {
        private enum AreaEnum
        {
            Area1,
            Area2,
            Area3,
            Area4,
            Area5,
            Area6
        }

        [SerializeField] private AreaEnum area;

        private static AreaEnum inPlayerArea = AreaEnum.Area1;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (area == inPlayerArea)
            {
                //StartSpawnEnemies();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                inPlayerArea = area;
                StartSpawnEnemies();
                //Debug.Log(inPlayerArea);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                StopSpawnEnemies();
            }
        }

        private void StartSpawnEnemies()
        {
            Debug.Log("StartSpawnEnemies");
            foreach(SpawnEnemy spawner in GetComponentsInChildren<SpawnEnemy>())
            {
                spawner.StartSpawnCoroutine();
            }
        }

        private void StopSpawnEnemies()
        {
            foreach(SpawnEnemy spawner in GetComponentsInChildren<SpawnEnemy>())
            {
                spawner.StopSpawn();
            }
        }
    }
}