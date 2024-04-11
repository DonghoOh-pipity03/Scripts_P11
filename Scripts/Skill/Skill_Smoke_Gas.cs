using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Skill_Smoke_Gas : MonoBehaviour
    {
        float leftTime;
        bool useSticky;

        private void Awake()
        {
            PlayerManager.singleton.PlayerIsDead += DestroyThis;    // 플레이어 사망시, 바로 사라지도록 하는 델리게이트
        }

        private void Update()
        {
            // 수명
            if (leftTime < 0) DestroyThis();
            leftTime -= Time.deltaTime;

            // 위치 설정
            if (useSticky)
            {
                transform.position = Skill_Smoke.singleton.SmokeDeployPos.position;
                transform.rotation = Skill_Smoke.singleton.SmokeDeployPos.rotation;
            }
        }

        public void SetInit(float _time, bool _useSticky)
        {
            leftTime = _time;
            useSticky = _useSticky;
        }

        void DestroyThis()
        {
            if (this.gameObject != null)
            {
                PlayerManager.singleton.PlayerIsDead -= DestroyThis;
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // 적과 겹친 경우
            if (other.CompareTag("Enemy"))
            {
                Health_Enemy enemy = other.GetComponent<Health_Enemy>();
                if (enemy != null)
                {
                    // 공격 쿨타임 계산
                    bool canAttack = enemy.CanDamaged(WeaponKind.Skill_Smoke, Skill_Smoke.singleton.damageCoolTime);

                    if (canAttack)
                    {
                        // 공격
                        PlayerManager.singleton.SendDamage_P2E(enemy, Skill_Smoke.singleton.damage, WeaponKind.Skill_Smoke, DamageKind.Single, 0);

                        // 이속 감소
                        enemy.GetSlowDown(Skill_Smoke.singleton.slowDownTime, Skill_Smoke.singleton.slowDownSpeed);
                    }
                }
            }
        }
    }
}