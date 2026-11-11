using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
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
        GetPlayerInputAction(ref PlayerMoveAction , "Player" , "PlayerMove");
        GetPlayerInputAction(ref PlayerJumpAction , "Player" , "PlayerJump");
        GetPlayerInputAction(ref PlayerSprintAction , "Player" , "PlayerSprint");
        GetPlayerInputAction(ref PlayerInteractAction , "Player" , "PlayerInteract");
        GetPlayerInputAction(ref PlayerAttackAction , "Player" , "PlayerAttack");
        GetPlayerInputAction(ref PlayerAimAction , "Player" , "PlayerAim");
        GetPlayerInputAction(ref PlayerReloadAction , "Player" , "PlayerReload");   
    
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
