using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;
using UnityEngine.UIElements;

namespace Project_11
{
    public class Skill_WP : Skill
    {
        [SerializeField] SkillData_WP data;
        [SerializeField] GameObject projectile_body;    // 백린 연막탄
        [SerializeField] Transform deployPos_Left;  // 방향은 포탑의 발사기 로컬지역에서, 방향회전없이 
        [SerializeField] Transform deployPos_Right;

        // 공격 
        [HideInInspector] public int damage;
        [HideInInspector] public float damageCoolTime;
        [HideInInspector] public float wp_time;   // 백린 연막 지속 시간
        [HideInInspector] public float deployCoolTime;
         public int deployCnt_perSide;

        // 발사
        [HideInInspector] public float minTriggerTime;
        [HideInInspector] public float maxTriggerTime;
        [HideInInspector] public int deployAngle_x;
        [HideInInspector] public int deployAngle_y;
        [HideInInspector] public float deplyPower;
        [HideInInspector] public int nextDeployAngle;

        // 마스터 3
        [HideInInspector] public int explosionDamage;
        [HideInInspector] public float explosionDist;


        float lastDeployTime;
        bool useProjectile_WP = false;
        bool useExplosionProjectile = false;


        #region Singleton
        // 싱글톤
        private static Skill_WP SW;
        public static Skill_WP singleton
        {
            get
            {
                if (SW == null)
                {
                    SW = FindObjectOfType<Skill_WP>();
                    if (SW == null) Debug.Log("Skill_WP를 사용하려 했지만, 없어요.");
                }
                return SW;
            }
        }
        #endregion

        private void OnEnable()
        {
            InitData();
        }
        private void Update()
        {
            if (PlayerManager.singleton.isDead) return;

            DeployWP();

            if(useProjectile_WP || useExplosionProjectile) GUIManager.singleton.Wait_WP(deployCoolTime - (Time.time - lastDeployTime));
        }


        public override void LevelUp()
        {
            curLevel++;

            if (curLevel == 1)
            {
                // 2개 발사
                useProjectile_WP = true;

                GUIManager.singleton.Active_WP();
            }
            else if (curLevel == 2)
            {
                // 발사 쿨타임 감소
                deployCoolTime -= 2;
            }
            else if (curLevel == 3)
            {
                // 4개 발사
                deployCnt_perSide += 1;
            }
            else if (curLevel == 4)
            {
                // 지속시간?범위?데미지 증가

            }
            else if (curLevel == 5)
            {
                // 6개 발사
                deployCnt_perSide += 1;
            }
            else if (curLevel == 6)
            {
                // 지속시간?범위?데미지 증가

            }
        }

        public override void Master_1()
        {
            // 10개 발사
            deployCnt_perSide = 5;
        }

        public override void Master_2()
        {
            // 2개씩 연속 발사
            deployCnt_perSide = 1;
            deployCoolTime = 1.5f;
        }

        public override void Master_3()
        {
            // 고폭 유탄으로 변경
            useExplosionProjectile = true;
            // 데미지
            damage = explosionDamage;
        }



        public override void DisableSkill()
        {
            throw new System.NotImplementedException();
        }

        public override void Damage_Plus(int val)
        {
            damage += (int)(val / 5);
        }

        void DeployWP()
        {
            if (!useProjectile_WP) return;

            if (Time.time > lastDeployTime + deployCoolTime)
            {
                // 왼쪽 발사
                for (int i = 0; i < deployCnt_perSide; i++)
                {
                    // WP 생성
                    var wp = Instantiate(projectile_body);

                    // WP 위치, 방향 설정
                    wp.transform.position = deployPos_Left.position;

                    Vector3 tarEuler = deployPos_Left.eulerAngles;
                    tarEuler.x -= deployAngle_x;
                    tarEuler.y -= i * deployAngle_y;

                    Quaternion targetRotation = Quaternion.Euler(tarEuler);
                    wp.transform.rotation = targetRotation;

                    // WP 세팅 및 발사
                    wp.GetComponent<Rigidbody>().AddForce(wp.transform.forward * deplyPower, ForceMode.Impulse);
                    
                    var projectile = wp.GetComponent<Projectile>();
                    projectile.SetUp_Skill_WP();
                    if (useExplosionProjectile) projectile.explosionType = ExplosionType.Explosion;
                }

                // 오른쪽 발사
                for (int i = 0; i < deployCnt_perSide; i++)
                {
                    // WP 생성
                    var wp = Instantiate(projectile_body);

                    // WP 위치, 방향 설정
                    wp.transform.position = deployPos_Right.position;

                    Vector3 tarEuler = deployPos_Right.eulerAngles;
                    tarEuler.x -= deployAngle_x;
                    tarEuler.y += i * deployAngle_y;

                    Quaternion targetRotation = Quaternion.Euler(tarEuler);
                    wp.transform.rotation = targetRotation;

                    // WP 발사
                    wp.GetComponent<Rigidbody>().AddForce(wp.transform.forward * deplyPower, ForceMode.Impulse);
                    
                    var projectile = wp.GetComponent<Projectile>();
                    projectile.SetUp_Skill_WP();
                    if (useExplosionProjectile) projectile.explosionType = ExplosionType.Explosion;
                }
                lastDeployTime = Time.time;
            }
        }

        void InitData()
        {
            damage = data.Damage;
            damageCoolTime = data.DamageCoolTime;
            wp_time = data.WP_time;
            deployCoolTime = data.DeployCoolTime;
            deployCnt_perSide = data.DeployCnt_perSide;

            minTriggerTime = data.MinTriggerTime;
            maxTriggerTime = data.MaxTriggerTime;
            deployAngle_x = data.DeployAngle_x;
            deployAngle_y = data.DeployAngle_y;
            deplyPower = data.DeplyPower;
            nextDeployAngle = data.NextDeployAngle;

            explosionDamage = data.ExplosionDamage;
            explosionDist = data.ExplosionDist;
        }
    }
}