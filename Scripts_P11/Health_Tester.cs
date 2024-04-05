using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Health_Tester : Health
    {
        [Header("UI_테스터")]
        [SerializeField] TMP_Text text;
        string body;
        [Header("개발 옵션")]
        public bool useOutPut = true;

        public override void GetDamage(int _Damage, WeaponKind _DamageKind)
        {
            base.GetDamage(_Damage, _DamageKind);

            if (isDead) return;

            if (useOutPut)
            {
                body += "Time: " + (Time.time).ToString() + ", Damage:" + _Damage.ToString() + "\n";
                text.text = body;
            }
        }

        protected override void Dead()
        {
            base.Dead();
            body = "Dead";
            text.text = body;
        }
    }
}