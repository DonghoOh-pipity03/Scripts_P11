using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Health_Obstacle : Health
    {
        [Header("체력_장애물")]
        [SerializeField] GameObject originalOBJ; // 파괴전 장애물 - 오브젝트
        [SerializeField] GameObject destroyedOBJ;   // 파괴후 장애물 - 오브젝트
        
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        
        protected override void Dead()
        {   
            base.Dead();

            Destroy(originalOBJ);
            destroyedOBJ.SetActive(true);

            Destroy(col);
            Destroy(this);
        }
    }
}