using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
    // 游戏状态枚举
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        #region 单例实现
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region 游戏状态管理
        public GameState CurrentGameState { get; private set; } = GameState.MainMenu;

        void SetGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    Debug.Log("GameManager:主菜单");
                    break;
                case GameState.Playing:
                    Debug.Log("GameManager:游戏开始");
                    break;
                case GameState.Paused:
                    Debug.Log("GameManager:游戏已暂停");
                    break;
                case GameState.GameOver:
                    Debug.Log("GameManager:游戏结束");
                    break;
            }
        }
        #endregion

        #region 游戏存档管理
        void LoadSettingData(string filePath)
        {
            
        }
        #endregion
    }
}
