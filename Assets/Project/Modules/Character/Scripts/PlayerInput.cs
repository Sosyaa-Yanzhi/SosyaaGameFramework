using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Character.Player;
using UnityEngine.InputSystem;
using System;

public class PlayerInput : MonoBehaviour
{
    [Header("Input Actions 资源")]
    [SerializeField]public InputActionAsset PlayerInputActions;

    // ==== 玩家输入动作 ====
    private InputAction PlayerMoveAction;
    private InputAction PlayerJumpAction;
    private InputAction PlayerSprintAction;
    private InputAction PlayerInteractAction;
    private InputAction PlayerAttackAction;
    private InputAction PlayerAimAction;
    private InputAction PlayerReloadAction;
    // =====================

    // ==== 玩家对象 ====
    private GameObject PlayerObject;
    // =================

    void Awake()
    {
        // 获取玩家 InputAction
        GetPlayerInputAction(ref PlayerMoveAction , "Player" , "Move");
        GetPlayerInputAction(ref PlayerJumpAction , "Player" , "Jump");
        GetPlayerInputAction(ref PlayerSprintAction , "Player" , "Sprint");
        GetPlayerInputAction(ref PlayerInteractAction , "Player" , "Interact");
        GetPlayerInputAction(ref PlayerAttackAction , "Player" , "Attack");
        GetPlayerInputAction(ref PlayerAimAction , "Player" , "Aim");
        GetPlayerInputAction(ref PlayerReloadAction , "Player" , "Reload");   
    
        // 获取玩家对象
        PlayerObject = GameObject.FindWithTag("Player");
    }
    
    #region 获取玩家 InputAction
    void GetPlayerInputAction(ref InputAction inputAction , string map , string action)
    {
        inputAction = PlayerInputActions.FindAction($"{map}/{action}");
        if (inputAction == null)
        {
            Debug.LogError($"无法找到 {map}/{name} ，请检查相关名字！");
        }
    }
    #endregion

    #region 接入&移除事件
    void AddPerformed(InputAction inputAction , Action<InputAction.CallbackContext> callback)
    {
        inputAction.performed += callback;
        inputAction.Enable();
    }

    void RemovePerformed(InputAction inputAction , Action<InputAction.CallbackContext> callback)
    {
        inputAction.performed -= callback;
        inputAction.Disable();
    }
    #endregion
}
