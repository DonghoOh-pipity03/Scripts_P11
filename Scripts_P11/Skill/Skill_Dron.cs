using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;
using System;

namespace Project_11
{
    public class Skill_Dron : Skill
    {
        #region variable
        [SerializeField] SkillData_Dron data; // 스킬 밸런스 데이터
        [SerializeField] GameObject drone_around_attack;   // 배회 드롭 드론
        [SerializeField] GameObject drone_around_recon;   // 배회 정찰 드론
        [SerializeField] GameObject drone_suicide;  // 자폭 드론
        [SerializeField] Transform drone_suicide_shoot_transform;  // 자폭 드론 발사 위치

        [Header("밸런스 - 배회 공통 드론")]
        [HideInInspector] public float rotSpeed = 90.0f; // 회전 속도
        [HideInInspector] public float rotRadius = 7.0f; // 회전 반지름
        [HideInInspector] public float shootHeight = 8.0f; // 발사 비행 높이 (플레이어 높이 기준에서 더해짐)
        [HideInInspector] public Action deleteDrone;    // 드론 삭제 이벤트

        [Header("밸런스 - 배회 드롭 드론")]
        [HideInInspector] public int damage_aroundDrone = 50;
        [HideInInspector] public float damageDist_aroundDrone = 3;
        [HideInInspector] public float coolTime_aroundDrone = 3;
        [HideInInspector] public float rayRadius = 1.0f; // 탐지 구형 레이의 반지름
        [HideInInspector] public int[] reviceAngle;   // 여러 대의 드론이 있는 경우, 각 드론별 각도를 조정해 주는 변수

        [Header("밸런스 - 마스터1: 박격포 드롭 드론")]
        [HideInInspector] int damage_mortarDrone = 300;
        [HideInInspector] int damageDist_mortarDrone = 6;
        [HideInInspector] float coolTime_mortarDrone = 3;
        [HideInInspector] float rotRadius_mortarDrone = 7.0f;
        [HideInInspector] int rotSpeed_mortarDrone = 75;
        [HideInInspector] float rayRadius_mortarDrone = 2;


        [Header("밸런스 - 마스터3: 자폭 드론")]
        [HideInInspector] public int damage_SuicideDrone;
        [HideInInspector] public float damageDist_SuicideDrone;
        [HideInInspector] float maxAttackDist_SuicideDrone;
        [HideInInspector] float cooltime_SuicideDrone;
        [SerializeField] LayerMask enemyLayer;
        bool useSuicideDrone = false;
        float lastAttackTime_SuicideDrone;
        #endregion


        #region Singleton
        // 싱글톤
        private static Skill_Dron SD;
        public static Skill_Dron singleton
        {
            get
            {
                if (SD == null)
                {
                    SD = FindObjectOfType<Skill_Dron>();
                    if (SD == null) Debug.Log("Skill_Dron를 사용하려 했지만, 없어요.");
                }
                return SD;
            }
        }
        #endregion


        #region LifeFunction
        private void Awake()
        {   
            InitData();

            reviceAngle = new int[3] { 0, 0, 0 };
        }

        private void Update()
        {
            if (useSuicideDrone) SuicideDroneMaster();
        }
        #endregion


        #region LevelUp
        public override void LevelUp()
        {
            curLevel++;

            if (curLevel == 1)
            {
                // 드론 생성
                var obj = Instantiate(drone_around_attack);
                obj.SetActive(true);
                obj.GetComponent<Skill_Dron_Around_Attack>().SetNum(0);
            }
            else if (curLevel == 2)
            {
                // 회전 속도 증가
                rotSpeed = 115;
                // 쿨타임 감소
                coolTime_aroundDrone = 2;
            }
            else if (curLevel == 3)
            {
                // 드론 생성
                var obj = Instantiate(drone_around_attack);
                obj.SetActive(true);
                obj.GetComponent<Skill_Dron_Around_Attack>().SetNum(1);

                // 드론 간격 조정
                reviceAngle[1] = 180;
            }
            else if (curLevel == 4)
            {
                // 활동 범위 증가
                rotRadius = 10;
            }
            else if (curLevel == 5)
            {
                // 범위 증가
                damageDist_aroundDrone = 4;
            }
            else if (curLevel == 6)
            {
                // 드론 생성
                var obj = Instantiate(drone_around_attack);
                obj.SetActive(true);
                obj.GetComponent<Skill_Dron_Around_Attack>().SetNum(2);

                // 드론 간격 조정
                reviceAngle[1] = 120;
                reviceAngle[2] = 240;
            }
        }

