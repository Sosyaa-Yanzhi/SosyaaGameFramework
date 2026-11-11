using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.GameplaySystem.Achievement
{
    /// <summary>
    /// 此成就管理SO更多服务于UI显示，实际数据持久化应写入文件（例如Json）。
    /// 配合 GameProfileMap 的使用，可以仅在 Map 中添加以下字段：
    /// Dictionary<int , bool> （成就id & 成就解锁状态） 
    /// 这两个字段分别用于检索成就SO，以及读取、写入解锁状态。
    /// </summary>
    public class AchievementSO : ScriptableObject
    {
        [Header("成就ID")]
        public int AchievementID;
        [Header("成就名称")]
        public string AchievementName;
        [Header("成就内容/解锁条件")]
        public TextAsset AchievementContent;
        [Header("成就图标")]
        public Sprite AchievementIcon;
        [Header("是否已解锁")]
        public bool bIsUnlocked;
    }
}
