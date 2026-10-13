using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor
{
    public class GameProfileManagingTool : EditorWindow
    {
        // 设置菜单入口
        [MenuItem("Window/Tools/GameProfileManagingTool")]
        public static void ShowWindow()
        {
            GameProfileManagingTool MainWindow = GetWindow<GameProfileManagingTool>("存档编辑器");
        }

        // 主窗口绘制
        
    }
}
