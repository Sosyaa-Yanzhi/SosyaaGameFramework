using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.GameplaySystem.InventorySystem
{
    /// <summary>
    /// InventoryItemManager 用于管理被放入此容器下的物品。
    /// </summary>
    public class InventoryItemManager : MonoBehaviour
    {
        public List<Item> itemsInInventory = new List<Item>();
        public Action<Item> OnItemPutInInventory;
        public Action<Item> OnItemOutOfInventory;

        void Start()
        {
            OnItemPutInInventory += (item) =>
            {
                itemsInInventory.Add(item);
            };

            OnItemOutOfInventory += (item) =>
            {
                itemsInInventory.Remove(item);
            };
        }
    }
}
