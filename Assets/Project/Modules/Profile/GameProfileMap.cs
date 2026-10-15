using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Core;
using System;
using UnityEditor;
using GameFramework.Profile;


namespace GameFramework.Profile
{
    public enum ProfileType
    {
        Setting,
        InGameData,
        AccountData
    }

    [Serializable]
    public class ProfileData
    {
        public string ProfileDataName = "NewProfileName";
        private DataType ProfileDataType = DataType.Int;
        public string ProfileDataValue = "";
         

        public ProfileData(string name = "NewProfileName" , DataType dataType = DataType.Int)
        {
            ProfileDataName = name;
            ProfileDataType = dataType;

            switch (dataType)
            {
                case DataType.Int:
                    ProfileDataValue = "0";
                    break;
                case DataType.Float:
                    ProfileDataValue = "0f";
                    break;
                case DataType.String:
                    ProfileDataValue = "";
                    break;
                case DataType.Boolean:
                    ProfileDataValue = "false";
                    break;
                case DataType.Dictionary:
                    ProfileDataValue = "<0,0>";
                    break;
                case DataType.List:
                    ProfileDataValue = "[0,0]";
                    break;
                default:
                    ProfileDataValue = null;
                    break;
            }
        }
    }

    public class GameProfileMap : ScriptableObject
    {
        [Header("存档种类")]
        public ProfileType profileType;
        [Header("存档名称")]
        public string profileName;
        [Header("存档内容")]
        public List<ProfileData> profileDatas;
        
        [Header("____________________________________\n新增条目配置表")]
        public string newProfileName = "NewProfileName";
        public DataType newProfileDataType = DataType.Int;

        #region 构造函数
        public GameProfileMap(ProfileType type , string name)
        {
            profileType = type;
            profileName = name;
        }
        #endregion

        #region 添加存档内容条目
        // 合法性检测
        public bool IsNewProfileLegal()
        {
            // 进行检查：不允许同名条目出现
            foreach (var profileData in profileDatas)
            {
                if (profileData.ProfileDataName == newProfileName)
                {
                    return false;
                }
            }
            return true;
        }

        // 执行添加逻辑
        public void AddNewProfileContent()
        {
            var newProfileData = new ProfileData(newProfileName , newProfileDataType);
            profileDatas.Add(newProfileData);
        }
        #endregion

        #region 保存存档表前的检查
        #endregion
    }

    [CustomEditor(typeof(GameProfileMap))]
    public class GameProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var profile = (GameProfileMap)target;

            var AddContentButton = GUILayout.Button("添加存档内容条目");
            if (AddContentButton)
            {
                if (profile.newProfileName == null)
                {
                    EditorGUILayout.HelpBox("新增条目的命名不能为空！" , MessageType.Error);
                    return;
                }
                if (!profile.IsNewProfileLegal())
                {
                    EditorGUILayout.HelpBox("新增条目命名不能同已有条目相同！" , MessageType.Error);
                    return;
                }
                // 检查流程通过后执行实际条目添加操作
                profile.AddNewProfileContent();
            }

            var WriteProfileButton = GUILayout.Button("写入存档文件");
            if (WriteProfileButton)
            {
                
            }
        }
    }
}
