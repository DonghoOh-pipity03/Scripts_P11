using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;
using TMPro;
using System.ComponentModel.Design;
using System;

namespace Project_11
{
    public class GUIManager : MonoBehaviour
    {
        [Header("시간")]
        // GUI_Time
        [SerializeField] TMP_Text gameTime_TMP;

        [Header("차량 - 속도")]
        // GUI_Vehicle_Speed
        [SerializeField] TMP_Text speed_TMP;
        const string speedFormat = "{0} km/h";

        [Header("스킬")]
        // GUI_Vehicle_Skill
        [SerializeField] TMP_Text smoke_TMP;
        [SerializeField] TMP_Text smoke_TMP_2;
        [SerializeField] TMP_Text wp_TMP;
        [SerializeField] TMP_Text wp_TMP_2;
        [SerializeField] TMP_Text aps_TMP_L;
        [SerializeField] TMP_Text aps_TMP_L_2;
        [SerializeField] TMP_Text aps_TMP_R;
        [SerializeField] TMP_Text aps_TMP_R_2;

        const string waiFormat = "WAIT {0:F1}";



        [SerializeField] Color green;
        [SerializeField] Color red;
        [SerializeField] float colorChangeSpeed;


        #region Singleton
        // 싱글톤
        private static GUIManager GM;
        public static GUIManager singleton
        {
            get
            {
                if (GM == null)
                {
                    GM = FindObjectOfType<GUIManager>();
                    if (GM == null) Debug.Log("GUIManager를 사용하려 했지만, 없어요.");
                }
                return GM;
            }
        }
        #endregion

        private void Update()
        {
            Velocity_GUI();
        }

        // 게임 시간
        public void ShowGameTime(int _min, int _sec)
        {
            string formattedTime = string.Format("{0:00}:{1:00}", _min, _sec);

            gameTime_TMP.text = formattedTime;
        }


        // 차량 - 속도
        void Velocity_GUI()
        {
            if (speed_TMP != null) speed_TMP.text = string.Format(speedFormat, (int)(PlayerManager.singleton.vehicleVelocity));
        }


        // 스킬 - 활성화
        public void Active_Smoke()
        {
            smoke_TMP.color = green;
            smoke_TMP.alpha = 1;
            smoke_TMP_2.color = green;
            smoke_TMP_2.alpha = 1;
            smoke_TMP_2.text = "READY";
        }

        public void Active_WP()
        {
            wp_TMP.color = green;
            wp_TMP.alpha = 1;
            wp_TMP_2.color = green;
            wp_TMP_2.alpha = 1;
            wp_TMP_2.text = "READY";
        }

        public void Active_APS_L()
        {
            aps_TMP_L.color = green;
            aps_TMP_L.alpha = 1;
            aps_TMP_L_2.color = green;
            aps_TMP_L_2.alpha = 1;
            aps_TMP_L_2.text = "READY";
        }

        public void Active_APS_R()
        {
            aps_TMP_R.color = green;
            aps_TMP_R.alpha = 1;
            aps_TMP_R_2.color = green;
            aps_TMP_R_2.alpha = 1;
            aps_TMP_R_2.text = "READY";
        }


        // 스킬 - 대기시간
        public void Wait_Smoke(float _t)
        {
            if (smoke_TMP != null)
            {
                if (_t > 0)
                {
                    smoke_TMP_2.text = string.Format(waiFormat, _t);

                    float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1.0f); // 0과 1 사이를 왕복하는 값 계산
                    Color lerpedColor = Color.Lerp(green, red, t); // A와 B 사이의 보간된 색상 계산

                    //smoke_TMP.color = lerpedColor;
                    smoke_TMP.alpha = 1;
                    //smoke_TMP_2.color = lerpedColor;
                    smoke_TMP_2.alpha = 1;
                }
                else
                {
                    smoke_TMP.color = green;
                    smoke_TMP_2.color = green;
                    smoke_TMP_2.text = "READY";
                }
            }
        }

        public void Wait_WP(float _t)
        {
            if (wp_TMP != null)
            {
                if (_t > 0)
                {
                    wp_TMP_2.text = string.Format(waiFormat, _t);

                    float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1.0f); // 0과 1 사이를 왕복하는 값 계산
                    Color lerpedColor = Color.Lerp(green, red, t); // A와 B 사이의 보간된 색상 계산

                    //wp_TMP.color = lerpedColor;
                    wp_TMP.alpha = 1;
                    //wp_TMP_2.color = lerpedColor;
                    wp_TMP_2.alpha = 1;
                }
                else
                {
                    wp_TMP.color = green;
                    wp_TMP_2.color = green;
                    wp_TMP_2.text = "READY";
                }
            }
        }

        public void Wait_APS_L(float _t)
        {
            if (aps_TMP_L != null)
            {
                if (_t > 0)
                {
                    aps_TMP_L_2.text = string.Format(waiFormat, _t);

                    float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1.0f); // 0과 1 사이를 왕복하는 값 계산
                    Color lerpedColor = Color.Lerp(green, red, t); // A와 B 사이의 보간된 색상 계산

                    //aps_TMP_L.color = lerpedColor;
                    aps_TMP_L.alpha = 1;
                    //aps_TMP_L_2.color = lerpedColor;
                    aps_TMP_L_2.alpha = 1;
                }
                else
                {
                    aps_TMP_L.color = green;
                    aps_TMP_L.alpha = 1;
                    aps_TMP_L_2.color = green;
                    aps_TMP_L_2.alpha = 1;
                    aps_TMP_L_2.text = "READY";
                }
            }
        }

        public void Wait_APS_R(float _t)
        {
            if (aps_TMP_R != null)
            {
                if (_t > 0)
                {
                    aps_TMP_R_2.text = string.Format(waiFormat, _t);

                    float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1.0f); // 0과 1 사이를 왕복하는 값 계산
                    Color lerpedColor = Color.Lerp(green, red, t); // A와 B 사이의 보간된 색상 계산

                    //aps_TMP_R.color = lerpedColor;
                    aps_TMP_R.alpha = 1;
                    //aps_TMP_R_2.color = lerpedColor;
                    aps_TMP_R_2.alpha = 1;
                }
                else
                {
                    aps_TMP_R.color = green;
                    aps_TMP_R.alpha = 1;
                    aps_TMP_R_2.color = green;
                    aps_TMP_R_2.alpha = 1;
                    aps_TMP_R_2.text = "READY";
                }
            }
        }



        Color HexToColor(string hex)
        {
            Color newColor = Color.black;

            if (ColorUtility.TryParseHtmlString(hex, out newColor))
            {
                return newColor;
            }
            else
            {
                Debug.LogWarning("유효하지 않은 Hex 값");
                return newColor;
            }
        }
    }
}