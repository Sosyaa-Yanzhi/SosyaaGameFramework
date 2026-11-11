using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

namespace GameFramework.GameplaySystem.MissionSystem
{
    [CreateAssetMenu(fileName = "MissionTypeTable" , menuName = "ScriptableObject/GameplaySystem/MissionSystem/CreateMissionTypeTable" , order = 1)]
    public class MissionTypeTable : ScriptableObject
    {
        [Header("任务种类列表")]
        [Tooltip("用于提供声明任务种类时的参照表，为防止任务种类添加错误，应在声明任务种类后进行一次对照查错")]
        public List<string> MissionTypeList = new List<string>();
    }
}
