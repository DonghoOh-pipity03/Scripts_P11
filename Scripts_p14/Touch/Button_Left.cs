using System.Collections;
using System.Collections.Generic;
using Project_14;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Left : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData) => MoveController.singleton.clicked_left = true;
    public void OnPointerUp(PointerEventData eventData) => MoveController.singleton.clicked_left = false;
}
