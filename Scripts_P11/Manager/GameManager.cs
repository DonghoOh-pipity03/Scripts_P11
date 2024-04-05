using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class GameManager : MonoBehaviour
    {

        Dictionary<WeaponKind, int> TotalDamage = new Dictionary<WeaponKind, int>();    // 임시로 OnEnable에 하드코딩으로 초기화함

        [Header("시간")]
        float gameStartTime;
        public int curGameTime_min;
        public int curGameTime_sec;

        [Header("UI")]
        public bool isInGameUIMode; // 게임 UI 사용 유무, 반대의 경우는 메뉴 UI 사용시

        #region Singlton
       // 싱글톤
        private static GameManager GM;
        public static GameManager singleton
        {
            get
            {
                if (GM == null)
                {
                    GM = FindObjectOfType<GameManager>();
                    if (GM == null) Debug.Log("GameManager를 사용하려 했지만, 없어요.");
                }
                return GM;
            }
        }
        #endregion


        #region Lifecycle_Fucntion
        private void Start()
        {
            SetGameTimeStart();

            Cursor_Game();  // 개발용 코드
        }

        private void Update() 
        {
            SetGameTime();
        }
        #endregion


        #region Fucntion
        public void GetDamageData(WeaponKind _weaponKind, int _damageAmount)
        {
            // 정보 있음
            if (TotalDamage.ContainsKey(_weaponKind)) TotalDamage[_weaponKind] += _damageAmount;
            // 공격 받은 적이 없음
            else TotalDamage[_weaponKind] = _damageAmount;
        }


        void SetGameTimeStart()
        {
            gameStartTime = Time.time;
        }

        void SetGameTime()
        {
            float gameTime = Time.time - gameStartTime;
            curGameTime_min = Mathf.FloorToInt(gameTime / 60);
            curGameTime_sec = Mathf.FloorToInt(gameTime % 60);

            GUIManager.singleton.ShowGameTime(curGameTime_min, curGameTime_sec);

        }

        public void Cursor_Game()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        public void Cursor_Menu()
        {
            Cursor.lockState = CursorLockMode.None;
        }
        #endregion
    }
}