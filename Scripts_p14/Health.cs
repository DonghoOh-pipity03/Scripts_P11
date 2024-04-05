using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public abstract class Health : MonoBehaviour
    {
        [SerializeField] protected int maxHP = 100;
        protected int curHP;
        protected bool isDead = false;



        protected virtual void OnEnable()
        {
            curHP = maxHP;
        }

        public virtual void GetDamage(int _damage)
        {
            if (isDead) return;

            curHP -= _damage;
            if (curHP <= 0) Dead();

        }

        protected virtual void Dead()
        {
            isDead = true;
        }
    }
}