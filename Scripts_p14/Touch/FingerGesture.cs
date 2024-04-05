using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_14;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

namespace Project_14
{
    public class FingerGesture : MonoBehaviour
    {
        enum Swipe
        {
            None,
            Left,
            Right,
            Up,
            Down
        }
        float swipeThreshold = 50f; // 감지 감도
        bool checkedGesture = false;
        Swipe lastSwipe = Swipe.None;    // 마지막 인식 제스처


        private void Update()
        {
            FigureTouch();
        }

        void FigureTouch()
        {
            // 터치 입력이 있는지 확인
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // 첫 번째 터치만 고려

                // UI를 터치하지 않은 경우에만 일반적인 터치 입력 처리
                if (!IsPointerOverUIObject(touch.position))
                {
                    // 터치의 상대적인 위치를 구함
                    Vector2 touchPosition = touch.position;

                    if (touch.phase == TouchPhase.Began)
                    {
                        checkedGesture = false;
                        lastSwipe = Swipe.None;
                    }
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        // 터치가 움직이거나 정지해 있는 경우
                        DetectSwipeDirection(touch.deltaPosition);
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        // 제스처 확인이 안되었고, 움직임이 없었을 때, 탭으로 인식
                        if (!checkedGesture && touch.deltaPosition.magnitude < swipeThreshold)
                        {
                            Debug.Log("터치 - 탭");
                        }
                    }
                }
            }
        }

        void DetectSwipeDirection(Vector2 deltaPosition)
        {
            if (deltaPosition.magnitude > swipeThreshold)
            {
                float angle = Mathf.Atan2(deltaPosition.y, deltaPosition.x) * Mathf.Rad2Deg;

                if (angle < 45 && angle > -45)
                {
                    if (lastSwipe != Swipe.Right)
                    {
                        Debug.Log("오른쪽으로 스와이프");
                        checkedGesture = true;
                        lastSwipe = Swipe.Right;
                    }
                }
                else if (angle < -45 && angle > -135)
                {
                    if (lastSwipe != Swipe.Down)
                    {
                        Debug.Log("아래로 스와이프");
                        checkedGesture = true;
                        lastSwipe = Swipe.Down;

                        Hands.singleton.Act_Remove();
                    }
                }
                else if (angle < -135 || angle > 135)
                {
                    if (lastSwipe != Swipe.Left)
                    {
                        Debug.Log("왼쪽으로 스와이프");
                        checkedGesture = true;
                        lastSwipe = Swipe.Left;

                        Hands.singleton.Act_Slide();
                    }
                }
                else if (angle > 45 && angle < 135)
                {
                    if (lastSwipe != Swipe.Up)
                    {
                        Debug.Log("위로 스와이프");
                        checkedGesture = true;
                        lastSwipe = Swipe.Up;
                    }
                }
            }
        }

        bool IsPointerOverUIObject(Vector2 touchPosition)
        {
            // PointerEventData를 생성하여 현재 터치 위치로 초기화
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = touchPosition;

            // UI 요소 위에 포인터가 위치하는지 여부를 반환
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }
    }
}