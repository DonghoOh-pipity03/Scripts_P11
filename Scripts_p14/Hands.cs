/*
    // (1) 들고 있는 아이템을 사용한다.
    (2) 아이템과 인벤토리 관리를 한다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public class Hands : MonoBehaviour
    {
        public GameObject curItem_R_Hand;
        [SerializeField] Gun curGun;
        int curWeaponNum = 0;   // 현재 들고있는 무기 종류, 0-주무기, 1-보조무기, 2-가젯1, 3-가젯2...-1-빈 손

        public GameObject curItem_L_Hand;

        [SerializeField] Transform carrier;


        #region 싱글톤
        // 싱글톤
        private static Hands H;
        public static Hands singleton
        {
            get
            {
                if (H == null)
                {
                    H = FindObjectOfType<Hands>();
                    if (H == null) Debug.Log("Hands를 사용하려 했지만, 없어요.");
                }
                return H;
            }
        }
        #endregion


        // 공통
        // 사격
        public void Act_Trigger()
        {
            if (curGun != null && AnimatorManager.singleton.IsIdleState(1))
            {
                curGun.Act_Trigger();
                AnimatorManager.singleton.TriggerAnim();
            }
        }

        public void Act_DeTrigger()
        {
            if (curGun != null)
            {
                curGun.Act_DeTrigger();
                AnimatorManager.singleton.DeTriggerAnim();
            }
        }

        // 투척

        public void Act_Remove()
        {
            if (curGun != null) curGun.Act_Remove();
            // 보조무기, 가젯 버리기?
        }


        public void Act_Slide()
        {
            if (curGun != null && AnimatorManager.singleton.IsIdleState(1))
            {
                curGun.Act_Slide();
            }
        }

        public void Act_Insert()
        {
            if (curGun != null && AnimatorManager.singleton.IsIdleState(1))
            {
                curGun.Act_Insert();
            }
        }

        public void Act_MagStop()
        {
            if (curGun != null && AnimatorManager.singleton.IsIdleState(1))
            {
                curGun.Act_MagStop();
                AnimatorManager.singleton.MagStopTabAnim();
            }
        }

        public void Act_PickUp()
        {
            GameObject item = PickUp.singleton.PickItem();
            // 주울 물건이 있을 때
            if (item != null)
            {
                // 손에 무언가를 들고 있었을 때 - 버림
                if (curItem_L_Hand != null)
                {
                    curItem_L_Hand.gameObject.transform.SetParent(null);
                    Mag temp = curItem_L_Hand.GetComponent<Mag>();
                    if (temp != null) temp.Dropped();
                }
                // 줍기 - 겹치면 아무거나
                item.gameObject.transform.SetParent(carrier);
                item.GetComponent<Mag>().Picked();
                item.transform.localPosition = Vector3.zero;
                curItem_L_Hand = item;
                Debug.Log("아이템을 주움");
            }
        }

        // 아이템 상호작용
        // 아이템 프로토콜 사용, Inventory.cs의 GetItem함수 주석 참고
        public void Act_LogisItem(int _slot)
        {
            // 탄창류
            if (_slot < 10)
            {
                // 이미 왼손에 들고있는 경우
                if (curItem_L_Hand != null)
                {
                    // 집어넣기
                    // 집어넣기 성공
                    if (Inventory.singleton.SetItem(_slot, curItem_L_Hand))
                    {
                        curItem_L_Hand = null;
                        Debug.Log("물건 집어 넣기 성공");
                    }
                    // 집어넣기 실패
                    else
                    {
                        Debug.Log("물건 집어 넣기 실패");
                    }
                }
                // 이미 왼손이 비어있는 경우
                else
                {
                    // 꺼내기 시도
                    GameObject item = Inventory.singleton.GetItem(_slot);

                    // 꺼낸 아이템 없음
                    if (item == null)
                    {
                        // 변화없음
                    }
                    // 꺼낸 아이템 있음
                    else
                    {
                        curItem_L_Hand = item;
                        Debug.Log("꺼낸 물건을 왼손에 잡음");
                    }
                }
            }

            // 무기류
            else
            {
                // 오른손에 들고있는 경우
                if (curItem_R_Hand != null)
                {
                    // 들고있는 무기를 누른 경우
                    if (_slot % 10 == curWeaponNum)
                    {
                        // 집어넣기 성공
                        if (Inventory.singleton.SetItem(_slot, curItem_R_Hand))
                        {
                            curItem_R_Hand = null;
                            curGun = null;
                            curWeaponNum = -1;
                            Debug.Log("무기 집어 넣기 성공");
                        }
                        // 집어넣기 실패
                        else
                        {
                            Debug.Log("무기 집어 넣기 실패");
                        }

                    }
                    // 다른 무기를 누른 경우
                    else
                    {
                        // 주무기를 들고 있는 경우
                        if (curWeaponNum == 0)
                        {
                            // 주무기 집어넣고, 다른 무기 꺼내기
                            Inventory.singleton.SetItem(20, curItem_R_Hand);
                            curItem_R_Hand = null;
                            curGun = null;
                            curWeaponNum = -1;
                            Debug.Log("무기 집어 넣기 성공");

                            // 꺼내기 시도
                            GameObject item = Inventory.singleton.GetItem(_slot);

                            // 꺼낸 아이템 없음
                            if (item == null)
                            {
                                // 변화없음
                            }
                            // 꺼낸 아이템 있음
                            else
                            {
                                curItem_R_Hand = item;
                                Gun gun = item.GetComponent<Gun>();
                                if (gun != null) curGun = gun;
                                curWeaponNum = _slot % 10;
                                Debug.Log("꺼낸 무기를 오른손에 잡음");
                            }
                        }
                        // 주무기를 들고있지 않은 경우
                        else
                        {
                            // 반응없음
                        }
                    }
                }
                // 오른손이 비어있는 경우
                else
                {
                    // 꺼내기 시도
                    GameObject item = Inventory.singleton.GetItem(_slot);

                    // 꺼낸 아이템 없음
                    if (item == null)
                    {
                        // 변화없음
                    }
                    // 꺼낸 아이템 있음
                    else
                    {
                        curItem_R_Hand = item;
                        Gun gun = item.GetComponent<Gun>();
                        if (gun != null) curGun = gun;
                        curWeaponNum = _slot % 10;
                        Debug.Log("꺼낸 무기를 오른손에 잡음");
                    }
                }
            }
        }

    }
}