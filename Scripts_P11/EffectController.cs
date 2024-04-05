using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_11;

namespace Project_11
{
    public class EffectController : MonoBehaviour
    {
        [SerializeField] EffectKind effectKind;
        [SerializeField] float effectTime;


        private void OnEnable() 
        {
            StartCoroutine(Disapear());
        }

        IEnumerator Disapear()
        {
            yield return new WaitForSeconds(effectTime);

            EffectManager.singleton.ReturnToPool(effectKind, this.gameObject);
        }
    }
}