using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace GameFramework.Profile
{
    public class ProfileManager : MonoBehaviour
    {
        #region 单例实现
        static private ProfileManager _instance;
        static public ProfileManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ProfileManager>();
                    if (_instance == null)
                    {
                        GameObject profileManagerObject = new GameObject("ProfileManager");
                        _instance = profileManagerObject.AddComponent<ProfileManager>();
                        DontDestroyOnLoad(profileManagerObject);
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region 成员
        // 游戏存档列表
        public List<GameProfileMap> gameProfiles = new List<GameProfileMap>();
        #endregion
        
        #region 加载存档文件
        public void LoadGameProfiles(ref GameProfileMap GameProfile_toLoad)
        {
            // 如果路径为空，直接返回
            if (string.IsNullOrEmpty(GameProfile_toLoad.profileFilePath))
            {
                Debug.LogError("存档文件加载失败：存档路径为空。");
                return;
            }
            // 如果有相关存档文件，直接进行读取
            if (File.Exists(GameProfile_toLoad.profileFilePath))
            {
                string content = File.ReadAllText(GameProfile_toLoad.profileFilePath);
                switch (GameProfile_toLoad.profileFileType)
                {
                    case ProfileFileType.Json:
                        JsonConvert.PopulateObject(content, GameProfile_toLoad);
                        break;
                    case ProfileFileType.Binary:
                        Binary.BinaryForProfile.LoadProfileFromBinary(ref GameProfile_toLoad);
                        break;
                    case ProfileFileType.Xml:
                        XML.XMLForProfile.LoadProfileFromXML(ref GameProfile_toLoad);
                        break;
                    default:
                        Debug.LogError("存档文件加载失败：不支持的存档文件类型。");
                        break;
                }
            }
            // 如果没有相关存档文件，在建立文件前进行询问
            else
            {
                Debug.LogError("存档文件加载失败：未找到指定路径的存档文件。");
            }
        }
        #endregion

        #region 存档表写入
        public void WriteProfileToFile(GameProfileMap profileMap_toWrite)
        {
            // 若存档路径重复，直接返回
            if (gameProfiles.Any(p => p.profileFilePath == profileMap_toWrite.profileFilePath))
            {
                Debug.LogError($"{profileMap_toWrite.profileName}保存失败！存档路径重复！");
                return;
            }

            // 如果路径为空，直接返回
            if (string.IsNullOrEmpty(profileMap_toWrite.profileFilePath))
            {
                Debug.LogError($"{profileMap_toWrite.profileName}保存失败！存档路径为空！");
                return;
            }

            // 检查存档内容是否合法
            if (!profileMap_toWrite.CheckBeforeSaving())
            {
                Debug.LogError($"{profileMap_toWrite.profileName}保存失败！请检查数据内容是否符合类型要求！");
                return;
            }

            switch (profileMap_toWrite.profileFileType)
            {
                case ProfileFileType.Json:
                    string json = JsonConvert.SerializeObject(profileMap_toWrite, Formatting.Indented);
                    File.WriteAllTextAsync(profileMap_toWrite.profileFilePath, json);
                    break;
                case ProfileFileType.Binary:
                    Binary.BinaryForProfile.SaveProfileToBinary(profileMap_toWrite);
                    break;
                case ProfileFileType.Xml:
                    XML.XMLForProfile.SaveProfileToXML(profileMap_toWrite);
                    break;
                default:
                    Debug.LogError($"{profileMap_toWrite.profileName}保存失败！不支持的存档文件类型！");
                    break;
            }

            gameProfiles.Add(profileMap_toWrite);
        }   
        #endregion

        #region 保存/更新存档文件
        public void SaveGameProfiles(GameProfileMap GameProfile_toSave)
        {
            WriteProfileToFile(GameProfile_toSave);
        }
        #endregion
    }
}
