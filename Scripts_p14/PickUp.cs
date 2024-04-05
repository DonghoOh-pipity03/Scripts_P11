using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;
using Unity.VisualScripting;

namespace Project_14
{
    public class PickUp : MonoBehaviour
    {
        List<Mag> mags;

        // 싱글톤
        private static PickUp PU;
        public static PickUp singleton
        {
            get
            {
                if (PU == null)
                {
                    PU = FindObjectOfType<PickUp>();
                    if (PU == null) Debug.Log("PickUp을 사용하려 했지만, 없어요.");
                }
                return PU;
            }
        }


        public GameObject PickItem()
        {
            // 트리거 안에 있는 모든 Collider 배열을 가져옴
            Collider[] collidersInsideTrigger = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);

            foreach (Collider collider in collidersInsideTrigger)
            {
                Mag magComponent = collider.GetComponent<Mag>();
                if (magComponent != null) return magComponent.gameObject;
            }
            return null;
        }

        // 디버그 기즈모를 그리는 메서드
        void OnDrawGizmos()
        {
            // 트리거 영역을 디버그 기즈모로 그림
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}