using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Profile;
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

    #region 键位配置文件
    [Header("键位表SO")]
    [Tooltip("此SO用于读取本地键位设置")]
    [SerializeField] private GameProfileMap inputSetting_SO;
    [Header("玩家InputActionMap")]
    [Tooltip("实际使用的玩家键位的map")]
    [SerializeField] private InputActionMap playerInputActionMap;
    #endregion

    #region 键位列表
    /// <summary>
    /// 键位列表通过读取键位数据表SO后，将根据此SO创建对应的键位字典，在进入游戏时，
    /// 应将此列表同步映射到InputActionMap中；在更新时若在InputActionMap中进行了键位更新（如添加新操作），
    /// 则应在键位数据表SO中进行相关更新后打包InputActionMap及SO的AA包。
    /// </summary>
    [Header("键位")]
    public Dictionary<string , string> KeyInputs = new Dictionary<string, string>(); 
    #endregion

    #region 读取键位方法
    /// <summary>
    /// 从键位表SO进行读取，需确认键位表SO正确读取对应存档文件。
    /// 建议在游戏开始时读取一次，之后通过玩家重绑定键位方法进行修改。
    /// </summary>
    void GetKeyInputSetting()
    {
        // 读取数据并构建键位输入字典
        foreach (ProfileData profileData in inputSetting_SO.profileDatas.Values)
        {
            if (profileData.ProfileDataType != DataType.String)
            {
                Debug.LogWarning("无法正常读取键位数据表SO：出现不可读取字段数据类型。");
                return;
            }
            KeyInputs.Add(profileData.ProfileDataName , profileData.ProfileDataValue);
        }

        // 同步到InputActionMap中 - 若InputActionMap中有多的Action，使用其中的默认键位
        foreach (InputAction action in playerInputActionMap.actions)
        {
            try
            {
                /// <summary>
                /// 若仅有一个键位输入，直接绑定 BindingIndex = 0；
                /// 若不止一个键位输入，则根据顺序依次进行同步绑定。
                /// </summary>
                
                
            }
            catch
            {
                
            }
        }
    }
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
