using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_11
{
    [CreateAssetMenu(fileName = "SkillData_APS", menuName = "Scriptable Objects/SkillData APS", order = int.MaxValue)]
    public class SkillData_APS : ScriptableObject
    {
        [Header("공격")]
        [SerializeField] int damage = 100;
        public int Damage => damage;
        [SerializeField] int knockbackPower = 40;
        public int KnockbackPower => knockbackPower;
        [SerializeField] float explosionDist = 2;
        public float ExplosionDist => explosionDist;
        [SerializeField] float explosionReviceTime = 0.01f; // 적 위치에 도달하기 전에 폭발시키는 시간
        public float ExplosionReviceTime => explosionReviceTime;

        [SerializeField] float deployCoolTime = 3;
        public float DeployCoolTime => deployCoolTime;
        [SerializeField] float deployPower = 100;
        public float DeployPower => deployPower;
        
        [SerializeField] float detectDist = 5;
        public float DetectDist => detectDist;
        [SerializeField] float detectAngle = 225;   // 한쪽이 아닌 전체 각도 
        public float DetectAngle => detectAngle; 
        

        [Header("마스터 3")]
        [SerializeField] int shootGunAngle = 90;   // 산탄 사격 전체 각도
        public int ShootGunAngle => shootGunAngle;
    }
}