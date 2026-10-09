using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;



public class InputManager : MonoBehaviour
{
    #region 单例实现
    static private InputManager _instance;
    static public InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputManager>();

                if (_instance == null)
                {
                    GameObject inputManagerObject = new GameObject("InputManager");
                    _instance = inputManagerObject.AddComponent<InputManager>();
                    DontDestroyOnLoad(inputManagerObject);
                }
            }
            return _instance;   
        }
    }
    #endregion

    #region 键位表
    [Header("玩家角色基础操作")]
    public string PlayerCharacterCharacter_MoveForward_Key = "/Keyboard/w";
    public string PlayerCharacter_MoveBackward_Key = "/Keyboard/s";
    public string PlayerCharacter_MoveLeft_Key = "/Keyboard/a";
    public string PlayerCharacter_MoveRight_Key = "/Keyboard/d";
    public string PlayerCharacter_Sprint_Key = "/Keyboard/left shift";
    public string PlayerCharacter_Jump_Key = "/Keyboard/space";

    [Header("玩家角色交互操作")]
    public string PlayerCharacter_Interact_Key = "/Keyboard/e";

    [Header("玩家战斗操作")]
    public string PlayerCharacter_Attack_Key = "/Mouse/leftButton";
    public string PlayerCharacter_Aim_Key = "/Mouse/rightButton";
    public string PlayerCharacter_Reload_Key = "/Keyboard/r";

    [Header("游戏进程操作")]
    public string Game_PauseAndContinue_Key = "/Keyboard/escape";
    #endregion

    #region 读取键位方法
    
    #endregion

    #region 直接设置键位方法
    public void SetInput(InputAction action , int bindingIndex , string newPath)
    {
        action.ApplyBindingOverride(bindingIndex , new InputBinding { overridePath = newPath });
    }
    #endregion
    
    #region 玩家重绑定键位方法
    public void RebindButton(InputAction action , int bindingIndex , Action onDone = null)
    {
        action.Disable(); // 禁用操作

        action.PerformInteractiveRebinding(bindingIndex)
        .WithCancelingThrough("<Keyboard>/escape")
        .OnComplete(op => {
            op.Dispose();
            action.Enable();
            onDone?.Invoke();
        })
        .OnCancel(op =>
        {
            op.Dispose();
            action.Enable();
        })
        .Start();
    }

    public void RebindAxis(InputAction action, string partName, Action onDone = null)
    {
        int idx = GetPartIndex(action, partName);
        if (idx < 0) { Debug.LogError($"找不到 {partName}"); return; }

        action.Disable();
        action.PerformInteractiveRebinding(idx)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("<Mouse>")
            .OnComplete(op =>
            {
                op.Dispose();
                action.Enable();
                onDone?.Invoke();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                action.Enable();
            })
            .Start();
    }

    public int GetPartIndex(InputAction action, string partName)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];
            if (b.isPartOfComposite && b.name.Equals(partName, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }
    #endregion
}
