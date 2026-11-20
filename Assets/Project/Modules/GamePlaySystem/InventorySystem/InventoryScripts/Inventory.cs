using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.GameplaySystem.InventorySystem
{
    public class Inventory : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler
    {
        /// <summary>
        /// inventoryID 容器id，每个容器（独立的网格阵）都应有单独的ID，用于后续的检索。
        /// </summary>
        public int inventoryID {private set; get;}

        /// <summary>
        /// inventorySlots 容器属下网格列表。
        /// </summary>
        [Header("容器属下网格列表")]
        public List<InventorySlot> inventorySlots = new List<InventorySlot>();

        public Inventory(int id)
        {
            inventoryID = id;
        }

        #region 获取某个坐标的网格
        public InventorySlot GetInventorySlot(Vector2 targetSlotPosition)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.slotPosition == targetSlotPosition)
                {
                    return slot;
                }
            }
            return null;
        }
        #endregion

        public void OnPointerEnter(PointerEventData data)
        {
            InventoryManager.Instance.NotifyInventoryEntered(this);
        }

        public void OnPointerExit(PointerEventData data)
        {
            InventoryManager.Instance.NotifyInventoryExited(this);
        }

        void Awake()
        {
            // 初始化网格列表
            inventorySlots = GetComponentsInChildren<InventorySlot>().ToList();
        }
    }
}
