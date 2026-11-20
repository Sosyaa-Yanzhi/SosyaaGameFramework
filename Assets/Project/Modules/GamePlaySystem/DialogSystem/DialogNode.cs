using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.GameplaySystem.DialogSystem
{
    public class DialogNode : ScriptableObject
    {
        /// <summary>
        /// 发言的角色姓名
        /// </summary>
        [Header("发言角色")]
        public string speakerName;

        /// <summary>
        /// 角色所说台词 / 旁白内容
        /// </summary>
        [Header("对话内容")]
        public string dialogContent;

        /// <summary>
        /// 对话时发生的事件（如物品交付、角色动作触发等）
        /// </summary>
        [Header("对话时事件")]
        public Action<object> dialogAction;

        /// <summary>
        /// 后续直接接着的对话节点（可能会有多个节点，根据实际情况选择节点进行触发）
        /// </summary>
        [Header("后续对话节点")]
        public List<DialogNode> nextDialogNodes;
    }
}
