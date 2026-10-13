using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        private List<GameProfile> gameProfiles = new List<GameProfile>();
        #endregion
        
        #region 加载存档文件
        public void LoadGameProfiles(string ProfilePath , ref GameProfile GameProfile_toLoad)
        {
            // 如果有相关存档文件，直接进行读取
            if (File.Exists(ProfilePath))
            {
                string content = File.ReadAllText(ProfilePath);
                GameProfile_toLoad = JsonUtility.FromJson<GameProfile>(content);
            }
            // 如果没有相关存档文件，在建立文件前进行询问
            else
            {
                
            }
        }
        #endregion

        #region 保存/更新存档文件
        public void SaveGameProfiles(string ProfilePath , GameProfile GameProfile_toSave)
        {
            string json = JsonUtility.ToJson(GameProfile_toSave , true);
            File.WriteAllTextAsync(ProfilePath , json);
        }
        #endregion
    }
}
