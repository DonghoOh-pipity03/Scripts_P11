using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class Item_EXP : MonoBehaviour, IItem
    {
        bool magneticed = false;
        float currentSpeed;
        private void Update()
        {
            Move();
        }

        public void Use(GameObject _user)
        {
            PlayerManager player = _user.GetComponent<PlayerManager>();
            if (player != null)
            {
                player.GetEXP(ItemManager.singleton.exp_val);
                Destroy(this.gameObject);
            }
        }

        public void Magneticed()
        {
            magneticed = true;
        }

        void Move()
        {
            if (magneticed)
            {
                // 플레이어 방향으로 가속하여 이동
                transform.position = Vector3.MoveTowards(transform.position, PlayerManager.singleton.transform.position, currentSpeed * Time.deltaTime);

                // 현재 속도 증가 (가속도 적용)
                currentSpeed += ItemManager.singleton.accelerationSpeed * Time.deltaTime;

                // 최대 속도 제한
                currentSpeed = Mathf.Clamp(currentSpeed, ItemManager.singleton.initialSpeed, ItemManager.singleton.maxSpeed);
            }
        }
    }
}