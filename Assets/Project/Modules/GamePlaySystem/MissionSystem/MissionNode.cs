using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;

namespace GameFramework.GameplaySystem.MissionSystem
{
    [CreateAssetMenu(fileName = "NewMissionNode" , menuName = "ScriptableObject/GameplaySystem/MissionSystem/CreateMissionNode")]
    public class MissionNode : ScriptableObject
    {
        [Header("是否为首任务")]
        public bool bIsHeadMissionNode = false;
        [Header("任务类型参照表")]
        public MissionTypeTable _missionTypeTable;
        [Header("任务类型")]
        public string MissionType = "unknown";
        [Header("任务名称")]
        public string MissionName = "unknown";
        [Header("任务内容描述")]
        public TextAsset MissionContent;
        [Header("任务解锁状态")]
        public bool bMissionUnlocked = false;
        [Header("前置任务节点")]
        public List<MissionNode> PrerequisiteMissionNodes = new List<MissionNode>();
        [Header("后续任务节点")]
        public List<MissionNode> FollowUpMissionNodes = new List<MissionNode>();
    }

    [CustomEditor(typeof(MissionNode))]
    public class MissionNodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var missionNode = (MissionNode)target;

            serializedObject.Update(); // 加载当前SO的数据到暂存区

            DrawDefaultInspector();

            if (GUILayout.Button("Apply"))
            {
                // 读取暂存区中的数据进行检查
                SerializedProperty serializedProperty = serializedObject.FindProperty("MissionType");

                // 实际保存前进行数据合法性检测
                if (missionNode._missionTypeTable == null ||
                    missionNode.MissionType == null ||
                    missionNode.MissionContent == null ||
                    missionNode.MissionName == null)
                {
                    serializedObject.Update();
                    EditorUtility.DisplayDialog("错误" , "任务节点有成员属性为空，已恢复默认值。" , "确定");
                }


                bool isValid = CheckValidity(missionNode.MissionType , missionNode._missionTypeTable.MissionTypeList);
                if (!isValid)
                {
                    serializedObject.Update(); // 重新从 SO 加载，覆盖掉暂存区里的非法改动
                    EditorUtility.DisplayDialog("错误", "当前所配置的任务种类不存在，已恢复默认值。", "确定");
                }
                else
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        bool CheckValidity(string missionType , List<string> typeTable)
        {
            foreach (string type in typeTable)
            {
                if (missionType == type)
                return true;
            }
            return false;
        }
    }
}
