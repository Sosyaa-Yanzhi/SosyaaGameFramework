using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.GameplaySystem.InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        /// <summary>
        /// 鼠标指针正在悬浮于上方的容器
        /// </summary>
        public Inventory pointerHoverInventory;

        /// <summary>
        /// 是否有物品正在被拖拽
        /// </summary>
        public bool isItemDragging = false;

        /// <summary>
        /// 正在被拖拽的物品
        /// </summary>
        public Item draggingItem;

        public void BeginItemDrag(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("无法开始拖拽物品：Item 为空。");
                return;
            }

            draggingItem = item;
            isItemDragging = true;
            pointerHoverInventory = null;
        }

        public void EndItemDrag(Item item)
        {
            if (draggingItem != item) return;

            draggingItem = null;
            isItemDragging = false;
            pointerHoverInventory = null;
        }

        public void NotifyInventoryEntered(Inventory inventory)
        {
            if (!isItemDragging || draggingItem == null || inventory == null) return;

            pointerHoverInventory = inventory;
            draggingItem.NotifyInventoryEntered(inventory);
        }

        public void NotifyInventoryExited(Inventory inventory)
        {
            if (pointerHoverInventory != inventory) return;

            pointerHoverInventory = null;
            if (isItemDragging && draggingItem != null)
            {
                draggingItem.NotifyInventoryExited(inventory);
            }
        }

        #region 单例实现
        static private InventoryManager _insctance;
        static public InventoryManager Instance
        {
            get
            {
                if (_insctance == null)
                {
                    _insctance = FindObjectOfType<InventoryManager>();

                    if (_insctance == null)
                    {
                        GameObject go = new GameObject("InventoryManager");
                        _insctance = go.AddComponent<InventoryManager>();

                        if (Application.isPlaying)
                        {
                            DontDestroyOnLoad(go);
                        }
                    }
                }
                return _insctance;
            }
        }
        #endregion
    }
}
