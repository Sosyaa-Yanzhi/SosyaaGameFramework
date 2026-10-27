using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.GameplaySystem.MissionSystem
{
    public class MissionNode : ScriptableObject
    {
        [Header("任务名称")]
        public string MissionName;
        [Header("任务内容描述")]
        public TextAsset MissionContent;
        [Header("任务解锁状态")]
        public bool bMissionUnlocked;
        [Header("前置任务节点")]
        public MissionNode PrerequisiteMissionNode;
        [Header("后续任务节点")]
        public MissionNode FollowUpMissionNode;
    }
}
