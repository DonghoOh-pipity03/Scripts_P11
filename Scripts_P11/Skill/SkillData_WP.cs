using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_11
{
    [CreateAssetMenu(fileName = "SkillData_WP", menuName = "Scriptable Objects/SkillData WP", order = int.MaxValue)]
    public class SkillData_WP : ScriptableObject
    {
        [Header("공격")]
        [SerializeField] int damage = 10;
        public int Damage => damage;
        [SerializeField] float damageCoolTime = 0.1f;
        public float DamageCoolTime => damageCoolTime;
        [SerializeField] float wp_time = 5;   // 백린 연막 지속 시간
        public float WP_time => wp_time;
        [SerializeField] float deployCoolTime = 15;
        public float DeployCoolTime => deployCoolTime;
        [SerializeField] int deployCnt_perSide = 1;
        public int DeployCnt_perSide => deployCnt_perSide;
        
        [Header("발사")]
        [SerializeField] float minTriggerTime = 0.1f;
        public float MinTriggerTime => minTriggerTime;
        [SerializeField] float maxTriggerTime = 0.25f;
        public float MaxTriggerTime => maxTriggerTime;
        [SerializeField] int deployAngle_x = 15;
        public int DeployAngle_x => deployAngle_x;
        [SerializeField] int deployAngle_y = 15;
        public int DeployAngle_y => deployAngle_y;
        [SerializeField] float deplyPower = 60;
        public float DeplyPower => deplyPower;
        [SerializeField] int nextDeployAngle = 15;
        public int NextDeployAngle => nextDeployAngle;
        
        [Header("마스터 3")]
        [SerializeField] int explosionDamage = 25;
        public int ExplosionDamage => explosionDamage;
        [SerializeField] float explosionDist = 6;
        public float ExplosionDist => explosionDist;
    }
}