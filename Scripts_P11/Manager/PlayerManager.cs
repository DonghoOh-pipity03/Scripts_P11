using System;
using System.Collections;
using System.Collections.Generic;
using Project_11;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project_11
{
    // 무기 종류
    public enum WeaponKind
    {
        Impact,
        Weapon,
        Self_Explosion,
        Skill_Dron,
        Skill_Smoke,
        Skill_WP,
        Skill_APS
        // 추후 수정할 부분
    }

    // 데미지 종류
    public enum DamageKind
    {
        Single,
        Explosion,
        HeatExplosion
    }

    public class PlayerManager : MonoBehaviour
    {
        #region variable
        public Rigidbody rb;

        [Header("경험치")]
        int curEXP = 0;
        public int curMaxEXP = 100;
        int curLevel = 1;
        public int EXP_Step = 25;  // 레벨업을 할 때 마다, 경험치 통 증가량

        [Header("밸런스 - 체력")]
        public int maxHP = 100;

        [HideInInspector] public bool isDead = false; // 생사 여부

        [Header("밸런스 - 기동")]
        public int maxTotalTorque;   // 최대 전체 토크
        public int topSpeed;    // 최대 속력
        public float maxRPM = 1000.0f;
        public int breakingTorque;    // 브레이크 토크
        public float steerAngle;
        public float vehicleVelocity => rb.velocity.magnitude * 3.6f;

        [Header("밸런스 - 충돌")]
        //[SerializeField] public float impactDamageRatio;  // 충돌데미지 계수
        //[SerializeField] public float impactPhysicsRatio;  // 충돌물리 계수
        //public int impactDamage => (int)(vehicleVelocity * rb.mass * impactDamageRatio); // 충돌 데미지 = 현재속도 * 중량 * 충돌데미지 계수
        //public int impactForce => (int)(impactDamage * impactPhysicsRatio);// 충돌 전달힘 = 현재속도 * 중량 * 충돌데미지 계수 * 충돌물리 계수
        public int minVelocity_impact = 20; // 충돌데미지를 전달하기 위한, 최소 속도
        public int minAngle_impact = 30;    // 충돌 데미지를 전달하기 위한, 최소 각도
        public int impactDamage;    // 충돌시 적에게 가하는 데미지
        public int impactReverseForce;  // 충돌 시 감속되는 힘
        public int power_KnockBack = 10;
        public float sec_KnockBack = 2;
        public float coolTime_impact;

        [Header("밸런스 - 무기")]
        public DamageKind damageKind;   // 데미지 종류
        public float explosionDist; // 폭발형 데미지 범위
        public int weaponDamage;  // 데미지
        public float weaponRPM; // 무기 RPM
        public float weaponCooltime => 60 / weaponRPM;  // 무기 쿨타임
        public float weaponRotAngle_PerSec; // 무기 회전 속도 (각도/초)
        public float fireRange; // 사거리
        public float fireAngle; // 사격 범위의 각도
        public float VisualAccuracy;  // 시각적 명중률, 0에 가까울 수록 정확도가 높음

        [Header("애니메이션 - 무기")]
        public int weapon_maxVertical = 70;  // 비주얼 무기 최대 수직 각도
        public float weapon_rotationSpeed_H = 5;  // 비주얼 무기 회전 속도_수평
        public float weapon_rotationSpeed_V = 5;  // 비주얼 무기 회전 속도_수직

        [Header("애니메이션 - 사망 자폭")]
        public int selfExp_damage;  // 자폭 데미지
        public float selfExp_dist;  // 자폭 거리

        [Header("스킬")]



        [Header("개발 옵션")]
        public bool useImpactDamage = true;

        // 이벤트
        public event Action PlayerIsDead;
        #endregion


        #region Singleton
        // 싱글톤
        private static PlayerManager PM;
        public static PlayerManager singleton
        {
            get
            {
                if (PM == null)
                {
                    PM = FindObjectOfType<PlayerManager>();
                    if (PM == null) Debug.Log("PlayerManager를 사용하려 했지만, 없어요.");
                }
                return PM;
            }
        }
        #endregion

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PlayerManager.singleton.isDead) return;

            IItem item = other.GetComponent<IItem>();
            if (item != null) item.Use(this.gameObject);
        }


        // 플레이어가 적에게 공격할 때, 사용하는 방식
        public void SendDamage_P2E(Health _enemy, int _damage, WeaponKind _weaponKind, DamageKind _damageKind, float _explosionDist)
        {
            // 단일형
            if (_damageKind == DamageKind.Single)
            {
                _enemy.GetDamage(_damage, _weaponKind);
                GameManager.singleton.GetDamageData(_weaponKind, _damage);
            }
            // 폭발형
            else
            {
                int layerMask = ~LayerMask.GetMask("Player"); // "Player" 레이어를 제외한 마스크 생성
                Collider[] colliders = Physics.OverlapSphere(_enemy.transform.position, _explosionDist, layerMask);

                foreach (Collider collider in colliders)
                {
                    Health health = collider.GetComponent<Health>();
                    if (health != null)
                    {
                        health.GetDamage(_damage, _weaponKind);
                    }
                }

                // 이펙트 - 폭발
                GameObject obj = EffectManager.singleton.GetPooledObject(EffectKind.Explosion);
                if (obj != null)
                {
                    obj.transform.position = _enemy.transform.position + Vector3.up;
                    obj.transform.localScale = Vector3.one * _explosionDist;
                }
            }
        }
        // 임의 지정 폭발형
        public void SendDamage_P2E(Vector3 _pos, int _damage, WeaponKind _weaponKind, DamageKind _damageKind, float _explosionDist, int _knockbackPower, Vector3 _explosionPos)
        {
            // 단일형
            if (_damageKind == DamageKind.Single)
            {

            }
            // 고폭발형
            else if (_damageKind == DamageKind.Explosion)
            {
                int layerMask = ~LayerMask.GetMask("Player"); // "Player" 레이어를 제외한 마스크 생성
                Collider[] colliders = Physics.OverlapSphere(_pos, _explosionDist, layerMask);

                foreach (Collider collider in colliders)
                {
                    Health health = collider.GetComponent<Health>();
                    if (health != null)
                    {
                        health.GetDamage(_damage, _weaponKind);
                    }
                }

                // 이펙트 - 폭발
                GameObject obj = EffectManager.singleton.GetPooledObject(EffectKind.Explosion);
                if (obj != null)
                {
                    obj.transform.position = _pos + Vector3.up;
                    obj.transform.localScale = Vector3.one * _explosionDist;
                }
            }
            // 열압력형
            else if (_damageKind == DamageKind.HeatExplosion)
            {
                int layerMask = ~LayerMask.GetMask("Player"); // "Player" 레이어를 제외한 마스크 생성
                Collider[] colliders = Physics.OverlapSphere(_pos, _explosionDist, layerMask);

                // 공격 처리
                foreach (Collider collider in colliders)
                {
                    Health health = collider.GetComponent<Health>();
                    if (health != null)
                    {
                        health.GetDamage(_damage, _weaponKind);

                        // 넉백 처리
                        Health_Enemy HE = health.GetComponent<Health_Enemy>();
                        if (HE != null) HE.GetKnockBack(health.transform.position - _explosionPos, _knockbackPower, PlayerManager.singleton.sec_KnockBack);

                    }
                }

                // 이펙트 - 폭발
                GameObject obj = EffectManager.singleton.GetPooledObject(EffectKind.Explosion);
                if (obj != null)
                {
                    obj.transform.position = _pos + Vector3.up;
                    obj.transform.localScale = Vector3.one * _explosionDist;
                }
            }
        }

        public void PlayerDead()
        {
            isDead = true;

            PlayerIsDead?.Invoke();
        }

        public void Set_Weapon_Damage_Mul(float _mul)
        {
            weaponDamage = (int)(weaponDamage * _mul);
        }

        public void Set_Weapon_FireRange_direct(float _range)
        {
            fireRange = _range;
            Weapon.singleton.Set_FireRangeUI_Range(_range);
        }

        public void Set_Weapon_FireRange_Mul(float _mul)
        {
            fireRange *= _mul;
            Weapon.singleton.Set_FireRangeUI_Range(fireRange);
        }

        public void Set_Weapon_FireAngle(float _angle)
        {
            fireAngle = _angle;
            Weapon.singleton.Set_FireRangeUI_HalfAngle(_angle / 2);
        }

        // UI를 통해 1개의 스킬을 입력받고, 해당 스킬 클래스에 스킬 레벨업을 전달한다.
        public void SelectedSkill(int _skill) { }

        // UI를 통해 2개의 스킬을 입력받고, 해당 스킬 클래스에 관련 작업을 전달한다.
        public void SelectedSkill(int _skill_01, int _skill_02) { }


        public void GetEXP(int _exp)
        {
            curEXP += _exp;

            // 레벨업
            if(curEXP > curMaxEXP)
            {
                curEXP -= curMaxEXP;

                curMaxEXP += EXP_Step;

                curLevel++;
            }
        }
    }
}