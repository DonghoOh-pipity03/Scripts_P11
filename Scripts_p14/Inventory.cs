using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;
using System;

namespace Project_14
{
    public class Inventory : MonoBehaviour
    {
        const int mainMagCnt = 4;
        const int subMagCnt = 2;
        [SerializeField] GameObject[] mainMag;
        [SerializeField] GameObject[] subMag;
        [SerializeField] GameObject[] weapon;



        // 싱글톤
        private static Inventory I;
        public static Inventory singleton
        {
            get
            {
                if (I == null)
                {
                    I = FindObjectOfType<Inventory>();
                    if (I == null) Debug.Log("Inventory를 사용하려 했지만, 없어요.");
                }
                return I;
            }
        }


        // 꺼내기 
        // 슬롯 프로토콜: 0~3 - 메인 탄창, 10~11 - 서브 탄창, 20~23 - 무기와 가젯
        public GameObject GetItem(int _slot)
        {
            int flag = FindInven(_slot);

            // 아이템 미존재 & 잘못된 주소 접근
            if (flag == 0 || flag == 2) return null;

            // 메인 탄창
            if (_slot / 10 == 0)
            {
                GameObject temp = mainMag[_slot];
                mainMag[_slot] = null;
                return temp;
            }
            // 서브 탄창
            else if (_slot / 10 == 1)
            {
                GameObject temp = subMag[_slot % 10];
                subMag[_slot % 10] = null;
                return temp;
            }
            // 무기
            else
            {
                GameObject temp = weapon[_slot % 10];
                weapon[_slot % 10] = null;
                return temp;
            }
        }

        // 집어넣기
        public bool SetItem(int _slot, GameObject _obj)
        {
             int flag = FindInven(_slot);

            // 아이템 존재 & 잘못된 주소 접근
            if (flag == 0 || flag == 1) return false;

            // 메인 탄창
            if (_slot / 10 == 0) mainMag[_slot] = _obj;
            // 서브 탄창
            else if (_slot / 10 == 1) subMag[_slot % 10] = _obj;
            // 무기
            else weapon[_slot % 10] = _obj;
            
            return true;
        }

        // 아이템 검색
        // 잘못된 주소: 0, 아이템 있음: 1, 아이템 없음: 2
        int FindInven(int _slot)
        {
            // 메인 탄창
            if (_slot / 10 == 0)
            {
                // 잘못된 주소 접근
                if (mainMag.Length <= _slot)
                {
                    Debug.LogWarning("Inventory에서 잘못된 주소에 접근 시도");
                    return 0;
                }
                // 아이템 미존재
                else if (mainMag[_slot] == null) return 2;
                // 아이템 존재
                else return 1;

            }
            // 서브 탄창
            else if (_slot / 10 == 1)
            {
                // 잘못된 주소 접근
                if (subMag.Length <= _slot % 10)
                {
                    Debug.LogWarning("Inventory에서 잘못된 주소에 접근 시도");
                    return 0;
                }
                // 아이템 미존재
                else if (subMag[_slot % 10] == null) return 2;
                // 아이템 존재
                else return 1;
            }
            // 무기
            else
            {
                // 잘못된 주소 접근
                if (weapon.Length <= _slot % 10)
                {
                    Debug.LogWarning("Inventory에서 잘못된 주소에 접근 시도");
                    return 0;
                }
                // 아이템 미존재
                else if (weapon[_slot % 10] == null) return 2;
                // 아이템 존재
                else return 1;
            }
        }

    }
}