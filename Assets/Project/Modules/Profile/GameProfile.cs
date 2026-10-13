using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Core;
using System;

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
        public object ProfileDataValue = 0;
        
    }

    public class GameProfile : ScriptableObject
    {
        [Header("存档种类")]
        public ProfileType profileType;
        [Header("存档信息")]
        public List<ProfileData> profileDatas;
    }
}
