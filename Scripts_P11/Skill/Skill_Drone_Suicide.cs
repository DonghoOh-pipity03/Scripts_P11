/*
    분기: p
    위치 1: a
    위치 2: b

    (t<p)
    lerp( a, a + (b - a) * p)

    (t>p)
    lerp( a + (b - a) * p, b)
*/

using System.Collections;
using System.Collections.Generic;
using Project_11;
using UnityEngine;

namespace Project_11
{
    public class Skill_Drone_Suicide : MonoBehaviour
    {
        [SerializeField] float firstTime = 1; // 첫번째 분기 비행 시간
        [SerializeField] float secondTime = 1;    // 두번째 분기 비행 시간
        [SerializeField] float soarHeight;  // 초기 솟아오르는 높이
        [Range(0f, 1f)][SerializeField] float maxHeightPoint;    // 최대로 솟는 지점 분기: 0~1 
        [SerializeField] AnimationCurve xCurve_01; // X 좌표의 곡선
        [SerializeField] AnimationCurve xCurve_02; // X 좌표의 곡선
        [SerializeField] AnimationCurve yCurve_01; // Y 좌표의 곡선
        [SerializeField] AnimationCurve yCurve_02; // Y 좌표의 곡선
        [SerializeField] AnimationCurve zCurve_01; // Z 좌표의 곡선
        [SerializeField] AnimationCurve zCurve_02; // Z 좌표의 곡선
        Transform startPoint; // 출발지 GameObject
        Transform endPoint;   // 목적지 GameObject
        Vector3 initPos;
        Vector3 lastPos;
        float maxFlightHeight;
        float elapsedTime = 0.0f;   // 누적 동작 시간


        private void Start()
        {
            elapsedTime = 0;
            initPos = startPoint.position;
            maxFlightHeight = startPoint.position.y + soarHeight;
            lastPos = transform.position;
        }

        private void Update()
        {
            if (elapsedTime < firstTime + secondTime)
            {
                float x = 0;
                float y = 0;
                float z = 0;

                if (elapsedTime < firstTime)
                {   // 첫번째 상승 분기 이동
                    float t = elapsedTime / firstTime;
                    x = Mathf.Lerp(initPos.x, initPos.x + (endPoint.position.x - initPos.x) * maxHeightPoint, xCurve_01.Evaluate(t));
                    y = Mathf.Lerp(initPos.y, maxFlightHeight, yCurve_01.Evaluate(t));
                    z = Mathf.Lerp(initPos.z, initPos.z + (endPoint.position.z - initPos.z) * maxHeightPoint, zCurve_01.Evaluate(t));
                }
                else
                {   // 두번째 하강 분기 이동
                    float t = (elapsedTime - firstTime) / secondTime;
                    x = Mathf.Lerp(initPos.x + (endPoint.position.x - initPos.x) * maxHeightPoint, endPoint.position.x, xCurve_02.Evaluate(t));
                    y = Mathf.Lerp(maxFlightHeight, endPoint.position.y, yCurve_02.Evaluate(t));
                    z = Mathf.Lerp(initPos.z + (endPoint.position.z - initPos.z) * maxHeightPoint, endPoint.position.z, zCurve_02.Evaluate(t));
                }

                // 이동 
                Vector3 newPosition = new Vector3(x, y, z);
                transform.position = newPosition;

                // 시간 업데이트
                elapsedTime += Time.deltaTime;

                // 운동방향에 따른 회전
                Vector3 moveDirection = (transform.position - lastPos).normalized;
                if (moveDirection != Vector3.zero) transform.forward = moveDirection;
                lastPos = transform.position;
            }
            else if (elapsedTime > firstTime + secondTime)
            {
                // 공격
                PlayerManager.singleton.SendDamage_P2E(transform.position, Skill_Dron.singleton.damage_SuicideDrone, WeaponKind.Skill_Dron, DamageKind.Explosion, Skill_Dron.singleton.damageDist_SuicideDrone, 0, Vector3.zero);

                // 비활성화
                this.gameObject.SetActive(false);
            }
        }

        public void SetInit(Transform _startPoint, Transform _endPoint)
        {
            startPoint = _startPoint;
            endPoint = _endPoint;
        }
    }
}