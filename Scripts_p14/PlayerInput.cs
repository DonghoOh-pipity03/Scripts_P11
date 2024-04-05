using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public class PlayerInput : MonoBehaviour
    {
        void Update()
        {
            DevInput();
        }

        void DevInput()
        {
            // 사격 
            if (Input.GetKeyDown(KeyCode.F)) { Hands.singleton.Act_Trigger(); }
            if (Input.GetKeyUp(KeyCode.F)) { Hands.singleton.Act_DeTrigger(); }
            
            // 슬라이드/노리쇠 후퇴 draW
            if (Input.GetKeyDown(KeyCode.W)) { Hands.singleton.Act_Slide(); }

            // 탄창 삽입 E(I)nsert 
            if (Input.GetKeyDown(KeyCode.E)) { Hands.singleton.Act_Insert(); }

            // 탄창 제거 Remove
            if (Input.GetKeyDown(KeyCode.R)) { Hands.singleton.Act_Remove(); }

            // 탄창멈치 탭 TAB!
            if (Input.GetKeyDown(KeyCode.Tab)) { Hands.singleton.Act_MagStop(); }

            // 아이템 줍기 
            if (Input.GetKeyDown(KeyCode.Q)) { Hands.singleton.Act_PickUp(); }

            // 물건 상호작용
            if (Input.GetKeyDown(KeyCode.Alpha1)) { Hands.singleton.Act_LogisItem(0); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { Hands.singleton.Act_LogisItem(1); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { Hands.singleton.Act_LogisItem(2); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { Hands.singleton.Act_LogisItem(3); }

            if (Input.GetKeyDown(KeyCode.Alpha5)) { Hands.singleton.Act_LogisItem(10); }
            if (Input.GetKeyDown(KeyCode.Alpha6)) { Hands.singleton.Act_LogisItem(11); }

            if (Input.GetKeyDown(KeyCode.F1)) { Hands.singleton.Act_LogisItem(20); }
            if (Input.GetKeyDown(KeyCode.F2)) { Hands.singleton.Act_LogisItem(21); }
            if (Input.GetKeyDown(KeyCode.F3)) { Hands.singleton.Act_LogisItem(22); }
            if (Input.GetKeyDown(KeyCode.F4)) { Hands.singleton.Act_LogisItem(23); }    

        }
    }
}