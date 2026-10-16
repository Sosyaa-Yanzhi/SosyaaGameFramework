using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameFramework.Profile.Binary
{
    public class BinaryForProfile : MonoBehaviour
    {
        #region 保存存档到二进制文件
        public static void SaveProfileToBinary(GameProfileMap profile)
        {
            using (var stream = new FileStream(profile.profileFilePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(profile.profileName);
                    writer.Write((int)profile.profileType);
                    writer.Write((int)profile.profileFileType);
                    writer.Write(profile.profileDatas.Count);
                    foreach (var data in profile.profileDatas)
                    {
                        writer.Write(data.ProfileDataName);
                        writer.Write((int)data.ProfileDataType);
                        writer.Write(data.ProfileDataValue);
                    }
                }
            }
            Debug.Log($"存档文件保存成功，路径为 {profile.profileFilePath}");
        }
        #endregion

        #region 从二进制文件加载存档
        public static void LoadProfileFromBinary(ref GameProfileMap profile)
        {
            // 加载前先进行清空List
            profile.profileDatas.Clear();
            
            using (var stream = new FileStream(profile.profileFilePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    profile.profileName = reader.ReadString();
                    profile.profileType = (ProfileType)reader.ReadInt32();
                    profile.profileFileType = (ProfileFileType)reader.ReadInt32();
                    int dataCount = reader.ReadInt32();
                    profile.profileDatas.Clear();
                    for (int i = 0; i < dataCount; i++)
                    {
                        var dataName = reader.ReadString();
                        var dataType = (DataType)reader.ReadInt32();
                        var dataValue = reader.ReadString();
                        profile.profileDatas.Add(new ProfileData(dataName, dataType) { ProfileDataValue = dataValue });
                    }
                }
            }
            Debug.Log($"存档文件加载成功，路径为 {profile.profileFilePath}");
        }
        #endregion
    }
}
