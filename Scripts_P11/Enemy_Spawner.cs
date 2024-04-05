using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Project_11;
using System.Runtime.InteropServices;

namespace Project_11
{
    public class Enemy_Spawner : MonoBehaviour
    {
        [Header("오브젝트 풀")]
        public int poolSize = 10;
        private Queue<GameObject> objectQueue;

        [Header("스폰")]
        public float spawnDistance = 10.0f; // 플레이어와 적이 떨어진 거리
        public float spawnInterval = 5.0f; // 적 스폰 간격

        [Header("적")]
        public GameObject enemyPrefab; // 적 프리팹

        [Header("밸런스 - 적")]
        public float stopDistance = 1;    // 목표근처에서 멈추는, 목표까지의 거리
        public float angularSpeed = 240;    // 회전 속도
        public float acceleration = 20;    // 가감속 속도
        public float turnSmoothTime = 0.1f; // 회전의 부드러움
        public float attack_CoolTime = 1;
        [Space(10f)]
        public float runSpeed = 13f;
        public int maxHP = 100;
        public int damage = 5;
        [Space(10f)]
        public float orderCycle_AI = 0.1f;

        [Header("밸런스 - 적 - 레벨")]
        public int curLevelStep = 0; // 현재 레벨 상승 정도
        public int levelUp_GapTime = 1; // 레벨 상승 시간 간격
        public float speed_Up;
        public int hp_Up;
        public int damage_Up;
        [Space(10f)]
        public float orderCycle_Level = 1f;
        float lastOrderTime;


        #region Singleton
        // 싱글톤
        private static Enemy_Spawner ES;
        public static Enemy_Spawner singleton
        {
            get
            {
                if (ES == null)
                {
                    ES = FindObjectOfType<Enemy_Spawner>();
                    if (ES == null) Debug.Log("Enemy_Spawner 사용하려 했지만, 없어요.");
                }
                return ES;
            }
        }
        #endregion


        private void Awake()
        {
            // 오브젝트 풀링 초기 생성
            objectQueue = new Queue<GameObject>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(enemyPrefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }
        }

        private void Start()
        {
            // 시작하자마자 스폰 간격마다 스폰 메서드를 호출
            InvokeRepeating("SpawnEnemy", 0, spawnInterval);
        }

        private void Update()
        {
            // AI 레벨
            if (Time.time > lastOrderTime + orderCycle_Level)
            {
                // curLevelStep을 갱신
                int tarStep = GameManager.singleton.curGameTime_min / levelUp_GapTime;
                // 필요시 능력치 업
                if (tarStep != curLevelStep)
                {
                    runSpeed += speed_Up;
                    maxHP += hp_Up;
                    damage += damage_Up;

                    curLevelStep++;
                }
            }
        }


        // 오브젝트 풀에서 오브젝트를 가져오기
        public GameObject GetPooledObject()
        {
            if (objectQueue.Count == 0) CreateAndEnqueueObject();
            GameObject obj = objectQueue.Dequeue();
            return obj;
        }

        private void CreateAndEnqueueObject()
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }

        // 오브젝트 풀에 반납하기
        public void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }

        // 플레이어와 일정 거리에 떨어진 위치에서 적을 소환
        private void SpawnEnemy()
        {
            if (PlayerManager.singleton.isDead) return;

            // 위치 선정하기
            NavMeshHit hit;
            while (true)
            {
                Vector3 randomDirection = Vector3.zero;
                Vector2 randomDirection_2D = Random.insideUnitCircle.normalized;
                randomDirection.x = randomDirection_2D.x;
                randomDirection.z = randomDirection_2D.y;
                randomDirection *= spawnDistance;
                randomDirection += PlayerManager.singleton.transform.position;
                NavMesh.SamplePosition(randomDirection, out hit, 100, NavMesh.AllAreas);

                if (Vector3.Distance(hit.position, PlayerManager.singleton.transform.position) > spawnDistance - 5) break;
            }

            // 스폰하기
            GameObject obj = GetPooledObject();
            if (obj != null)
            {
                obj.SetActive(true);
                obj.GetComponent<Health_Enemy>().Setup(hit.position, runSpeed, stopDistance, angularSpeed, acceleration, turnSmoothTime,
                    maxHP, damage, attack_CoolTime, orderCycle_AI);
            }
        }
    }
}