using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    // 이펙트 종류
    public enum EffectKind
    {
        Explosion
    }

    public class EffectManager : MonoBehaviour
    {
        private Dictionary<EffectKind, IEffect> effectDictionary;

        #region Singleton
        // 싱글톤
        private static EffectManager EM;
        public static EffectManager singleton
        {
            get
            {
                if (EM == null)
                {
                    EM = FindObjectOfType<EffectManager>();
                    if (EM == null) Debug.Log("EffectManager를 사용하려 했지만, 없어요.");
                }
                return EM;
            }
        }
        #endregion

        private void Awake()
        {
            // 구조체 생성
            effectDictionary = new Dictionary<EffectKind, IEffect>();

            // 구조체에 등록
            effectDictionary[EffectKind.Explosion] = GetComponent<EffectPool_Explosion>();
        }

        // 사용하기
        public GameObject GetPooledObject(EffectKind _effecKind)
        {
            if (effectDictionary.ContainsKey(_effecKind))
            {
                return effectDictionary[_effecKind].GetPooledObject();
            }
            return null;
        }

        // 반납하기
        public void ReturnToPool(EffectKind _effecKind, GameObject obj)
        {
            if (effectDictionary.ContainsKey(_effecKind))
            {
                effectDictionary[_effecKind].ReturnToPool(obj);
            }
        }

    }
}