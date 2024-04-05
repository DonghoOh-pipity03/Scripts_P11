using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public class Health_Player : Health
    {
        [HideInInspector] 
        private new int maxHP => PlayerManager.singleton.maxHP;


        // 싱글톤
        private static Health_Player HP;
        public static Health_Player singleton
        {
            get
            {
                if (HP == null)
                {
                    HP = FindObjectOfType<Health_Player>();
                    if (HP == null) Debug.Log("Health_Player를 사용하려 했지만, 없어요.");
                }
                return HP;
            }
        }
        
    }
}