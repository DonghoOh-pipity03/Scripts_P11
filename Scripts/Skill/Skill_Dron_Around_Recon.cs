using System.Collections;
using System.Collections.Generic;
using Project_11;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Project_11
{
    public class Skill_Dron_Around_Recon : MonoBehaviour
    {
        void Update()
        {
            DriveAround();
        }

        void DriveAround()
        {
            // 중심 오브젝트의 위치를 기준으로 회전 반지름을 고려하여 현재 오브젝트를 회전시킵니다.
            Vector3 newPos = PlayerManager.singleton.transform.position
                + Vector3.up * Skill_Dron.singleton.shootHeight
                - Quaternion.Euler(0, Skill_Dron.singleton.rotSpeed * Time.time, 0) * (Vector3.forward * Skill_Dron.singleton.rotRadius);
            transform.position = newPos;

            // 오브젝트의 방향을 회전 방향에 동기화한다.
            transform.rotation = Quaternion.Euler(0, Skill_Dron.singleton.rotSpeed * Time.time + 95, 0);
        }
    }
}