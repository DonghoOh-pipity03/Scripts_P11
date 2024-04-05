using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("이동 속도")]
        [SerializeField] public float moveSpd;

        [Header("체력")]
        [SerializeField] public int maxHP;

        // 싱글톤
        private static PlayerManager PM;
        public static PlayerManager singleton
        {
            get
            {
                if (PM == null)
                {
                    PM = FindObjectOfType<PlayerManager>();
                    if (PM == null) Debug.Log("PlayerManager를 사용하려 했지만, 없어요.");
                }
                return PM;
            }
        }

    }
}