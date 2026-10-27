using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Core;
using System;
using UnityEditor;
using GameFramework.Profile;
using System.Linq;
using System.IO;
using Newtonsoft.Json;


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
        public DataType ProfileDataType = DataType.Int;
        public string ProfileDataValue = "";

        public ProfileData() { }
        
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
                    ProfileDataValue = "content string";
                    break;
                case DataType.Boolean:
                    ProfileDataValue = "false";
                    break;
                case DataType.Dictionary:
                    ProfileDataValue = "{\"key\":\"value\"}";
                    break;
                case DataType.List:
                    ProfileDataValue = "[\"content_1\" , \"content_2\"]";
                    break;
                default:
                    ProfileDataValue = null;
                    break;
            }
        }
    }

    public class GameProfileMap : ScriptableObject
    {
        #region 成员
        [Header("存档种类")]
        public ProfileType profileType;
        [Header("存档名称")]
        public string profileName;
        [Header("存档内容")]
        public Dictionary<string , ProfileData> profileDatas;
        [Header("存档文件类型")]
        public ProfileFileType profileFileType = ProfileFileType.Json;
        [Header("存档文件路径")]
        public string profileFilePath;
        
        [Header("____________________________________\n新增条目配置表")]
        [JsonIgnore]
        public string newProfileName = "NewProfileName";
        [JsonIgnore]
        public DataType newProfileDataType = DataType.Int;
        #endregion

        #region 构造函数
        public GameProfileMap() { }
        public GameProfileMap(ProfileType type , string name)
        {
            profileType = type;
            profileName = name;
        }
        #endregion

        #region 添加存档内容字段
        // 合法性检测
        public bool IsNewProfileLegal()
        {
            // 进行检查：不允许同名字段出现
            foreach (var profileData in profileDatas.Values)
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
            profileDatas.Add(newProfileName, newProfileData);
        }
        #endregion

        #region 保存存档表前的检查
        public bool CheckBeforeSaving()
        {
            foreach (var profileData in profileDatas.Values)
            {
                switch (profileData.ProfileDataType)
                {
                    case DataType.Int:
                    {
                        if (!profileData.ProfileDataValue.All(char.IsDigit))
                        {
                            Debug.LogError($"{profileName}保存失败！数据内容同{profileData.ProfileDataType}类型不符！");
                            return false;
                        }
                        break;
                    }
                    case DataType.Float:
                    {
                        if (!float.TryParse(profileData.ProfileDataValue , out _))
                        {
                            Debug.LogError($"{profileName}保存失败！数据内容同{profileData.ProfileDataType}类型不符！");
                            return false;
                        }
                        break;
                    }
                    case DataType.Boolean:
                    {
                        if (!bool.TryParse(profileData.ProfileDataValue , out _))
                        {
                            Debug.LogError($"{profileName}保存失败！数据内容同{profileData.ProfileDataType}类型不符！");
                            return false;
                        }
                        break;
                    }
                    case DataType.String:
                    {
                        break;
                    }
                    case DataType.Dictionary:
                    {
                        if (!profileData.ProfileDataValue.StartsWith("<") || !profileData.ProfileDataValue.EndsWith(">"))
                        {
                            Debug.LogError($"{profileName}保存失败！数据内容同{profileData.ProfileDataType}类型不符！");
                            return false;
                        }
                        break;
                    }
                    case DataType.List:
                    {
                        if (!profileData.ProfileDataValue.StartsWith("[") || !profileData.ProfileDataValue.EndsWith("]") || !profileData.ProfileDataValue.Contains(","))
                        {
                            Debug.LogError($"{profileName}保存失败！数据内容同{profileData.ProfileDataType}类型不符！");
                            return false;
                        }
                        break;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Json文件解析方法
        public List<T> GetListFromJsonFile<T>(GameProfileMap gameProfileMap , ProfileData profileData)
        {
            if (gameProfileMap.profileFileType != ProfileFileType.Json)
            {
                Debug.LogWarning($"{profileData.ProfileDataName} 数据字段读取失败：数据表所设不是对应的文件类型。");
                return null;
            }
            if (profileData.ProfileDataType != DataType.List)
            {
                Debug.LogWarning($"{profileData.ProfileDataName} 数据字段读取失败：字段数据种类不匹配（不为DataType.List）。");
                return null;
            }
            return JsonConvert.DeserializeObject<List<T>>(profileData.ProfileDataValue);
        }

        public Dictionary<TKey , TValue> GetDictionaryFromJsonFile<TKey , TValue>(GameProfileMap gameProfileMap , ProfileData profileData)
        {
            if (gameProfileMap.profileFileType != ProfileFileType.Json)
            {
                Debug.LogWarning($"{profileData.ProfileDataName} 数据字段读取失败：数据表所设不是对应的文件类型。");
                return null;
            }
            if (profileData.ProfileDataType != DataType.Dictionary)
            {
                Debug.LogWarning($"{profileData.ProfileDataName} 数据字段读取失败：字段数据种类不匹配（不为DataType.Dictionary）。");
                return null;
            }
            return JsonConvert.DeserializeObject<Dictionary<TKey , TValue>>(profileData.ProfileDataValue);
        }
        #endregion
    
        #region XML文件解析方法
        #endregion

        #region Binary文件解析方法
        #endregion
    }

    #region 自定义编辑器
    [CustomEditor(typeof(GameProfileMap))]
    public class GameProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var profile = (GameProfileMap)target;

            var AddContentButton = GUILayout.Button("添加存档内容字段");
            if (AddContentButton)
            {
                if (profile.newProfileName == null)
                {
                    EditorGUILayout.HelpBox("新增字段的命名不能为空！" , MessageType.Error);
                    return;
                }
                if (!profile.IsNewProfileLegal())
                {
                    EditorGUILayout.HelpBox("新增字段命名不能同已有字段相同！" , MessageType.Error);
                    return;
                }
                // 检查流程通过后执行实际条目添加操作
                profile.AddNewProfileContent();
            }
            
            // 写入存档文件按钮及功能
            var WriteProfileButton = GUILayout.Button("写入存档文件");
            if (WriteProfileButton)
            {
                if (!profile.CheckBeforeSaving())
                {
                    EditorGUILayout.HelpBox("存档表保存失败！请检查数据内容是否符合类型要求！" , MessageType.Error);
                    return;
                }

                ProfileManager.Instance.WriteProfileToFile(profile);
            }

            // 删除存档文件按钮及功能
            var DeleteProfileButton = GUILayout.Button("删除存档文件");
            if (DeleteProfileButton)
            {
                if (File.Exists(profile.profileFilePath))
                {
                    File.Delete(profile.profileFilePath);
                    ProfileManager.Instance.gameProfiles.Remove(profile);
                    Debug.Log($"{profile.profileName}存档文件已删除！");
                }
                else
                {
                    Debug.LogError($"{profile.profileName}存档文件删除失败！未找到指定路径的存档文件！");
                }
            }

            // 预览存档文件内容按钮及功能
            var PreviewProfileButton = GUILayout.Button("预览存档文件内容");
            if (PreviewProfileButton)
            {
                if (File.Exists(profile.profileFilePath))
                {
                    string uri = new Uri(profile.profileFilePath).AbsoluteUri;
                    Application.OpenURL(uri);
                }
                else
                {
                    Debug.LogError($"{profile.profileName}存档文件预览失败！未找到指定路径的存档文件！");
                }
            }
        }
    }
    #endregion
}
