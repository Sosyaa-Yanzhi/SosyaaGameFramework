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
        public List<ProfileData> profileDatas;

        public GameProfileMapXML() { }
    }
    public class XMLForProfile : MonoBehaviour
    {
        #region 保存存档到XML文件
        public static void SaveProfileToXML(GameProfileMap profile)
        {
            var xmlProfile = new GameProfileMapXML
            {
                profileName = profile.profileName,
                profileType = profile.profileType,
                profileFileType = profile.profileFileType,
                profileFilePath = profile.profileFilePath,
                profileDatas = profile.profileDatas
            };

            var rootAttribute = new XmlRootAttribute(profile.profileName);
            var serializer = new XmlSerializer(typeof(GameProfileMapXML) , rootAttribute);
            using (var stream = new FileStream(profile.profileFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, xmlProfile);
            }
            Debug.Log($"存档文件保存成功，路径为 {profile.profileFilePath}");
        }
        #endregion

        #region 从XML文件加载存档
        public static void LoadProfileFromXML(ref GameProfileMap profile)
        {
            if (File.Exists(profile.profileFilePath))
            {
                var serializer = new XmlSerializer(typeof(GameProfileMapXML));
                using (var stream = new FileStream(profile.profileFilePath, FileMode.Open))
                {
                    var loadedProfile = (GameProfileMapXML)serializer.Deserialize(stream);
                    profile.profileName = loadedProfile.profileName;
                    profile.profileFileType = loadedProfile.profileFileType;
                    profile.profileFilePath = loadedProfile.profileFilePath;
                    profile.profileDatas = loadedProfile.profileDatas;
                }
                Debug.Log($"存档文件加载成功，路径为 {profile.profileFilePath}");
            }
            else
            {
                Debug.LogError($"存档文件加载失败：文件不存在，路径为 {profile.profileFilePath}");
            }
        }
        #endregion
    }
}
