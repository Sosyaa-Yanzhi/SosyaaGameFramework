using System.Collections;
using System.Collections.Generic;
using GameFramework.Profile;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFramework.Editor
{
    public class GameProfileManagingTool : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset GameProfileManagingToolVisualTreeAsset;

        // 设置菜单入口
        [MenuItem("Window/Tools/GameProfileManagingTool")]
        public static void ShowWindow()
        {
            var MainWindow = GetWindow<GameProfileManagingTool>();
            if (ProfileManager.Instance == null)
            {
                Debug.LogError("GameProfileManagingTool 出错：未能获取到 ProfileManager");
                return;
            }
        }

        void CreateGUI()
        {
            // 加载 UXML，构建视觉树
            if (GameProfileManagingToolVisualTreeAsset != null)
            {
                GameProfileManagingToolVisualTreeAsset.CloneTree(rootVisualElement);
            }
            else
            {
                Debug.LogError("未指定 UXML 资源，请在脚本 Inspector 中赋值");
                return;
            }
        }

        // 读取并显示现有存档数据
        void ShowExistingGameProfileData(string ProfilePath)
        {
            ListView GameProfileListView = rootVisualElement.Q<ListView>("listview-gameprofiledatas");
        }
    }
}
