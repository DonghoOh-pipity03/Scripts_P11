using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Skill_WP_Floor : MonoBehaviour
    {
        float leftTime;

        private void OnEnable() => leftTime = Skill_WP.singleton.wp_time;

        private void Update()
        {
            leftTime -= Time.deltaTime;
            if (leftTime < 0) Destroy(this.gameObject);
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
                    bool canAttack = enemy.CanDamaged(WeaponKind.Skill_WP, Skill_WP.singleton.damageCoolTime);

                    // 공격
                    if (canAttack) PlayerManager.singleton.SendDamage_P2E(enemy, Skill_WP.singleton.damage, WeaponKind.Skill_WP, DamageKind.Single, 0);
                }
            }
        }
    }
}