using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;
using UnityEngine.AI;

public class ZombieSpawner_Dev : MonoBehaviour
{
    public GameObject zombie;

    [Header("오브젝트 풀")]
    public int poolSize = 10;
    private Queue<GameObject> objectQueue;

    [Header("스폰")]
    public Transform player; // 플레이어의 Transform 컴포넌트
    public float spawnDistance = 10.0f; // 플레이어와 적이 떨어진 거리
    public float spawnInterval = 5.0f; // 적 스폰 간격

    [Header("적")]
    public GameObject enemyPrefab; // 적 프리팹

    [Header("밸런스 - 적")]
    public float runSpeed = 13f;
    public float stopDistance = 1;    // 목표근처에서 멈추는, 목표까지의 거리
    public float angularSpeed = 240;    // 회전 속도
    public float acceleration = 20;    // 가감속 속도
    public float turnSmoothTime = 0.1f; // 회전의 부드러움
    [Space(5f)]
    public int maxHP = 100;
    public int damage = 5;
    public float attack_CoolTime = 1;
    [Space(5f)]
    public float orderCycle = 0.1f;


    void Start()
    {
        SpawnEnemy();
    }

    // 플레이어와 일정 거리에 떨어진 위치에서 적을 소환
    private void SpawnEnemy()
    {
        if (PlayerManager.singleton.isDead) return;

        // 위치 선정하기
        Vector3 randomDirection = Vector3.zero;
        Vector2 randomDirection_2D = Random.insideUnitCircle.normalized;
        randomDirection.x = randomDirection_2D.x;
        randomDirection.z = randomDirection_2D.y;
        randomDirection *= spawnDistance;
        randomDirection += player.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 100, NavMesh.AllAreas);
        Vector3 randomPosition = hit.position;

        // 스폰하기
        GameObject obj = Instantiate(zombie);
        if (obj != null)
        {
            obj.SetActive(true);
            obj.GetComponent<Health_Enemy>().Setup(hit.position, runSpeed, stopDistance, angularSpeed, acceleration, turnSmoothTime,
                maxHP, damage, attack_CoolTime, orderCycle);
        }
    }
}
