using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Project_14;

public class Button_Right : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData) => MoveController.singleton.clicked_right = true;
    public void OnPointerUp(PointerEventData eventData) => MoveController.singleton.clicked_right = false;
}
