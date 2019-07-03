using System;
using System.Collections.Generic;
using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.General.UI
{
    public static class UIUtility
    {
        public static bool isFocusedOnInput
        {
            get
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                    if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
                        return true;

                return false;
            }
        }

        public static GameObject currentlySelectedGameObject
        {
            get
            {
                if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
                    return null;

                return EventSystem.current.currentSelectedGameObject;
            }
        }

        public static bool isHoveringUIElement
        {
            get { return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(); }
        }

//        public static List<RaycastResult> RaycastAllHoveringUIObjects(Vector2 pos)
//        {
//            var raycastResults = new List<RaycastResult>();
//
//            if (EventSystem.current != null)
//            {
//                var eventData = new PointerEventData(EventSystem.current)
//                {
//                    position = pos
//                };
//
//                EventSystem.current.RaycastAll(eventData, raycastResults);
//            }
//
//            return raycastResults;
//        }

        public static void ResetTransform(Transform transform)
        {
            Assert.IsNotNull(transform, "Transform given is null");

            transform.localPosition = Vector3.zero;
            var rectTransform = transform.gameObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void InheritParentSize(Transform transform)
        {
            var r = transform.GetComponent<RectTransform>();
            Assert.IsNotNull(r, "No RectTransform found on " + transform.name);

            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.sizeDelta = Vector2.one;
            r.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}
