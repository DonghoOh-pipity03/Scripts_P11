using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public abstract class Health : MonoBehaviour
    {
        protected Collider col;   // 모든 생명체는 콜라이더를 가진다. 죽을 때, 콜라이더를 삭제한다.

        [Header("체력")]
        [SerializeField] protected int maxHP = 100;
        protected int curHP;
        protected bool isDead = false;

        [SerializeField] Dictionary<WeaponKind, float> lastDamagedTime; // 마지막으로 공격받은 시간

        [Header("개발 옵션")]
        [SerializeField] bool controlHP = false;
        [SerializeField] int newHP;

        protected virtual void Awake()
        {
            col = GetComponent<Collider>();
            lastDamagedTime = new Dictionary<WeaponKind, float>();
        }

        protected virtual void OnEnable()
        {
            curHP = maxHP;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            SetHP();
#endif
        }

        public virtual void GetDamage(int _damage, WeaponKind _weaponKind)
        {
            if (isDead) return;

            curHP -= _damage;
            if (curHP <= 0) Dead();

        }

        public void GetHeal(int _Heal)
        {
            if (isDead) return;

            curHP += _Heal;
            if (curHP > maxHP) curHP = maxHP;
        }

        // 쿨타임이 되어서 공격을 받을 수 있는지 알려주는 함수
        public bool CanDamaged(WeaponKind _attackType, float _coolTime)
        {
            // 공격 받은 적이 있음
            if (lastDamagedTime.ContainsKey(_attackType))
            {
                if (Time.time > lastDamagedTime[_attackType] + _coolTime)
                {
                    lastDamagedTime[_attackType] = Time.time;
                    return true;
                }
                else return false;
            }
            // 공격 받은 적이 없음
            else
            {
                lastDamagedTime[_attackType] = Time.time;
                return true;
            }
        }

        protected void SelfExplosion(int _damage, float _dist)
        {
            int layerMask = ~LayerMask.GetMask("Player"); // "Player" 레이어를 제외한 마스크 생성
            Collider[] colliders = Physics.OverlapSphere(transform.position, _dist, layerMask);

            foreach (Collider collider in colliders)
            {
                Health health = collider.GetComponent<Health>();
                if (health != null)
                {
                    health.GetDamage(_damage, WeaponKind.Self_Explosion);
                    GameManager.singleton.GetDamageData(WeaponKind.Self_Explosion, _damage);
                }
            }
        }

        protected virtual void Dead()
        {
            isDead = true;
            col.enabled = false;
        }

        // 개발용 - 에디터에서 임의로 HP를 세팅한다.
        protected virtual void SetHP()
        {
            if (controlHP) curHP = newHP;
        }
    }
}