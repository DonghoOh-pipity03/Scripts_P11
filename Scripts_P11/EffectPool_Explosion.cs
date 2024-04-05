using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class EffectPool_Explosion : MonoBehaviour, IEffect
    {
        [SerializeField] GameObject explosionPrefab;
        [SerializeField] int poolSize;
        private Queue<GameObject> objectPool;

        private void Awake()
        {
            // 오브젝트 풀 초기 생성
            objectPool = new Queue<GameObject>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(explosionPrefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
        }

        public GameObject GetPooledObject()
        {
            if (objectPool.Count == 0) CreateAndEnqueueObject();
            GameObject obj = objectPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        public void CreateAndEnqueueObject()
        {
            GameObject obj = Instantiate(explosionPrefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        public void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }
}