using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace Project_11
{
    enum APS_Type
    {
        Explosion,
        Shotgun,
        HeatExplosion
    }

    struct APS_info
    {
        public Vector3 _dir;
        public float time;
    }
    public class Skill_APS : Skill
    {
        [SerializeField] SkillData_APS data;

        [SerializeField] GameObject projectile_body; // 발사체
        [SerializeField] Transform deployPos_Left;
        [SerializeField] Transform deployPos_Right;

        APS_Type aps_Type;

        [Header("공격")]
        [HideInInspector] public int damage;
        [HideInInspector] public int knockbackPower;
        [HideInInspector] public float explosionDist;
        [HideInInspector] public float explosionReviceTime; // 적 위치에 도달하기 전에 폭발시키는 시간

        [HideInInspector] public float deployCoolTime;
        [HideInInspector] public float deployPower;

        [HideInInspector] public float detectDist;
        [HideInInspector] public float detectAngle;   // 한쪽이 아닌 전체 각도 


        [Header("마스터 3")]
        [HideInInspector] public int shootGunAngle;   // 산탄 사격 전체 각도


        Transform curTarget;
        float lastDeployTime_left;
        float lastDeployTime_right;
        bool useSkill_APS = false;


        #region Singleton
        // 싱글톤
        private static Skill_APS SA;
        public static Skill_APS singleton
        {
            get
            {
                if (SA == null)
                {
                    SA = FindObjectOfType<Skill_APS>();
                    if (SA == null) Debug.Log("Skill_APS를 사용하려 했지만, 없어요.");
                }
                return SA;
            }
        }
        #endregion

        private void Start() 
        {
            InitData();    
        }

        private void Update()
        {
            if(PlayerManager.singleton.isDead) return;
            
            WorkTurret();

            if(useSkill_APS)
            {
                GUIManager.singleton.Wait_APS_L(deployCoolTime -(Time.time - lastDeployTime_left));
                GUIManager.singleton.Wait_APS_R(deployCoolTime -(Time.time - lastDeployTime_right));
            }
        }


        public override void LevelUp()
        {
            curLevel++;

            if (curLevel == 1)
            {
                useSkill_APS = true;

                GUIManager.singleton.Active_APS_L();
                GUIManager.singleton.Active_APS_R();
            }
            else if (curLevel == 2)
            {
                // 사거리 증가
                detectDist += 5;
            }
            else if (curLevel == 3)
            {
                // 발사 쿨타임 감소
            }
            else if (curLevel == 4)
            {
                // 데미지 증가
            }
            else if (curLevel == 5)
            {
                // 폭발 범위 증가
                explosionDist += 1f;
            }
            else if (curLevel == 6)
            {
                // 발사 쿨타임 감소
                deployCoolTime -= 1;
            }
        }

        public override void Master_1()
        {
            // 샷건 방식 - 원뿔 내의 적에게 데미지 부여, 사거리 감소
            aps_Type = APS_Type.Shotgun;
            // 데미지
            damage += 50;
            // 사거리
            detectDist = 7;
            // 쿨타임
        }

        public override void Master_2()
        {
            // 열압력탄 - 넉백 크게 추가, 데미지 감소, 범위 감소
            aps_Type = APS_Type.HeatExplosion;
            damage /= 2;
            explosionDist -= 0.5f;
        }

        public override void Master_3()
        {
            // 고속 발사
            deployCoolTime /= 4;
        }

        public override void DisableSkill()
        {
            throw new System.NotImplementedException();
        }

        public override void Damage_Plus(int val)
        {
            damage += val;
        }

        void WorkTurret()
        {
            if (PlayerManager.singleton.isDead) return;
            if (!useSkill_APS) return;

            // 왼쪽 발사기 시퀀스
            if (Time.time > lastDeployTime_left + deployCoolTime)
            {
                // 목표 탐지_왼쪽
                Find_ClosestEnemy(deployPos_Left);

                // 목표에 발사_왼쪽
                if (curTarget != null)
                {
                    lastDeployTime_left = Time.time;
                    FireAPS(deployPos_Left);
                }
            }
            // 오른쪽 발사기 시퀀스
            if (Time.time > lastDeployTime_right + deployCoolTime)
            {
                // 목표 탐지_왼쪽
                Find_ClosestEnemy(deployPos_Right);

                // 목표에 발사_왼쪽
                if (curTarget != null)
                {
                    lastDeployTime_right = Time.time;
                    FireAPS(deployPos_Right);
                }
            }
        }

        void FireAPS(Transform _turret)
        {

            if (aps_Type != APS_Type.Shotgun)
            {   // 발사체 방식
                GameObject obj = Instantiate(projectile_body);

                // 방향 획득
                var dir = curTarget.position - _turret.position;

                // 시간 계산
                float explosionSec = dir.magnitude / deployPower;
                explosionSec -= explosionReviceTime;

                // 발사
                dir = dir.normalized;
                obj.transform.rotation = Quaternion.LookRotation(dir);
                obj.transform.position = _turret.position;
                obj.GetComponent<Rigidbody>().AddForce(dir * deployPower, ForceMode.Impulse);
                var projectile = obj.GetComponent<Projectile>();

                if (aps_Type == APS_Type.Explosion) projectile.SetUp_Skill_APS(explosionSec, ExplosionType.Explosion);
                else if (aps_Type == APS_Type.HeatExplosion) projectile.SetUp_Skill_APS(explosionSec, ExplosionType.HeatExplosion);


                /*  APS_info info = GetFireInfo(deployPos_Left.position);

                 obj.transform.rotation = Quaternion.LookRotation(info._dir);
                 obj.transform.position = deployPos_Left.position;

                 obj.GetComponent<Rigidbody>().AddForce(info._dir * deployPower, ForceMode.Impulse);

                 var projectile = obj.GetComponent<Projectile>(); 
                 if (aps_Type == APS_Type.Explosion) projectile.SetUp_Skill_APS(info.time, ExplosionType.Explosion);
                 else if( aps_Type == APS_Type.HeatExplosion) projectile.SetUp_Skill_APS(info.time, ExplosionType.HeatExplosion); */
            }
            else
            {   // 샷건 방식
                ShotGun(_turret);
            }

        }

        // 사격 범위 내의 가장 가까운 적을 찾아 저장한다.
        void Find_ClosestEnemy(Transform _trans)
        {
            curTarget = null;

            Collider[] colliders = Physics.OverlapSphere(_trans.position, detectDist, LayerMask.GetMask("Enemy"));
            float closestDistance = detectDist;

            foreach (var collider in colliders)
            {
                Transform newEnemyTransform = collider.transform;

                // 거리 계산
                float distance = Vector3.Distance(_trans.position, newEnemyTransform.position);

                // 가장 가까운 적 발견
                if (distance < closestDistance)
                {
                    // 적과의 방향벡터 구하기 - 월드 기준, y축은 사용안함
                    Vector3 targetDirection = newEnemyTransform.position - _trans.position;
                    targetDirection.y = 0;

                    // 무기의 방향벡터 구하기 - 월드 기준, y축은 사용안함 
                    Vector3 weaponDirection = _trans.transform.forward;
                    weaponDirection.y = 0;

                    // 적과 무기 사이의 각도
                    float angleToTarget = Vector3.Angle(targetDirection, weaponDirection);

                    // 사격 범위 내에 있을 경우
                    if (angleToTarget < detectAngle / 2)
                    {
                        // closestObject에 가장 가까운 물체가 저장된다
                        curTarget = newEnemyTransform;
                        closestDistance = distance;
                    }
                }
            }
        }

        // 샷건 발사
        // 사거리 내의 적들을 가져와서, 현재 타겟과의 각도를 기준으로, 사격 범위 내의 적들에게 피해를 준다.
        void ShotGun(Transform _turret)
        {
            // 현재 타겟과의 방향 벡터 구하기 - 월드 기준, y축은 사용안함
            Vector3 curTargetDirection = curTarget.position - _turret.position;
            curTargetDirection.y = 0;

            int layer1 = 1 << LayerMask.NameToLayer("Enemy");
            int layer2 = 1 << LayerMask.NameToLayer("Obstacle");
            int twoLayersMask = layer1 | layer2;

            // 터렛 기준의 적과 장애물 가져오기
            Collider[] colliders = Physics.OverlapSphere(_turret.position, detectDist, twoLayersMask);
            foreach (var collider in colliders)
            {
                Transform target = collider.transform;

                // 대상과의 방향벡터 구하기 - 월드 기준, y축은 사용안함
                Vector3 targetDirection = target.position - _turret.position;
                targetDirection.y = 0;

                // 적과 무기 사이의 각도
                float angleToTarget = Vector3.Angle(targetDirection, curTargetDirection);

                // 사격 범위 내의 적들이나 장애물에 피해를 주기
                if (angleToTarget < shootGunAngle / 2)
                {
                    // 공격
                    Health_Enemy enemy = target.GetComponent<Health_Enemy>();

                    // 적인 경우
                    if (enemy != null) PlayerManager.singleton.SendDamage_P2E(enemy, damage, WeaponKind.Skill_APS, DamageKind.Single, 0);

                    // 장애물인 경우

                }
            }
        }

        // 발사 위치를 받아서, 현재 타겟의 이동속도와 발사체의 속도를 고려해서, 
        // 최단시간에 서로 만날 위치와 시간을 반환환다.
        APS_info GetFireInfo(Vector3 _shootPos)
        {
            Vector3 targetSpeed = curTarget.GetComponent<Rigidbody>().velocity;

            float A = Mathf.Pow(targetSpeed.x, 2) + Mathf.Pow(targetSpeed.y, 2) + Mathf.Pow(targetSpeed.z, 2) - Mathf.Pow(deployPower, 2);
            float B = 2 * (targetSpeed.x * curTarget.position.x + targetSpeed.y * curTarget.position.y + targetSpeed.z * curTarget.position.z
                            - _shootPos.x * deployPower - _shootPos.y * deployPower - _shootPos.z * deployPower);
            float C = Mathf.Pow(curTarget.position.x, 2) + Mathf.Pow(curTarget.position.y, 2) + Mathf.Pow(curTarget.position.z, 2);

            float t1 = (-B + Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
            float t2 = (-B - Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
            float t = Mathf.Max(t1, t2);
            Vector3 contactPos = new Vector3(curTarget.position.x + targetSpeed.x * t, curTarget.position.y + targetSpeed.y * t, curTarget.position.z + targetSpeed.z * t);
            Vector3 APS_dir = (contactPos - _shootPos).normalized;

            APS_info val;
            val._dir = APS_dir;
            val.time = t;

            //Debug.Log(val._dir);
            //Debug.Log(val.time);
            return val;
        }

        void InitData()
        {
            damage = data.Damage;
            knockbackPower = data.KnockbackPower;
            explosionDist = data.ExplosionDist;
            explosionReviceTime = data.ExplosionReviceTime;

            deployCoolTime = data.DeployCoolTime;
            deployPower = data.DeployPower;

            detectDist = data.DetectDist;
            detectAngle = data.DetectAngle;

            shootGunAngle = data.ShootGunAngle;
        }
    }
}
