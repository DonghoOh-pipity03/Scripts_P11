using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_11
{
    public abstract class Skill:MonoBehaviour
    {   
        [SerializeField] protected int curLevel = 0;

        // 레벨업
        public abstract void LevelUp();

        public abstract void Master_1();
        public abstract void Master_2();
        public abstract void Master_3();

        public abstract void DisableSkill();

        // 밸런스 함수
        public abstract void Damage_Plus(int val);
    }
}