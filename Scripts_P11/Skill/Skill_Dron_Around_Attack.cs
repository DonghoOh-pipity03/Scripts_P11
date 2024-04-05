using System.Collections;
using System.Collections.Generic;
using Project_11;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Project_11
{
    public class Skill_Dron_Around_Attack : MonoBehaviour
    {
        [SerializeField] LayerMask enemyLayer; // Enemy 레이어를 검출할 레이어 마스크
        [SerializeField] float raycastDistance = 20f; // 레이 캐스트의 최대 거리
        int droneNum;   // 드론 번호
        float lastAttackTime;


        private void Awake()
        {
            // 델리게이트 등록, 이벤트가 작동하면 이 드론을 삭제함, 해당 클래스의 public void DestroyThis() 참고
            Skill_Dron.singleton.deleteDrone += DestroyThis;
        }

        void Update()
        {
            DriveAround();

            if (PlayerManager.singleton.isDead) return;
            Attack();
        }


        public void SetNum(int _num) => droneNum = _num;

        void DriveAround()
        {
            // 중심 오브젝트의 위치를 기준으로 회전 반지름을 고려하여 현재 오브젝트를 회전시킵니다.
            Vector3 newPos = PlayerManager.singleton.transform.position
                + Vector3.up * Skill_Dron.singleton.shootHeight
                - Quaternion.Euler(0, Skill_Dron.singleton.rotSpeed * Time.time + Skill_Dron.singleton.reviceAngle[droneNum], 0) * (Vector3.forward * Skill_Dron.singleton.rotRadius);
            transform.position = newPos;

            // 오브젝트의 방향을 회전 방향에 동기화한다.
            transform.rotation = Quaternion.Euler(0, Skill_Dron.singleton.rotSpeed * Time.time + Skill_Dron.singleton.reviceAngle[droneNum] + 95, 0);
        }
        void Attack()
        {
            if (Time.time < lastAttackTime + Skill_Dron.singleton.coolTime_aroundDrone) return;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, Skill_Dron.singleton.rayRadius, Vector3.down, out hit, raycastDistance, enemyLayer))
            {
                PlayerManager.singleton.SendDamage_P2E(hit.transform.GetComponent<Health>(), Skill_Dron.singleton.damage_aroundDrone, WeaponKind.Skill_Dron, DamageKind.Explosion, Skill_Dron.singleton.damageDist_aroundDrone);
                lastAttackTime = Time.time;
            }
        }

        public void DestroyThis() => Destroy(gameObject);
    }
}