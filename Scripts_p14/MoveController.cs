using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project_14;

namespace Project_14
{
    public class MoveController : MonoBehaviour
    {
        AnimatorManager animatorManager;
        float horizontalInput => Input.GetAxis("Horizontal");
        [HideInInspector] public bool clicked_left = false;
        [HideInInspector] public bool clicked_right = false;

        // 싱글톤
        private static MoveController MC;
        public static MoveController singleton
        {
            get
            {
                if (MC == null)
                {
                    MC = FindObjectOfType<MoveController>();
                    if (MC == null) Debug.Log("MoveController를 사용하려 했지만, 없어요.");
                }
                return MC;
            }
        }

        private void Awake()
        {
            animatorManager = GetComponent<AnimatorManager>();
        }

        private void Update()
        {
            Move();
        }

        void Move()
        {
            if (horizontalInput > 0 || clicked_right) GoRight();
            else if (horizontalInput < 0 || clicked_left) GoLeft();
            else UpdateAnim(0.5f);
        }

        public void GoLeft()
        {
            transform.Translate(-Vector3.forward * PlayerManager.singleton.moveSpd * Time.deltaTime);
            UpdateAnim(0);
        }
        public void GoRight()
        {
            transform.Translate(Vector3.forward * PlayerManager.singleton.moveSpd * Time.deltaTime);
            UpdateAnim(1);
        }

        // 애니메이터에서 0 ~ 0.5 ~ 1로 처리함 
        void UpdateAnim(float _move)
        {
            if (animatorManager != null) animatorManager.WalkAnim(_move);
        }
    }
}