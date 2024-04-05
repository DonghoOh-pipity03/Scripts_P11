using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;
using UnityEngine.EventSystems;

namespace Project_14
{
    public class Button_Attack : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        
        public void OnPointerUp(PointerEventData eventData)
        {
            Hands.singleton.Act_DeTrigger();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            Hands.singleton.Act_Trigger();
        }
    }
}