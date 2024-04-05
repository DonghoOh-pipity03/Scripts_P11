using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;

namespace Project_14
{
    public class AnimatorManager : MonoBehaviour
    {
        Animator animator;
        [SerializeField] float WalkLerpSpd;
        float curAnimMove;


        #region 싱글톤
        // 싱글톤
        private static AnimatorManager AM;
        public static AnimatorManager singleton
        {
            get
            {
                if (AM == null)
                {
                    AM = FindObjectOfType<AnimatorManager>();
                    if (AM == null) Debug.Log("AnimatorManager를 사용하려 했지만, 없어요.");
                }
                return AM;
            }
        }
        #endregion


        private void Awake()
        {
            animator = GetComponent<Animator>();
        }


        // 레이어 번호를 입력 받아서, Idle 상태 여부를 반환하는 함수 - 1: 상체, 3: 왼손
        public bool IsIdleState(int _layer)
        {
            string name = GetCurrentClipName(_layer);
            if (name.Contains("Idle")) return true;
            else return false;
        }

        // 레이어 계층 번호를 입력받아, 재생 중인 클립의 이름을 반환하는 함수
        string GetCurrentClipName(int _layer)
        {
            // 레이어에서 재생 중인 클립 정보를 가져옵니다.
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(_layer);

            // 첫 번째 클립의 이름을 반환합니다. 여러 클립이 재생 중이라면 적절한 방식으로 처리해야 합니다.
            string clipName = clipInfo.Length > 0 ? clipInfo[0].clip.name : "None";

            return clipName;
        }

        
        // 애니메이터에서 0 ~ 0.5 ~ 1로 처리함 
        public void WalkAnim(float _move)
        {
            curAnimMove = Mathf.Lerp(curAnimMove, _move, WalkLerpSpd);
            if (animator != null) animator.SetFloat("Move", curAnimMove);
        }

        public void TriggerAnim()
        {
            if (animator != null) animator.SetBool("Trigger", true);
        }
        public void DeTriggerAnim()
        {
            if (animator != null) animator.SetBool("Trigger", false);
        }

        public void ShootAnim()
        {
            if (animator != null) animator.SetTrigger("Shoot");
        }

        public void MagStopPullAnim()
        {
            if (animator != null) animator.SetTrigger("MagStopPull");
        }

        public void ReloadAnim()
        {
            if (animator != null) animator.SetTrigger("Reload");
        }

        public void MagStopTabAnim()
        {
            if (animator != null) animator.SetTrigger("MagStopTab");
        }
    }
}