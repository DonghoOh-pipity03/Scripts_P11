using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Project_11;
using Unity.AI.Navigation;

namespace Project_11
{
    public class ItemManager : MonoBehaviour
    {
        public NavMeshSurface navMeshSurface;

        // 체력
        public int heal_val = 50;
        public int heal_spawnGapTime = 15;
        [SerializeField] GameObject heal_obj;
        float lastSpawnTime_heal;

        // 경험치
        public int exp_val = 5;
        [SerializeField] GameObject exp_obj;

        // 자석
        public float initialSpeed = 1;
        public float accelerationSpeed = 1;
        public float maxSpeed = 10;
        public int magnetic_spawnGapTime = 15;
        [SerializeField] GameObject magnetic_obj;
        float lastSpawnTime_magnetic;

        #region Singleton
        // 싱글톤
        private static ItemManager IM;
        public static ItemManager singleton
        {
            get
            {
                if (IM == null)
                {
                    IM = FindObjectOfType<ItemManager>();
                    if (IM == null) Debug.Log("ItemManager를 사용하려 했지만, 없어요.");
                }
                return IM;
            }
        }
        #endregion

        private void Start()
        {
            //navMeshSurface = FindObjectOfType<NavMeshSurface>();
        }

        private void Update()
        {
            Spawn_Heal();
            Spawn_Magnetic();
        }

        private void Spawn_Heal()
        {
            if (Time.time < lastSpawnTime_heal + heal_spawnGapTime) return;
            if (PlayerManager.singleton.isDead) return;

            lastSpawnTime_heal = Time.time;

            Vector3 spawnPos = GetRandomNavMeshPosition();
            spawnPos += Vector3.up * 1.5f;

            Instantiate(heal_obj, spawnPos, Quaternion.identity);

            // 생성된 아이템을 5초 뒤에 파괴
            // Destroy(item, 5f);
        }

        private void Spawn_Magnetic()
        {
            if (Time.time < lastSpawnTime_magnetic + magnetic_spawnGapTime) return;
            if (PlayerManager.singleton.isDead) return;

            lastSpawnTime_magnetic = Time.time;

            Vector3 spawnPos = GetRandomNavMeshPosition();
            spawnPos += Vector3.up * 1.5f;

            Instantiate(magnetic_obj, spawnPos, Quaternion.identity);

            // 생성된 아이템을 5초 뒤에 파괴
            // Destroy(item, 5f);
        }

        Vector3 GetRandomNavMeshPosition()
        {
            NavMeshHit hit;
            Vector3 randomPosition = Vector3.zero;
            int radius = 10;

            // NavMesh 상에서 유효한 위치로 랜덤하게 샘플링
            if (NavMesh.SamplePosition(RandomPointInCircle(-100, 100, -100, 100), out hit, radius, NavMesh.AllAreas))
            {
                // 만약 샘플링된 위치에 NavMesh Obstacle이 있다면 다시 샘플링
                if (hit.hit)
                {
                    randomPosition = hit.position;
                }
                else
                {
                    // NavMesh Obstacle이 있는 경우에는 다시 샘플링
                    randomPosition = GetRandomNavMeshPosition();
                }
            }

            return randomPosition;
        }

        Vector3 RandomPointInCircle(int _minX, int _maxX, int _minZ, int _maxZ)
        {
            // 원 안의 랜덤한 위치를 생성
            int x = Random.Range(_minX, _maxX);
            int z = Random.Range(_minZ, _maxZ);
            return new Vector3(x, 0, z);
        }
    }
}