using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Utilities;
using Unity.VisualScripting;

public class InputTestTool : EditorWindow
{
    // 设置菜单入口
    [MenuItem("Window/Tools/InputTestTool")]
    public static void ShowWindow()
    {
        GetWindow<InputTestTool>("输入测试工具");
    }

    // 绘制测试工具窗口
    void OnGUI()
    {
        GUILayout.Label("当前输入键位一览" , EditorStyles.boldLabel);
        // ==== 显示当前输入键位 ====
        InputManager inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogError("无法获取场景中的 InputManager！");
            return;
        }
        Dictionary<string, string> keyBindings = GetAllKeyBindings(inputManager);
        foreach (var binding in keyBindings)
        {
            GUILayout.Label($"{binding.Key}:");
            if (GUILayout.Button(binding.Value))
            {
                StartRebind(keyBindings , binding.Key);
            }
        }
        // =========================

        // ==== 重设InputManager键位 ====
        if (GUILayout.Button("重设键位至默认值"))
        {
            inputManager.PlayerCharacterCharacter_MoveForward_Key = "/Keyboard/w";
            inputManager.PlayerCharacter_MoveBackward_Key = "/Keyboard/s";
            inputManager.PlayerCharacter_MoveLeft_Key = "/Keyboard/a";
            inputManager.PlayerCharacter_MoveRight_Key = "/Keyboard/d";
            inputManager.PlayerCharacter_Sprint_Key = "/Keyboard/left shift";
            inputManager.PlayerCharacter_Jump_Key = "/Keyboard/space";

            inputManager.PlayerCharacter_Interact_Key = "/Keyboard/e";

            inputManager.PlayerCharacter_Attack_Key = "/Mouse/leftButton";
            inputManager.PlayerCharacter_Aim_Key = "/Mouse/rightButton";
            inputManager.PlayerCharacter_Reload_Key = "/Keyboard/r";
            inputManager.Game_PauseAndContinue_Key = "/Keyboard/escape";

            Debug.Log("已重设键位至默认值");
        }
        // =============================
    }

    #region 获取InputManager中的键位
    public Dictionary<string, string> GetAllKeyBindings(InputManager inputManager)
    {
        var result = new Dictionary<string, string>();
        var fields = typeof(InputManager).GetFields(
            BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(string) && field.Name.EndsWith("_Key"))
            {
                string value = field.GetValue(inputManager) as string;
                result[field.Name] = value;
            }
        }
        return result;
    }
    #endregion

    #region 重设键位
    private EditorApplication.CallbackFunction _pendingDelay;
    void StartRebind(Dictionary<string, string> keyBindings , string bindingName)
    {
        if (_pendingDelay != null)
        {
            EditorApplication.delayCall -= _pendingDelay;
        }
        _pendingDelay = () => Rebind(keyBindings , bindingName);
        EditorApplication.delayCall += _pendingDelay;
    }

    void Rebind(Dictionary<string, string> keyBindings , string bindingName)
    {
        EditorApplication.delayCall -= _pendingDelay;
        var field = typeof(InputManager).GetField(bindingName);
        IDisposable sub = null;
        sub = InputSystem.onAnyButtonPress.Call(control =>
        {
            string newKeyPath = control.path;

            // 检查键位是否有重复占用
            foreach (var binding in keyBindings)
            {
                if (binding.Key == bindingName) continue;
                if (binding.Value == newKeyPath)
                {
                    Debug.LogWarning("更改键位失败：键位已被占用");
                    sub.Dispose();
                    return;
                }
            }

            keyBindings[bindingName] = newKeyPath;
            field.SetValue(InputManager.Instance , newKeyPath);

            sub.Dispose();

            Debug.Log($"成功更改 {bindingName} 的键位至 {newKeyPath}");

            Repaint();
        });
    }
    #endregion
}
