using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public class Gun : MonoBehaviour
    {
        public enum SlideState
        {
            Forward,
            Backward,
            Jam
        }
        public enum FireMode
        {
            Safe,
            One,
            Two,
            Three,
            Auto
        }
        [Header("무기 스펙")]
        [SerializeField] FireMode[] firemode; // 총이 가지는 발사모드
        [SerializeField] bool useSlide = false; // 노리쇠/슬라이드 후퇴고정 상태에서, 슬라이드를 뒤로 당기면 슬라이드 전진이 되는지 여부
        [SerializeField] bool doubleAction = false; // 더블액션 방식 여부 - 베레타 92, p226/8/9, Five-Seven, Mark23, USP, 리볼버 권총
        [SerializeField] bool revolver = false; // 리볼버 방식 여부
        [SerializeField] float FireRPM;   // 총 발사 RPM


        [Header("무기 상태")]
        [SerializeField] Mag curMag;
        [SerializeField] MagKind magKind;
        [SerializeField] bool chamberCharged;   // 약실에 탄 존재 여부
        [SerializeField] bool cocked;    // 장전 여부
        [SerializeField] SlideState slideState;  // 슬라이드/노리쇠 상태
        [SerializeField] bool jamed = false; // 탄피 잼 여부
        [SerializeField] bool triggered = false; // 트리거 작동 여부
        [SerializeField] int curFireMode = 0;

        float fireInterval; // 발사 속도에 따른 발사 간격

        // 사격
        float firstFireTime;  // 쏴야하는 발사 batch 중, 첫번째 발사 시간
        int batchCount; // 쏴야하는 발사 batch 중, batch 크기
        int fireCount;   // 쏴야하는 발사 batch 중, 발사한 횟수
        float lastFireTime = float.MinValue; // 마지막 발사 시간

        private void Start()
        {
            fireInterval = 60 / FireRPM;
        }
        private void Update()
        {
            React_Fire_Shot_FCS();
        }

        #region 액션 - 트리거
        // 액션 - 트리거
        public void Act_Trigger()
        {
            triggered = true;
            Debug.Log("트리거 작동");

            // 싱글액션
            if (!doubleAction)
            {
                // 슬라이드/노리쇠 전진
                if (slideState == SlideState.Forward)
                {
                    // 장전됨
                    if (cocked)
                    {
                        // 약실에 탄 있음
                        if (chamberCharged)
                        {
                            React_Fire_Shot();  // 발사
                        }
                        // 약실에 탄 없음
                        else
                        {
                            React_Fire_UnCock();    // 장전 풀림
                        }
                    }
                    // 장전안됨
                    else
                    {
                        React_Fire_None();  // 반응 없음
                    }
                }
                // 슬라이드/노리쇠 후퇴 || 잼
                else
                {
                    React_Fire_None();  // 반응 없음
                }
            }

            // 더블액션
            else
            {
                // 슬라이드/노리쇠 전진 || 리볼버 방식
                if (slideState == SlideState.Forward || revolver)
                {
                    // 약실에 탄 있음
                    if (chamberCharged)
                    {
                        // 장전됨
                        if (cocked)
                        {
                            React_Fire_Shot();  // 발사
                        }
                        // 장전안됨
                        else
                        {
                            React_Fire_Shot();  // 발사 + 명중률 낮음
                        }
                    }
                    // 약실에 탄 없음
                    else
                    {
                        // 장전됨 
                        if (cocked)
                        {
                            React_Fire_UnCock();    // 장전 풀림
                        }
                        // 장전안됨
                        else
                        {
                            React_Fire_None();  // 반응 없음
                        }
                    }

                }
                // 슬라이드/노리쇠 후퇴 || 잼
                else
                {
                    React_Fire_None();  // 반응 없음
                }
            }
        }

        // 리액션 - 사격_반응없음
        void React_Fire_None() { }

        // 리액션 - 사격_장전풀림
        void React_Fire_UnCock()
        {
            cocked = false;
        }

        // 리액션 - 사격_발사
        // 사격절차 1티어 - 사격통제장치: 트리거 작동에 의해서, 격발 횟수와 초탄 발사시간을 지정한다.
        void React_Fire_Shot()
        {
            // 마지막 발사 시간으로부터 재발사 시간 이전인 경우, 새로운 발사 명령 무시
            if (Time.time < lastFireTime + fireInterval) return;

            fireCount = 0;
            firstFireTime = Time.time;

            if (firemode[curFireMode] == FireMode.Auto) batchCount = int.MaxValue;
            else batchCount = (int)firemode[curFireMode];
        }

        // 사격절차 2티어  - 사격통제장치: 남은 격발 횟수에 따라, 격발 처리를 한다. Update문에 의존한다.
        void React_Fire_Shot_FCS()
        {
            // 자동사격이 끝난 경우, 잔탄이 없는 경우, 종료
            if (batchCount == fireCount || !chamberCharged) return;
            // 연발사격모드이고, 트리거를 뗀 경우, 종료
            if (firemode[curFireMode] == FireMode.Auto && !triggered) return;

            // 현재 프레임에 발사해야 하는 횟수 구하기
            int fireCnt = Mathf.FloorToInt((Time.time - firstFireTime) / fireInterval) + 1; // 첫 발사 때부터 지금까지, 발사해야 하는 횟수
            int fireCntNow = fireCnt - fireCount;   // 지금 프레임에서 발사해야 하는 횟수

            // 사격
            fireCount += fireCntNow;
            while (fireCntNow > 0)
            {
                fireCntNow--;
                Shot();
            }
        }

        // 3티어 사격절차: 최종 1회 사격
        void Shot()
        {
            // 사격
            Debug.Log("사격!"); // 임시

            chamberCharged = false;
            lastFireTime = Time.time;
            React_Fire_Slide();

            AnimatorManager.singleton.ShootAnim();
        }

        // 리액션 - 발사 직후 자동 재장전
        public void React_Fire_Slide()
        {
            // 재장전 가능
            if (curMag != null && curMag.curBullet > 0)
            {
                // 재장전
                React_Slide_Reload();
            }
            // 재장전 불가능
            else
            {
                // 노리쇠/슬라이드 고정
                React_Slide_fixed();
            }
        }

        public void Act_DeTrigger() => triggered = false;
        #endregion


        #region 액션 - 탄창 제거
        // 액션 - 탄창 제거
        public void Act_Remove()
        {
            // 탄창이 결합되어 있음
            if (curMag != null)
            {
                // 왼손에 아이템을 들고 있음
                if (Hands.singleton.curItem_L_Hand != null)
                {
                    // 탄창을 분리하여 바닥에 버림
                    React_Remove_GunMag();
                }
                // 왼손에 아이템을 들고 있지 않음
                else
                {
                    // 탄창을 분리하여 왼손에 듬
                    React_Remove_Grab();
                }
            }
            // 왼손에 아이템을 들고 있음
            else if (Hands.singleton.curItem_L_Hand != null)
            {
                // 왼손의 아이템을 버림
                React_Remove_Left();
            }
            // 아무것도 아님
            else React_Remove_None();
        }

        // 리액션 - 탄창 제거_변화없음
        public void React_Remove_None() { }

        // 리액션 - 탄창 제거_왼손에 들기
        public void React_Remove_Grab()
        {
            Hands.singleton.curItem_L_Hand = curMag.gameObject;
            curMag = null;
            Debug.Log("제거한 탄창, 왼손으로 이동");
        }

        // 리액션 - 탄창 제거_결합 탄창 버리기
        public void React_Remove_GunMag()
        {
            curMag.gameObject.transform.SetParent(null);
            curMag.Dropped();
            curMag = null;
            Debug.Log("결합된 탄창을 버림");
        }

        // 리액션 - 탄창 제거_왼손 아이템 버리기
        public void React_Remove_Left()
        {
            // 왼손에 물건이 있을 때
            if (Hands.singleton.curItem_L_Hand != null)
            {
                Hands.singleton.curItem_L_Hand.gameObject.transform.SetParent(null);

                Mag temp = Hands.singleton.curItem_L_Hand.GetComponent<Mag>();
                if (temp != null) temp.Dropped();
                Hands.singleton.curItem_L_Hand = null;
                Debug.Log("왼손에 있던 아이템을 버림");
            }
        }

        #endregion


        #region 액션 - 탄창 삽입
        // 액션 - 탄창 삽입
        public void Act_Insert()
        {
            // 삽입 불가능 - 빈 손, 탄창이 아닌 다른 것을 들고 있음, 맞지않는 탄창 규격, 이미 탄창이 결합됨
            if (Hands.singleton.curItem_L_Hand == null) { React_Insert_fail(); return; }
            Mag mag = Hands.singleton.curItem_L_Hand.GetComponent<Mag>();
            if (mag == null) { React_Insert_fail(); return; }
            if (magKind != mag.magKind) { React_Insert_fail(); return; }
            if (curMag != null) { React_Insert_fail(); return; }

            // 삽입 가능
            React_Insert_Sucess();
        }

        // 리액션 - 탄창 삽입_탄창 삽입
        public void React_Insert_Sucess()
        {
            Mag mag = Hands.singleton.curItem_L_Hand.GetComponent<Mag>();
            curMag = mag;
            Hands.singleton.curItem_L_Hand = null;
            Debug.Log("탄창 삽입됨");
        }

        // 리액션 - 탄창 삽입_탄창 삽입 불가
        public void React_Insert_fail()
        {
            Debug.Log("탄창 삽입 명령 불가능");
        }
        #endregion


        #region 액션 - 노리쇠/슬라이드 후퇴
        // 액션 - 노리쇠/슬라이드 후퇴
        public void Act_Slide()
        {
            // 노리쇠/슬라이드 전진 / 잼 상태
            if (slideState != SlideState.Backward)
            {
                // 노리쇠/슬라이드 잼 상태
                if (slideState == SlideState.Jam)
                {
                    // 잼 해결
                    jamed = false;
                }

                // 재장전 가능
                if (curMag != null && curMag.curBullet > 0)
                {
                    // 재장전
                    React_Slide_Reload();

                    AnimatorManager.singleton.ReloadAnim();
                }
                // 재장전 불가능
                else
                {
                    // 노리쇠/슬라이드 고정
                    React_Slide_fixed();
                    AnimatorManager.singleton.MagStopPullAnim();
                }
            }
            // 노리쇠/슬라이드 후퇴 상태
            else
            {
                // 슬라이드 방식
                if (useSlide)
                {
                    // 탄창 없음 || 탄창에 탄 없음
                    if (curMag == null || curMag.curBullet == 0) React_Slide_EmptyReload();
                    // 탄창에 탄 있음
                    else
                    {
                        React_Slide_Reload();
                        AnimatorManager.singleton.ReloadAnim();
                    }
                }
                // 노리쇠 방식
                else
                {
                    // 반응 없음
                }
            }
        }

        // 리액션 - 노리쇠/슬라이드 후퇴_재장전
        void React_Slide_Reload()
        {
            chamberCharged = true;
            cocked = true;
            slideState = SlideState.Forward;

            curMag.curBullet--;

            Debug.Log("재장전");
        }

        // 리액션 - 노리쇠/슬라이드 후퇴_재장전(약실에 탄 없음)
        void React_Slide_EmptyReload()
        {
            cocked = true;
            slideState = SlideState.Forward;

            Debug.Log("재장전(약실에 탄 없음)");
        }

        // 리액션 - 노리쇠/슬라이드 후퇴_노리쇠/슬라이드 고정
        void React_Slide_fixed()
        {
            chamberCharged = false;
            cocked = false;
            slideState = SlideState.Backward;

            Debug.Log("노리쇠/슬라이드 후퇴고정");
        }
        #endregion


        #region 액션 - 탄창멈치
        // 액션 - 탄창멈치
        public void Act_MagStop()
        {
            // 슬라이드 후퇴 고정 상태
            if (slideState == SlideState.Backward)
            {
                // 탄창 없음 || 탄창에 탄 없음
                if (curMag == null || curMag.curBullet == 0) React_Slide_EmptyReload();

                // 탄창에 탄 있음
                else React_Slide_Reload();
            }
        }
        #endregion
    }
}