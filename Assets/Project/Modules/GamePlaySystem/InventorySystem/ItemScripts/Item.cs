using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramework.GameplaySystem.InventorySystem
{
    /// <summary>
    /// 注意：强烈建议 Item 的 transform.position 同其成员下的
    /// pivotSlot 重合（使二者坐标锚点重合），以保持视觉有好的情况下方便后续逻辑的执行。 
    /// </summary>
    public class Item : MonoBehaviour
    {
        /// <summary>
        /// itemSlots 列表用于存放属于物体本身的网格。
        /// </summary>
        public readonly List<InventorySlot> itemSlots = new List<InventorySlot>();

        /// <summary>
        /// bIsInventory 表明物品是否在任意某一个容器的范围内。
        /// </summary>
        public bool bIsInInventory = false;

        /// <summary>
        /// bIsPut 表明物品是否被放置在任意某个容器中。
        /// </summary>
        public bool bIsPut = false;
        
        /// <summary>
        /// _currentInventory 用于记录物品当前所在的容器，
        /// （当物品被放入某个容器 & 进入某个容器范围内，此成员需被更新）。
        /// </summary>
        public Inventory _currentInventory;

        /// <summary>
        /// recordedPosition 用于记录物品之前的位置（所在容器 & 相对应的坐标系下的逻辑坐标）。
        /// </summary>
        public Dictionary<Inventory , Vector2> recordedPosition;

        /// <summary>
        /// 物品被放入容器时事件
        /// </summary>
        public Action<Inventory , Vector2> OnIntoInventory;

        /// <summary>
        /// 物品被从容器中取出时事件
        /// </summary>
        public Action<Inventory> OnOutOfInventory;

        /// <summary>
        /// 物品进入容器范围时事件
        /// </summary>
        public Action<Inventory> OnEnterInventory;

        /// <summary>
        /// 物品离开容器范围时事件
        /// </summary>
        public Action<Inventory> OnLeaveInventory;

        /// <summary>
        /// 搜寻到容器网格时事件
        /// </summary>
        public Action<Inventory , InventorySlot> OnInventorySlotDetected;

        /// <summary>
        /// pivotSlot 物品的锚点网格
        /// </summary>
        private InventorySlot pivotSlot;

        [SerializeField]private float detectDistance;

        private GameObject originParent;
        private GameObject temporaryParent;
        private List<InventorySlot> idealSlots = new List<InventorySlot>();

        void Start()
        {
            originParent = transform.parent.gameObject;

            OnIntoInventory += (inventory , targetPosition) =>
            {
                bIsPut = true;
                recordedPosition.Add(inventory , targetPosition);
            };

            OnOutOfInventory += (inventory) =>
            {
                bIsPut = false;
            };

            OnEnterInventory += (inventory) =>
            {
                _currentInventory = inventory;
                bIsInInventory = true;
                DetectInventorySlot(inventory);
            };

            OnLeaveInventory += (inventory) =>
            {
                _currentInventory = null;
                bIsInInventory = false;
            };

            OnInventorySlotDetected += (inventory , inventorySlot) =>
            {
                if (FindIdealSlot(inventory , inventorySlot))
                {
                    
                }
            };
        }

        #region 网格间距离换算逻辑
        float GetDistanceBetweenSlots(InventorySlot itemSlot , InventorySlot inventorySlot)
        {
            float distance;
            itemSlot.transform.parent = inventorySlot.transform.parent;
            distance = Vector2.Distance(
                itemSlot.GetComponent<RectTransform>().anchoredPosition,
                inventorySlot.GetComponent<RectTransform>().anchoredPosition
            );
            itemSlot.transform.parent = transform;
            return distance;
        }
        #endregion

        #region 网格搜寻逻辑
        void DetectInventorySlot(Inventory inventory)
        {
            foreach (var slot in inventory.inventorySlots)
            {
                if (detectDistance >= GetDistanceBetweenSlots(pivotSlot , slot))
                {
                    OnInventorySlotDetected?.Invoke(inventory , slot);
                }
            }
        }
        #endregion

        #region 网格可匹配性检查
        bool FindIdealSlot(Inventory inventory , InventorySlot inventorySlot)
        {
            // 清空缓存的“理想”网格
            idealSlots.Clear();

            // 遍历物体自身网格，比较容器相应范围内网格（以搜寻到的网格为起点）
            Vector2 slotOffset = new Vector2(
                pivotSlot.slotPosition.x - inventorySlot.slotPosition.x ,
                pivotSlot.slotPosition.y - inventorySlot.slotPosition.y
            );

            foreach (var itemSlot in itemSlots)
            {
                var idealSlot = inventory.GetInventorySlot(itemSlot.slotPosition);
                if (idealSlot == null)
                {
                    return false;
                }
                idealSlots.Add(idealSlot);
            }
            return true;
        }
        #endregion

        #region 吸附逻辑
        void AttachToInventorySlot(Inventory inventory , InventorySlot targetSlot)
        {
            // 进行父级转换
            temporaryParent = inventory.gameObject;
            transform.parent = temporaryParent.transform;
            // 坐标吸附
            GetComponent<RectTransform>().anchoredPosition = targetSlot.GetComponent<RectTransform>().anchoredPosition;
            OnIntoInventory?.Invoke(inventory , targetSlot.slotPosition);
        }
        #endregion
    }
}
