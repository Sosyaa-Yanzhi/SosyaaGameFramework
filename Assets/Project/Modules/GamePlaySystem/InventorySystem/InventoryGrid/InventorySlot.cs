using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.GameplaySystem.InventorySystem
{
    public class InventorySlot : MonoBehaviour
    {
        /// <summary>
        /// 网格逻辑坐标：指以网格阵的锚点网格为原点进行建系的坐标系上的坐标，
        /// 主要用于进行物体网格与容器网格的匹配。对于形状不规则的物体或容器，
        /// 此方案适用。但若容器网格及物品都是规则的（比如物体网格排布都是矩形），
        /// 建议更换其他方案以提升性能、减少时间消耗。
        /// 注意：对于每个锚点网格，都应将其逻辑坐标设置为(0f , 0f)。
        /// </summary>
        [Header("网格逻辑坐标")]
        public Vector2 slotPosition = new Vector2(0f , 0f);
        
        /// <summary>
        /// 若网格用于背包或其他容器的网格：有物体被放入容器后（物体的网格同背包网格匹配），
        /// 应将此状态切换为 true；
        /// 若网格用于物体本身，则当物体被成功放入容器后，需将其网格匹配状态都切换为true。
        /// </summary>
        [Header("网格占用匹配状态")]
        public bool bIsSlotMatched = false;

        /// <summary>
        /// 网格检测距离：
        /// 主要用于通过距离来进行网格之间的匹配检索，例如：当两个属于不同集合的网格靠近至该距离内，
        /// 一方会执行相关检测逻辑，从而决定另一方是否为可进行匹配的网格（可以说，这决定物品在容器中的
        /// 存放位置）。
        /// 注意：此距离检测建议仅用于锚点网格或其他特定网格即可，以提升效率。
        /// 如果是物品的话，可以添加/使用物品的检测距离，无需使用网格的数据。
        /// </summary>
        [SerializeField]public float _detecDistance{private set; get;}
        
        /// <summary>
        /// _targetInventorySlot 用于记录锚点网格通过 _detectDistance 锁定到的其他网格，
        /// 方便二者进行互通（例如吸附等逻辑）。
        /// </summary>
        public InventorySlot _targetInventorySlot;
    }
}
