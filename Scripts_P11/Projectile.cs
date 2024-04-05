using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public enum TriggerType
    {
        Timer,
        Impact
    }
    public enum ExplosionType
    {
        Explosion,
        HeatExplosion,
        WP
    }

    public class Projectile : MonoBehaviour
    {
        [Header("투사체 정의")]
        [SerializeField] TriggerType triggerType;   // 작동 규칙
        [SerializeField] public ExplosionType explosionType;   // 폭탄 종류

        [Header("폭발 이후 오브젝트")]
        [SerializeField] GameObject WP_Floor; // 바닥에 깔리는 백린 연막탄
        [SerializeField] GameObject WP_Smoke; // 공중에서 터지는 비주얼용 연막탄
        
        [Header("옵션")]
        [SerializeField] float reyDist;

        // 공격 정보
        float leftTiem;
        int damage;
        int knockbackPower;
        float explosionDist;
        WeaponKind weaponKind;

        private void Update()
        {
            if (triggerType == TriggerType.Timer)
            {
                leftTiem -= Time.deltaTime;
                if (leftTiem < 0) Explosion();
            }
        }

        // 폭발
        void Explosion()
        {
            if (explosionType == ExplosionType.WP)
            {   // 백린탄
                // 공격 - 아래 땅으로 레이를 발사해서, 땅에 백린 장판을 생성
                RaycastHit hitInfo;
                if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, reyDist, LayerMask.GetMask("Default")))
                {
                    var obj = Instantiate(WP_Floor);
                    obj.transform.position = hitInfo.point;
                }

                // 이펙트 - 공중 폭발
                var obj2 = Instantiate(WP_Smoke);
                obj2.transform.position = transform.position;
                obj2.GetComponent<Destroy_time>().leftTime = Skill_WP.singleton.wp_time;

                Destroy(this.gameObject);
            }
            else if (explosionType == ExplosionType.Explosion)
            {   // 고폭탄
                // 공격
                PlayerManager.singleton.SendDamage_P2E(transform.position, damage, weaponKind, DamageKind.Explosion, explosionDist, 0, Vector3.zero);
                Destroy(this.gameObject);
            }
            else if (explosionType == ExplosionType.HeatExplosion)
            {   // 열압력탄
                // 공격
                PlayerManager.singleton.SendDamage_P2E(transform.position, damage, weaponKind, DamageKind.HeatExplosion, explosionDist, knockbackPower, transform.position);
                Destroy(this.gameObject);
            }
        }

        // 초기 셋업
        public void SetUp_Skill_WP()
        {
            leftTiem = Random.Range(Skill_WP.singleton.minTriggerTime, Skill_WP.singleton.maxTriggerTime);
            damage = Skill_WP.singleton.damage;
            explosionDist = Skill_WP.singleton.explosionDist;
            weaponKind = WeaponKind.Skill_WP;
            explosionType = ExplosionType.WP;
        }

        public void SetUp_Skill_APS(float _TimerTime, ExplosionType _explosionType)
        {
            leftTiem = _TimerTime;
            explosionType = _explosionType;            

            damage = Skill_APS.singleton.damage;
            explosionDist = Skill_APS.singleton.explosionDist;
            triggerType = TriggerType.Timer;
            weaponKind = WeaponKind.Skill_APS;
            knockbackPower = Skill_APS.singleton.knockbackPower;
        }
    }
}