        public override void Master_1()
        {
            // 박격포탄 - 데미지/범위 증가
            // 랜덤 광역 딜 / 사각지대 큼
            damage_aroundDrone = damage_mortarDrone;
            damageDist_aroundDrone = damageDist_mortarDrone;
            coolTime_aroundDrone = coolTime_mortarDrone;
            rotRadius = rotRadius_mortarDrone;
            rotSpeed = rotSpeed_mortarDrone;
            rayRadius = rayRadius_mortarDrone;

        }

        public override void Master_2()
        {
            // 정찰드론(송골매) - 시야/메인무기 범위 증가/메인무기 데미지 증가
            // 특수 유틸기 - 메인무기 특화
            // 모든 드론 삭제
            deleteDrone?.Invoke();

            // 정찰드론 생성
            var obj = Instantiate(drone_around_recon);
            obj.SetActive(true);

            // 메인무기 범위 2배 증가
            PlayerManager.singleton.Set_Weapon_FireRange_Mul(2);

            // 메인 무기 데미지 1.5.배 증가
            PlayerManager.singleton.Set_Weapon_Damage_Mul(1.5f);
        }

        public override void Master_3()
        {
            // 자폭드론 - 자폭방식/쿨타임 감소/빠른 공격
            // 핀 포인트 공격 / 딜량 적음
            deleteDrone?.Invoke();

            useSuicideDrone = true;
        }
        #endregion


        #region Skill_Function
        public void InitData()
        {
            rotSpeed = data.RotSpeed; // 회전 속도
            rotRadius = data.RotRadius; // 회전 반지름
            shootHeight = data.ShootHeight; // 발사 비행 높이 (플레이어 높이 기준에서 더해짐)

            damage_aroundDrone = data.Damage_aroundDrone;
            damageDist_aroundDrone = data.DamageDist_aroundDrone;
            coolTime_aroundDrone = data.CoolTime_aroundDrone;
            rayRadius = data.RayRadius; // 탐지 구형 레이의 반지름

            damage_mortarDrone = data.Damage_mortarDrone;
            damageDist_mortarDrone = data.DamageDist_mortarDrone;
            coolTime_mortarDrone = data.CoolTime_mortarDrone;
            rotRadius_mortarDrone = data.RotRadius_mortarDrone;
            rotSpeed_mortarDrone = data.RotSpeed_mortarDrone;
            rayRadius_mortarDrone = data.RayRadius_mortarDrone;

            damage_SuicideDrone = data.Damage_SuicideDrone;
            damageDist_SuicideDrone = data.DamageDist_SuicideDrone;
            maxAttackDist_SuicideDrone = data.MaxAttackDist_SuicideDrone;
            cooltime_SuicideDrone = data.Cooltime_SuicideDrone;
        }

        public override void DisableSkill() => deleteDrone?.Invoke();

        public override void Damage_Plus(int _val)
        {
            // 공격력 업
            damage_aroundDrone += _val;
            damage_mortarDrone += _val;
            damage_SuicideDrone += _val;
        }

        #endregion


        #region Function
        // 자폭드론 운용 함수
        void SuicideDroneMaster()
        {
            if (Time.time > lastAttackTime_SuicideDrone + cooltime_SuicideDrone && !PlayerManager.singleton.isDead)
            {
                // 타깃 선정
                var enemy = ClosestEnemy(maxAttackDist_SuicideDrone);
                if (enemy != null)
                {
                    // 드론 활성화
                    var obj = Instantiate(drone_suicide);
                    obj.transform.position = drone_suicide_shoot_transform.position;
                    obj.GetComponent<Skill_Drone_Suicide>().SetInit(drone_suicide_shoot_transform, enemy.transform);
                    obj.SetActive(true);

                    lastAttackTime_SuicideDrone = Time.time;
                }
            }
        }

        // 가장 가까운 적 찾기
        GameObject ClosestEnemy(float _maxDist)
        {
            GameObject curTarget = null;

            Collider[] colliders = Physics.OverlapSphere(transform.position, _maxDist, enemyLayer);
            float closestDistance = _maxDist;

            foreach (var collider in colliders)
            {
                Transform newEnemyTransform = collider.transform;

                // 거리 계산
                float distance = Vector3.Distance(transform.position, newEnemyTransform.position);

                // 가장 가까운 적 발견
                if (distance < closestDistance)
                {
                    // closestObject에 가장 가까운 물체가 저장된다
                    curTarget = newEnemyTransform.gameObject;
                    closestDistance = distance;
                }
            }
            return curTarget;
        }
        #endregion
    }
}