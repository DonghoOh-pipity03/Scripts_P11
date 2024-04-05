using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public class CombatManager : MonoBehaviour
    {
        // 싱글톤
        private static CombatManager CM;
        public static CombatManager singleton
        {
            get
            {
                if (CM == null)
                {
                    CM = FindObjectOfType<CombatManager>();
                    if (CM == null) Debug.Log("CombatManager를 사용하려 했지만, 없어요.");
                }
                return CM;
            }
        }

        public void Attack(Health _attacker, Gun _weapon, Health _target)
        {
            // 데미지 전달 - 
        }
    }
}