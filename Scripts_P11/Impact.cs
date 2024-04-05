using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Impact : MonoBehaviour
    {   
        #region Singleton
        // 싱글톤
        private static Impact DI;
        public static Impact singleton
        {
            get
            {
                if (DI == null)
                {
                    DI = FindObjectOfType<Impact>();
                    if (DI == null) Debug.Log("Damage_Impact을 사용하려 했지만, 없어요.");
                }
                return DI;
            }
        }
        #endregion

        private void OnTriggerStay(Collider other)
        {
            // 개발 옵션
            if (!PlayerManager.singleton.useImpactDamage) return;

            // 장애물이나 적과 충돌한 경우
            if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
            {
                Health obstacle = other.GetComponent<Health>();
                if (obstacle != null)
                {
                    SendImpactDamage(obstacle);
                }
            }
        }

        // 충돌로 인한 데미지 전달
        void SendImpactDamage(Health _health)
        {
            // 충돌 조건 검사
            if(!IsAbleImpact(_health)) return;

            // 공격 쿨타임 계산
            bool canAttack = _health.CanDamaged(WeaponKind.Impact, PlayerManager.singleton.coolTime_impact);
            if (canAttack)
            {
                // 충돌 데미지 전달
                PlayerManager.singleton.SendDamage_P2E(_health, PlayerManager.singleton.impactDamage, WeaponKind.Impact, DamageKind.Single, 0);

                // 충돌로 인한 넉백
                Health_Enemy HE = _health.GetComponent<Health_Enemy>();
                var dir_enemy = (_health.transform.position - transform.position).normalized;
                if(HE != null) HE.GetKnockBack(dir_enemy, PlayerManager.singleton.power_KnockBack, PlayerManager.singleton.sec_KnockBack);
                    

                // 충돌로 인한 속도 감속
                VehicleControl.singleton.ForceBody(PlayerManager.singleton.impactReverseForce);
            }
        }

        // 충돌 조건이 되는지 리턴하는 함수
        public bool IsAbleImpact(Health _health)
        {   
            // 충분한 속도를 가지지 않은 경우, 리턴
            if (PlayerManager.singleton.minVelocity_impact > PlayerManager.singleton.vehicleVelocity) return false;

            // 차량이 대상을 친 것이 아니라면, 리턴
            // '차량 이동 방향'과 '차량과 적과의 단위 벡터'의 각도가 일정 각도 이하인 경우
            var dir_vehicle = VehicleControl.singleton.vehicle_Velocity.normalized;
            var dir_enemy = (_health.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(dir_vehicle, dir_enemy);
            if( angle > PlayerManager.singleton.minAngle_impact ) return false;
            else return true;
        }

    }
}