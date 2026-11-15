using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace GameFramework.GameplaySystem.InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        /// <summary>
        /// inventoryID 容器id，每个容器（独立的网格阵）都应有单独的ID，用于后续的检索。
        /// </summary>
        public int inventoryID {private set; get;}

        /// <summary>
        /// inventorySlots 容器属下网格列表。
        /// </summary>
        [Header("容器属下网格列表")]
        public readonly List<InventorySlot> inventorySlots = new List<InventorySlot>();
    
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
    }
}
