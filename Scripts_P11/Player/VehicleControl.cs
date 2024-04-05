using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Project_11;

namespace Project_11
{
    public class VehicleControl : MonoBehaviour
    {
        #region variable
        Rigidbody mainRigidbody; // 본체에 사용되는 리지드바디
        public Vector3 vehicle_Velocity => mainRigidbody.velocity;

        [Tooltip("세팅 오브젝트")]
        [SerializeField] WheelCollider[] wheelColliders;  // 동력에 사용되는 휠 콜라이더
        [SerializeField] WheelCollider[] steeringWheels;  // 조향되는 휠 콜라이더
        [SerializeField] GameObject[] wheelMeshes;  // 타이어 메쉬 게임 오브젝트

        // 입력
        float verticalInput => Input.GetAxis("Vertical");
        float horizontalInput => Input.GetAxis("Horizontal");
        #endregion

        #region Singleton
        // 싱글톤
        private static VehicleControl DC;
        public static VehicleControl singleton
        {
            get
            {
                if (DC == null)
                {
                    DC = FindObjectOfType<VehicleControl>();
                    if (DC == null) Debug.Log("Drive_Control를 사용하려 했지만, 없어요.");
                }
                return DC;
            }
        }
        #endregion

        #region Lifecycle_Fucntion
        private void Awake()
        {
            mainRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            WheelAnimation();
        }

        private void FixedUpdate()
        {
            if (PlayerManager.singleton.isDead) return;
            Driving();
            ClampVelocity();
            ClampWheelRPM();
        }
        #endregion


        #region Fucntion
        // 차량을 기동시킨다.
        void Driving()
        {   
            if(PlayerManager.singleton.isDead) return;

            // 조작 - 전후진        
            if (verticalInput != 0)
            {
                foreach (var i in wheelColliders)
                {
                    i.brakeTorque = 0;
                    i.motorTorque = (PlayerManager.singleton.maxTotalTorque / wheelColliders.Length) * verticalInput;
                }
            }
            else foreach (var i in wheelColliders) i.brakeTorque = PlayerManager.singleton.breakingTorque;

            // 조작 - 좌우회전
            foreach (var i in steeringWheels)
            {
                if (horizontalInput != 0) i.steerAngle = PlayerManager.singleton.steerAngle * horizontalInput;
                else i.steerAngle = 0;
            }
        }

        // 최대 속도를 제한한다.
        void ClampVelocity()
        {
            if (PlayerManager.singleton.vehicleVelocity > PlayerManager.singleton.topSpeed)
                mainRigidbody.velocity = (PlayerManager.singleton.topSpeed / 3.6f) * vehicle_Velocity.normalized;  // 3축에 속도 제한이 적용됨
        }

        // 바퀴의 최대 RPM을 제한한다. (속도에 비해, 바퀴가 헛도는 현상을 완화하기 위해서)
        void ClampWheelRPM()
        {
            foreach (var i in wheelColliders)
            {
                // 현재 RPM 가져오기
                float currentRPM = i.rpm;

                // 전진 방향의 휠 RPM 제한
                if (currentRPM > PlayerManager.singleton.maxRPM)
                {
                    // 최대 RPM으로 설정
                    i.motorTorque = 0;
                    i.brakeTorque = i.mass * PlayerManager.singleton.maxRPM / 60.0f;
                }
                // 후진 방향의 휠 RPM 제한
                else if (currentRPM < -PlayerManager.singleton.maxRPM)
                {
                    // 최소 RPM으로 설정
                    i.motorTorque = 0;
                    i.brakeTorque = i.mass * -PlayerManager.singleton.maxRPM / 60.0f;
                }
            }
        }

        // 차량 움직임의 반대 방향으로 힘을 가한다.
        public void ForceBody(int _Force)
        {
            mainRigidbody.AddForce(-mainRigidbody.velocity.normalized * _Force, ForceMode.Impulse);
        }
        #endregion


        #region Animation
        // 바퀴 메쉬 Transform을 휠 콜라이더에 동기화시킨다.
        void WheelAnimation()
        {
            // 타이어 메쉬 애니메이션
            for (int i = 0; i < wheelColliders.Length; i++)
            {
                Vector3 position;
                Quaternion quat;
                wheelColliders[i].GetWorldPose(out position, out quat);
                wheelMeshes[i].transform.position = position;
                wheelMeshes[i].transform.rotation = quat;
            }
        }
        #endregion
    }
}