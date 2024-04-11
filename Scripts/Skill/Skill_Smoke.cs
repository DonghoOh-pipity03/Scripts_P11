using System.Collections;
using System.Collections.Generic;
using System.Data;
using Project_11;
using UnityEngine;

namespace Project_11
{
    public class Skill_Smoke : Skill
    {
        [SerializeField] SkillData_Smoke data;
        [SerializeField] GameObject gasSmoke_stick;
        public Transform SmokeDeployPos;

        [Header("연막 공통")]
        [HideInInspector] public int damage;
        [HideInInspector] public float damageCoolTime;
        [HideInInspector] public float slowDownSpeed;
        [HideInInspector] public float slowDownTime;
        [HideInInspector] public float deployTime;
        [HideInInspector] public float deployCoolTime;

        [Header("표류 공통")]
        [HideInInspector] public int delpoyCount;
        [HideInInspector] public float deployGapTime;


        bool useSmoke_stick;
        bool useSmoke_drift;
        float lastDeplyTime;


        #region Singleton
        // 싱글톤
        private static Skill_Smoke SS;
        public static Skill_Smoke singleton
        {
            get
            {
                if (SS == null)
                {
                    SS = FindObjectOfType<Skill_Smoke>();
                    if (SS == null) Debug.Log("Skill_Smoke를 사용하려 했지만, 없어요.");
                }
                return SS;
            }
        }
        #endregion

        private void Awake()
        {
            InitData();
        }

        private void Update()
        {
            if (PlayerManager.singleton.isDead) return;
            
            DeploySmoke();

            // GUI
            if(useSmoke_drift || useSmoke_stick) GUIManager.singleton.Wait_Smoke(deployCoolTime - (Time.time - lastDeplyTime));
        }


        public override void LevelUp()
        {
            curLevel++;

            if (curLevel == 1)
            {
                // 가스 연막 활성화 - 간헐적 방출, 데미지 부여, 적 이동속도 감소
                useSmoke_stick = true;

                GUIManager.singleton.Active_Smoke();
            }
            else if (curLevel == 2)
            {
                // 지속시간 증가
                deployTime += 1;
            }
            else if (curLevel == 3)
            {
                // 데미지 증가
                damage += 0;
            }
            else if (curLevel == 4)
            {
                // 쿨타임 감소
                deployCoolTime -= 1;
            }
            else if (curLevel == 5)
            {
                // 지속시간 증가
                deployTime += 1;
            }
            else if (curLevel == 6)
            {
                // 데미지 증가
                damage += 0;
            }
        }

        public override void Master_1()
        {
            // 지속시간 증가
            deployTime += 2;
            // 쿨타임 감소
            deployCoolTime -= 2;
            // 크기 증가

        }

        public override void Master_2()
        {
            // 데미지 없음
            damage = 0;
            // 쿨타임 증가
            deployCoolTime += 3;
            // 이동속도 감소량 증가
            slowDownSpeed += 3;
        }

        public override void Master_3()
        {
            // 대기에 표류
            useSmoke_stick = false;
            useSmoke_drift = true;
        }


        public override void DisableSkill()
        {
            throw new System.NotImplementedException();
        }

        public override void Damage_Plus(int val)
        {
            damage += val;
        }


        // 연막 전개
        void DeploySmoke()
        {
            if (useSmoke_stick)
            {   // 고정 연막 생성
                if (Time.time > lastDeplyTime + deployCoolTime)
                {
                    var obj = Instantiate(gasSmoke_stick);
                    obj.GetComponent<Skill_Smoke_Gas>().SetInit(deployTime, true);

                    lastDeplyTime = Time.time;
                }
            }
            else if (useSmoke_drift)
            {   // 표류 연막 생성
                if (Time.time > lastDeplyTime + deployCoolTime)
                {
                    StartCoroutine(DeploySmoke(delpoyCount));

                    lastDeplyTime = Time.time;
                }
            }

        }

        IEnumerator DeploySmoke(int _cnt)
        {
            while (_cnt > 0)
            {
                var obj = Instantiate(gasSmoke_stick);
                obj.GetComponent<Skill_Smoke_Gas>().SetInit(deployTime, false);
                obj.transform.position = SmokeDeployPos.position;
                obj.transform.rotation = SmokeDeployPos.rotation;

                yield return new WaitForSeconds(deployGapTime);
                _cnt--;
            }
        }

        void InitData()
        {
            damage = data.Damage;
            damageCoolTime = data.DamageCoolTime;
            slowDownSpeed = data.SlowDownSpeed;
            slowDownTime = data.SlowDownTime;
            deployTime = data.DeployTime;
            deployCoolTime = data.DeployCoolTime;

            delpoyCount = data.DelpoyCount;
            deployGapTime = data.DeployGapTime;
        }
    }
}