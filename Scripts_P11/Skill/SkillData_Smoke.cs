using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_11
{
    [CreateAssetMenu(fileName = "SkillData_Smoke", menuName = "Scriptable Objects/SkillData Smoke", order = int.MaxValue)]
    public class SkillData_Smoke : ScriptableObject
    {
        [Header("연막 공통")]
        [SerializeField] int damage = 75;
        public int Damage => damage;
        [SerializeField] float damageCoolTime = 2;
        public float DamageCoolTime => damageCoolTime;
        [SerializeField] float slowDownSpeed = 0.5f;
        public float SlowDownSpeed => slowDownSpeed;
        [SerializeField] float slowDownTime = 2;
        public float SlowDownTime => slowDownTime;
        [SerializeField] float deployTime = 3;
        public float DeployTime => deployTime;
        [SerializeField] float deployCoolTime = 10;
        public float DeployCoolTime => deployCoolTime;

        [Header("표류 공통")]
        [SerializeField] int delpoyCount= 5;
        public int DelpoyCount => delpoyCount;
        [SerializeField] float deployGapTime = 0.5f;
        public float DeployGapTime => deployGapTime;
    }
}