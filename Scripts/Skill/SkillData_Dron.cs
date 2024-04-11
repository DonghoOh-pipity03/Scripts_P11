using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_11
{
    [CreateAssetMenu(fileName = "SkillData_Dron", menuName = "Scriptable Objects/SkillData Dron", order = int.MaxValue)]
    public class SkillData_Dron : ScriptableObject
    {
        [Header("밸런스 - 배회 공통 드론")]
        [SerializeField] float rotSpeed = 90f; // 회전 속도
        public float RotSpeed => rotSpeed;
        [SerializeField] float rotRadius = 7f; // 회전 반지름
        public float RotRadius => rotRadius;


        [Header("애니메이션 - 배회 공통 드론")]
        [SerializeField] float shootHeight = 8.0f; // 발사 비행 높이 (플레이어 높이 기준에서 더해짐)
        public float ShootHeight => shootHeight;


        [Header("밸런스 - 배회 드롭 드론")]
        [SerializeField] int damage_aroundDrone = 100;
        public int Damage_aroundDrone => damage_aroundDrone;
        [SerializeField] float damageDist_aroundDrone = 3;
        public float DamageDist_aroundDrone => damageDist_aroundDrone;
        [SerializeField] float coolTime_aroundDrone = 3;
        public float CoolTime_aroundDrone => coolTime_aroundDrone;
        [SerializeField] float rayRadius = 1.0f; // 탐지 구형 레이의 반지름
        public float RayRadius => rayRadius;


        [Header("밸런스 - 마스터1: 박격포 드롭 드론")]
        [SerializeField] int damage_mortarDrone = 300;
        public int Damage_mortarDrone => damage_mortarDrone;
        [SerializeField] int damageDist_mortarDrone = 6;
        public int DamageDist_mortarDrone => damageDist_mortarDrone;
        [SerializeField] float coolTime_mortarDrone = 2.5f;
        public float CoolTime_mortarDrone => coolTime_mortarDrone;
        [SerializeField] float rotRadius_mortarDrone = 10.0f;
        public float RotRadius_mortarDrone => rotRadius_mortarDrone;
        [SerializeField] int rotSpeed_mortarDrone = 75;
        public int RotSpeed_mortarDrone => rotSpeed_mortarDrone;
        [SerializeField] float rayRadius_mortarDrone = 2.5f;
        public float RayRadius_mortarDrone => rayRadius_mortarDrone;


        [Header("밸런스 - 마스터3: 자폭 드론")]
        [SerializeField] int damage_SuicideDrone = 200;
        public int Damage_SuicideDrone => damage_SuicideDrone;
        [SerializeField] float damageDist_SuicideDrone = 4.5f;
        public float DamageDist_SuicideDrone => damageDist_SuicideDrone;
        [SerializeField] float maxAttackDist_SuicideDrone = 15;
        public float MaxAttackDist_SuicideDrone => maxAttackDist_SuicideDrone;
        [SerializeField] float cooltime_SuicideDrone = 1.5f;
        public float Cooltime_SuicideDrone => cooltime_SuicideDrone;
    }
}