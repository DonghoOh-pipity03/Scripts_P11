using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Project_11;

public class SkillManager : MonoBehaviour
{

    #region Singleton
    // 싱글톤
    private static SkillManager SM;
    public static SkillManager singleton
    {
        get
        {
            if (SM == null)
            {
                SM = FindObjectOfType<SkillManager>();
                if (SM == null) Debug.Log("SkillManager를 사용하려 했지만, 없어요.");
            }
            return SM;
        }
    }
    #endregion


    // 모든 스킬 데미지 증가
    public void All_Skill_Damage_Plus(int _val)
    {
        // 드론
        Skill_Dron.singleton.Damage_Plus(_val);
        // 연막 생성기
        // 백린 연막 
        // 능동 방어 장치

        // 포격 지원
        // 항공 지원
        // 외부 저격수
        // 미사일

        // 조합 스킬들
    }

}
