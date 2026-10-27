using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameFramework.Profile.Binary
{
    public class BinaryForProfile : MonoBehaviour
    {
        #region 保存存档到二进制文件
        public static void SaveProfileToBinary(GameProfileMap profileMap)
        {
            using (var stream = new FileStream(profileMap.profileFilePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(profileMap.profileName);
                    writer.Write((int)profileMap.profileType);
                    writer.Write((int)profileMap.profileFileType);
                    writer.Write(profileMap.profileDatas.Count);
                    foreach (var data in profileMap.profileDatas)
                    {
                        writer.Write(data.Key);
                        writer.Write(data.Value.ProfileDataName);
                        writer.Write((int)data.Value.ProfileDataType);
                        writer.Write(data.Value.ProfileDataValue);
                    }
                }
            }
            Debug.Log($"存档文件保存成功，路径为 {profileMap.profileFilePath}");
        }
        #endregion

        #region 从二进制文件加载存档
        public static void LoadProfileFromBinary(ref GameProfileMap profileMap)
        {
            // 加载前先进行清空List
            profileMap.profileDatas.Clear();
            
            using (var stream = new FileStream(profileMap.profileFilePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    profileMap.profileName = reader.ReadString();
                    profileMap.profileType = (ProfileType)reader.ReadInt32();
                    profileMap.profileFileType = (ProfileFileType)reader.ReadInt32();
                    int dataCount = reader.ReadInt32();
                    profileMap.profileDatas.Clear();
                    for (int i = 0; i < dataCount; i++)
                    {
                        var dataName = reader.ReadString();
                        var dataType = (DataType)reader.ReadInt32();
                        var dataValue = reader.ReadString();
                        profileMap.profileDatas.Add(dataName , new ProfileData(dataName, dataType) { ProfileDataValue = dataValue });
                    }
                }
            }
            Debug.Log($"存档文件加载成功，路径为 {profileMap.profileFilePath}");
        }
        #endregion
    }
}
