using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Project_11;

namespace Project_11
{
    public class Health_Enemy : Health
    {
        #region variable
        Rigidbody rb;
        NavMeshAgent agent;

        [Space(10f)]
        [Header("좀비")]
        // 이동
        float turnSmoothTime;
        float turnSmoothVelocity;

        // 공격
        int damage;
        bool isAttacking = false;
        float attack_CoolTime;
        float lastAttackTime = 0;

        // 상태 
        bool isStuned = false;  // 경직 상태인지
        bool haveToWakeUp = false;  // 경직 상태에서 일어나야 하는지, 에이전트 활성화 시도시 오류 방지용
        float leftSlowTime; // 느려짐에서 풀리기까지 남은 시간
        float originalSpeed;    // 느려지기 전에 원래 속도

        [Header("죽음")]
        [SerializeField] GameObject liveBody;
        [SerializeField] GameObject deadBody;

        [Header("경험치")]
        [SerializeField] GameObject item_exp;

        [Header("최적화")]
        float orderCycle;    // 길찾기 실행 주기
        float lastOrderTime;
        #endregion


        #region LifeFucntion
        // 처음 생성 되었을 때
        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
        }

        protected override void Update()
        {
            base.Update();

            // 명령 실행 주기
            if (Time.time <= lastOrderTime + orderCycle) return;
            lastOrderTime = Time.time;

            // 상태 이상 회복
            RecoverSlowDown();
            TryRecoverFromStun();

            // 살아있는 동안 + 행동 가능 상태이면, 실행
            if (isDead || PlayerManager.singleton.isDead || isStuned) return;

            // 경로를 갱신
            agent.SetDestination(Health_Player.singleton.transform.position);
        }

        private void FixedUpdate()
        {   
            // 스턴 상태인 경우, 아래로 떨어지는 힘을 크게 증가 - 날아가는 중에, 상태복귀로 agent 오류를 방지하기 위함
            if(isStuned) rb.AddForce(new Vector3(0, -10, 0), ForceMode.Acceleration);

            // 살아있는 동안 + 행동 가능 상태이면, 실행
            if (isDead || PlayerManager.singleton.isDead || isStuned) return;

            // 공격 로직 - 방향 설정
            if (isAttacking)
            {
                var lookRotation =
                    Quaternion.LookRotation(Health_Player.singleton.transform.position - transform.position, Vector3.up);
                var targetAngleY = lookRotation.eulerAngles.y;

                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY,
                                            ref turnSmoothVelocity, turnSmoothTime);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (isDead) return;

            // 공격 시퀀스로 전환
            if (other.gameObject.CompareTag("Player"))
            {
                isAttacking = true;

                // 공격 상태로 전환
                if (agent.isActiveAndEnabled)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }

                // 공격 가능 조건 검사
                if( Impact.singleton.IsAbleImpact(this) ) return;

                // 공격
                if (Time.time > lastAttackTime + attack_CoolTime)
                {
                    lastAttackTime = Time.time;
                    other.gameObject.GetComponent<Health>().GetDamage(damage, WeaponKind.Weapon);
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (isDead) return;

            // 추격 시퀀스로 전환
            if (other.gameObject.CompareTag("Player"))
            {
                if (agent.isActiveAndEnabled)
                {
                    isAttacking = false;
                    agent.isStopped = false;
                }
            }
        }
        #endregion

        // 시작시 사용을 위한 세팅
        public void Setup(Vector3 _pos, float _runSpeed, float _stopDistance, float _angularSpeed, float _acceleration, float _turnSmoothTime,
            int _health, int _damage, float _attack_Cooltime, float _orderCycle)
        {
            // 위치 세팅
            agent.enabled = false;
            transform.position = _pos;

            // 렌더러 세팅
            liveBody.SetActive(true);
            deadBody.SetActive(false);

            // 물리 세팅
            rb.isKinematic = false;
            col.enabled = true;

            // 상태 세팅
            isDead = false;
            isAttacking = false;
            haveToWakeUp = false;
            isStuned = false;

            // 체력 설정
            curHP = _health;

            // 공격 설정
            damage = _damage;
            attack_CoolTime = _attack_Cooltime;

            // 이동 설정
            turnSmoothTime = _turnSmoothTime;
            originalSpeed = _runSpeed;

            // 최적화 설정
            orderCycle = _orderCycle;

            // NavAgent 세팅
            agent.enabled = true;
            agent.speed = _runSpeed;
            agent.stoppingDistance = agent.radius + _stopDistance;
            agent.angularSpeed = _angularSpeed;
            agent.acceleration = _acceleration;
            agent.isStopped = false;
        }

        public override void GetDamage(int _damage, WeaponKind _weaponKind)
        {
            if (isDead) return;

            int initHP = curHP;

            curHP -= _damage;
            if (curHP <= 0)
            {
                curHP = 0;
                Dead();
            }

            GameManager.singleton.GetDamageData(_weaponKind, initHP - curHP);
        }

        public void GetKnockBack(Vector3 _dir, int _force, float _sec)
        {
            rb.AddForce(_dir.normalized * _force, ForceMode.Impulse);

            GetStun(_sec);
        }

        public void GetStun(float _sec)
        {
            if (!isDead) agent.enabled = false;

            isStuned = true;

            StartCoroutine(RecoverFromStun(_sec));
        }

        IEnumerator RecoverFromStun(float _sec)
        {
            yield return new WaitForSeconds(_sec);

            haveToWakeUp = true;
        }

        void TryRecoverFromStun()
        {
            if(!haveToWakeUp) return;
            if(isDead) return;

            NavMeshHit hit;
            bool onNavMesh = NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas);
            if (onNavMesh)
            {
                agent.enabled = true;
                isStuned = false;
                haveToWakeUp = false;

                isAttacking = false;
                agent.isStopped = false;
            }
        }

        public void GetSlowDown(float _time, float _spead)
        {
            leftSlowTime = _time;
            agent.speed = agent.speed - _spead;
        }

        public void RecoverSlowDown()
        {
            if(leftSlowTime < 0) return;
            leftSlowTime -= Time.deltaTime;
            if(leftSlowTime < 0) agent.speed = originalSpeed;
        }

        // 사망 처리
        protected override void Dead()
        {
            // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
            base.Dead();

            agent.enabled = false;
            rb.isKinematic = true;

            liveBody.SetActive(false);
            deadBody.SetActive(true);

            // Instantiate(item_exp, transform.position + Vector3.up * 0.5f, Quaternion.identity);

            StartCoroutine(InactiveDeadBody(10f));
        }

        IEnumerator InactiveDeadBody(float _sec)
        {
            yield return new WaitForSeconds(_sec);

            Enemy_Spawner.singleton.ReturnToPool(this.gameObject);
        }
    }
}