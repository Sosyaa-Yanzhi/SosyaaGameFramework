using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GameFramework.UI;
using UnityEngine.EventSystems;


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
        public (Inventory recordedInventory , Vector2 recordedSlotPosition) recordedPosition;

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

        public void NotifyInventoryEntered(Inventory inventory)
        {
            OnEnterInventory?.Invoke(inventory);
        }

        public void NotifyInventoryExited(Inventory inventory)
        {
            OnLeaveInventory?.Invoke(inventory);
        }

        /// <summary>
        /// pivotSlot 物品的锚点网格
        /// </summary>
        public InventorySlot pivotSlot;

        [SerializeField]private float detectDistance;

        private GameObject originParent;
        private GameObject temporaryParent;
        private List<InventorySlot> idealSlots = new List<InventorySlot>();
        private List<InventorySlot> placedSlots = new List<InventorySlot>();
        private InteractableComp interactableComp;

        void Start()
        {
            itemSlots.Clear();
            itemSlots.AddRange(GetComponentsInChildren<InventorySlot>(true));
            if (itemSlots.Count == 0 || pivotSlot == null || !itemSlots.Contains(pivotSlot))
            {
                Debug.LogError($"{name} 的物品网格列表为空，或锚点网格不属于该物品。", this);
            }

            originParent = transform.parent.gameObject;

            OnIntoInventory += (inventory , targetPosition) =>
            {
                bIsPut = true;
                var inventoryItemManager = inventory.gameObject.GetComponent<InventoryItemManager>();
                if (inventoryItemManager != null && !inventoryItemManager.itemsInInventory.Contains(this))
                {
                    inventoryItemManager.itemsInInventory.Add(this);
                }

                placedSlots.Clear();
                foreach (var slot in idealSlots)
                {
                    if (slot == null) continue;
                    slot.bIsSlotMatched = true;
                    placedSlots.Add(slot);
                }

                foreach (var slot in itemSlots)
                {
                    if (slot != null)
                    {
                        slot.bIsSlotMatched = true;
                    }
                }
            };

            OnOutOfInventory += (inventory) =>
            {
                bIsPut = false;

                if (inventory != null)
                {
                    inventory.gameObject.GetComponent<InventoryItemManager>().itemsInInventory.Remove(GetComponent<Item>());
                }

                foreach (var slot in placedSlots)
                {
                    if (slot != null)
                    {
                        slot.bIsSlotMatched = false;
                    }
                }
                placedSlots.Clear();

                foreach (var slot in itemSlots)
                {
                    if (slot != null)
                    {
                        slot.bIsSlotMatched = false;
                    }
                }
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
                    recordedPosition = (inventory , inventorySlot.slotPosition);
                }
            };

            interactableComp = GetComponent<InteractableComp>();
            if (interactableComp != null)
            {
                interactableComp.OnDragEnd += () =>
                {
                    try
                    {
                        var recordedInventory = recordedPosition.recordedInventory;
                        if (recordedInventory == null)
                        {
                            Debug.LogWarning($"{name} 拖拽结束时没有检测到可用的容器网格，跳过吸附。", this);
                            return;
                        }

                        var targetSlot = recordedInventory.GetInventorySlot(recordedPosition.recordedSlotPosition);
                        if (targetSlot == null)
                        {
                            Debug.LogWarning($"{name} 记录的容器网格已不存在，跳过吸附。", this);
                            return;
                        }

                        AttachToInventorySlot(recordedInventory, targetSlot);
                    }
                    finally
                    {
                        InventoryManager.Instance.EndItemDrag(this);
                    }
                };

                interactableComp.OnDragStart += () =>
                {
                    recordedPosition = (null, Vector2.zero);
                    OnOutOfInventory?.Invoke(_currentInventory);
                    DetectInventorySlot(_currentInventory);

                    InventoryManager.Instance.BeginItemDrag(this);
                };

                interactableComp.DuringDrag += () =>
                {
                    if (_currentInventory != null)
                    {
                        DetectInventorySlot(_currentInventory);
                    }
                };
            }
            else
            {
                Debug.LogWarning("item 未成功获取 interactableComp 组件");
            }
        }

        #region 网格间距离换算逻辑
        float GetDistanceBetweenSlots(InventorySlot itemSlot , InventorySlot inventorySlot)
        {
            if (itemSlot == null || inventorySlot == null)
            {
                return float.PositiveInfinity;
            }

            var itemRectTransform = itemSlot.GetComponent<RectTransform>();
            var inventoryRectTransform = inventorySlot.GetComponent<RectTransform>();
            if (itemRectTransform == null || inventoryRectTransform == null)
            {
                Debug.LogWarning($"{name} 网格缺少 RectTransform，无法计算检测距离。", this);
                return float.PositiveInfinity;
            }

            var coordinateSpace = inventoryRectTransform.parent;
            Vector3 itemPosition = coordinateSpace != null
                ? coordinateSpace.InverseTransformPoint(itemRectTransform.position)
                : itemRectTransform.position;
            Vector3 inventoryPosition = coordinateSpace != null
                ? coordinateSpace.InverseTransformPoint(inventoryRectTransform.position)
                : inventoryRectTransform.position;

            return Vector2.Distance(itemPosition, inventoryPosition);
        }
        #endregion

        #region 网格搜寻逻辑
        void DetectInventorySlot(Inventory inventory)
        {
            if (!bIsInInventory) return;
            if (inventory == null)
            {
                Debug.LogWarning($"{name} 标记为处于容器范围内，但未关联有效容器，跳过网格检测。", this);
                return;
            }
            if (pivotSlot == null)
            {
                Debug.LogWarning($"{name} 未设置锚点网格，跳过容器网格检测。", this);
                return;
            }

            var inventorySlots = inventory.inventorySlots;
            if (inventorySlots == null)
            {
                Debug.LogError($"{inventory.name} 的网格列表尚未初始化，无法检测物品网格。", inventory);
                return;
            }

            recordedPosition = (null, Vector2.zero);
            InventorySlot nearestSlot = null;
            float nearestDistance = float.PositiveInfinity;

            foreach (var slot in inventorySlots)
            {
                if (slot == null) continue;

                float distance = GetDistanceBetweenSlots(pivotSlot, slot);
                if (float.IsNaN(distance) || float.IsInfinity(distance) ||
                    distance > detectDistance || distance >= nearestDistance ||
                    !FindIdealSlot(inventory, slot))
                    continue;

                nearestSlot = slot;
                nearestDistance = distance;
            }

            if (nearestSlot != null)
            {
                OnInventorySlotDetected?.Invoke(inventory, nearestSlot);
            }
        }
        #endregion

        #region 网格可匹配性检查
        bool FindIdealSlot(Inventory inventory , InventorySlot inventorySlot)
        {
            if (inventory == null || inventorySlot == null || pivotSlot == null ||
                itemSlots.Count == 0 || !itemSlots.Contains(pivotSlot))
            {
                return false;
            }

            // 清空缓存的“理想”网格
            idealSlots.Clear();

            // 遍历物体自身网格，比较容器相应范围内网格（以搜寻到的网格为起点）
            Vector2 slotOffset = new Vector2(
                pivotSlot.slotPosition.x - inventorySlot.slotPosition.x ,
                pivotSlot.slotPosition.y - inventorySlot.slotPosition.y
            );

            foreach (var itemSlot in itemSlots)
            {
                if (itemSlot == null)
                {
                    return false;
                }

                var idealSlot = inventory.GetInventorySlot(itemSlot.slotPosition - slotOffset);
                if (idealSlot == null || idealSlot.bIsSlotMatched)
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
            if (inventory == null || targetSlot == null)
            {
                Debug.LogWarning($"{name} 无法吸附到无效的容器网格。", this);
                return;
            }

            // 进行父级转换
            temporaryParent = inventory.gameObject;
            transform.SetParent(temporaryParent.transform, true);
            // 坐标吸附
            GetComponent<RectTransform>().anchoredPosition = targetSlot.GetComponent<RectTransform>().anchoredPosition;
            OnIntoInventory?.Invoke(inventory , targetSlot.slotPosition);

            transform.SetParent(originParent.transform, true);
        }
        #endregion
    }
}
