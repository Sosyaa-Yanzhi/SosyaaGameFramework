using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameFramework.UI
{
    public class InteractableComp : MonoBehaviour , IBeginDragHandler , IDragHandler , IEndDragHandler
    {
        /// <summary>
        /// 鼠标是否进入/悬浮于UI控件
        /// </summary>
        private bool bHasPointerEntered = false;

        /// <summary>
        /// 标记是否开始跟随指针
        /// </summary>
        private bool bIsFollowingPointer = false;

        private CanvasGroup canvasGroup;
        private bool blocksRaycastsBeforeDrag;

        public Action OnDragStart;
        public Action DuringDrag;
        public Action OnDragEnd;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        #region 指针事件
        public void OnBeginDrag(PointerEventData data)
        {
            bIsFollowingPointer = true;
            OnDragStart?.Invoke();

            if (canvasGroup != null)
            {
                blocksRaycastsBeforeDrag = canvasGroup.blocksRaycasts;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void OnDrag(PointerEventData data)
        {
            FollowPointer(data);
            DuringDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData data)
        {
            bIsFollowingPointer = false;
            try
            {
                OnDragEnd?.Invoke();
            }
            finally
            {
                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = blocksRaycastsBeforeDrag;
                }
            }
        }
        #endregion

        #region 跟随指针移动
        void FollowPointer(PointerEventData data)
        {
           transform.position += (Vector3)data.delta;
        }
        #endregion
    }
}
