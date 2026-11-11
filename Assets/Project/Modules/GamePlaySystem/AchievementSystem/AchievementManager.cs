using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.GameplaySystem.Achievement;
using GameFramework.Profile;
using UnityEngine;

namespace GameFramework.GameplaySystem.AchievementSystem
{
    public class AchievementManager : MonoBehaviour
    {
        /// <summary>
        /// 此成就管理器应始终伴随游戏场景，统一管理各个成就。
        /// 工作流程：游戏应用启动且此管理器被激活后，从本地/线上服务器加载现有的所有成就（包含未解锁项），
        /// 当有类似成就解锁的情况时，其他方需要向此管理器发送请求，管理器再对特定成就SO进行数据更改操作，
        /// 在游戏结束后进行数据持久化。
        /// 
        /// 注意：如有类似隐藏成就的需要（如显示成就名为“???”），应由此管理器对相关UI进行截获，
        /// 实机中对玩家的显示以管理器为最高权限。
        /// </summary>
        
        [Header("成就SO存放相对地址")]
        [SerializeField]private string _sopath;
        [Header("成就列表")]
        [SerializeField]private List<AchievementSO> AchievementList = new List<AchievementSO>();
        [Header("读取的SO")]
        [SerializeField]private GameProfileMap AchievemtProfileMap;

        // 公开事件
        public Action<AchievementSO> OnAchievementUnlocked;

        void OnEnable()
        {
            // 从 Resources 文件夹进行成就SO的读取
            AchievementList = Resources.LoadAll<AchievementSO>(_sopath).ToList();
            AchievementList.Sort((a , b) => a.AchievementID.CompareTo(b.AchievementID)); // 注：应始终保持SO根据id进行有序排列
            // 从相应 GameProfileMap 进行数据读取并应用到SO中
            foreach (var id in AchievemtProfileMap.GetDictionaryFromJsonFile<int , bool>(AchievemtProfileMap , AchievemtProfileMap.profileDatasList[0].value).Keys)
            {
                AchievementList[id].bIsUnlocked = 
                AchievemtProfileMap.GetDictionaryFromJsonFile<int , bool>(AchievemtProfileMap , AchievemtProfileMap.profileDatasList[0].value)[id];
            }
        }

        #region 公共方法
        public void UnlockAchievement(int _achievementID = -1 , string _achievemtName = "no_name")
        {
            if (_achievementID == -1 && _achievemtName == "no_name")
            {
                Debug.LogWarning("成就解锁调用出错：未指定具体成就信息。");
                return;
            }

            // 防止成就重复解锁
            if (AchievementList[_achievementID].bIsUnlocked)
            {
                Debug.LogWarning("成就解锁调用：有尝试重复解锁的成就。");
                return;
            }

            // 解锁成就
            AchievementList[_achievementID].bIsUnlocked = true;
            Debug.Log($"成功解锁新的成就：{AchievementList[_achievementID].AchievementName}");

            OnAchievementUnlocked.Invoke(AchievementList[_achievementID]);
        }
        #endregion

        void OnDisable()
        {
            // 向 ProfileManager 发出进行数据写入文件的请求，实现持久化
            ProfileManager.Instance.WriteProfileToFile(AchievemtProfileMap);
        }
    }
}
