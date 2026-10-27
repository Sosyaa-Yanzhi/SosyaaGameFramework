using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace GameFramework.Profile.XML
{
    public class GameProfileMapXML
    {
        public string profileName;
        public ProfileType profileType;
        public ProfileFileType profileFileType;
        public string profileFilePath;
        public Dictionary<string, ProfileData> profileDatas = new Dictionary<string, ProfileData>();

        public GameProfileMapXML() { }
    }
    public class XMLForProfile : MonoBehaviour
    {
        #region 保存存档到XML文件
        public static void SaveProfileToXML(GameProfileMap profileMap)
        {
            var xmlProfile = new GameProfileMapXML
            {
                profileName = profileMap.profileName,
                profileType = profileMap.profileType,
                profileFileType = profileMap.profileFileType,
                profileFilePath = profileMap.profileFilePath,
                profileDatas = profileMap.profileDatas ?? new Dictionary<string, ProfileData>()
            };

            var rootAttribute = new XmlRootAttribute(profileMap.profileName);
            var serializer = new XmlSerializer(typeof(GameProfileMapXML) , rootAttribute);
            using (var stream = new FileStream(profileMap.profileFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, xmlProfile);
            }
            Debug.Log($"存档文件保存成功，路径为 {profileMap.profileFilePath}");
        }
        #endregion

        #region 从XML文件加载存档
        public static void LoadProfileFromXML(ref GameProfileMap profileMap)
        {
            if (File.Exists(profileMap.profileFilePath))
            {
                var serializer = new XmlSerializer(typeof(GameProfileMapXML));
                using (var stream = new FileStream(profileMap.profileFilePath, FileMode.Open))
                {
                    var loadedProfile = (GameProfileMapXML)serializer.Deserialize(stream);
                    profileMap.profileName = loadedProfile.profileName;
                    profileMap.profileType = loadedProfile.profileType;
                    profileMap.profileFileType = loadedProfile.profileFileType;
                    profileMap.profileFilePath = loadedProfile.profileFilePath;
                    profileMap.profileDatas = new Dictionary<string, ProfileData>();
                    if (loadedProfile.profileDatas != null)
                    {
                        foreach (var kvp in loadedProfile.profileDatas)
                        {
                            profileMap.profileDatas.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                Debug.Log($"存档文件加载成功，路径为 {profileMap.profileFilePath}");
            }
            else
            {
                Debug.LogError($"存档文件加载失败：文件不存在，路径为 {profileMap.profileFilePath}");
            }
        }
        #endregion
    }
}
