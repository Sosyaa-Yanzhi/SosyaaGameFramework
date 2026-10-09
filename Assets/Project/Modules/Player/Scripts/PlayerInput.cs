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

    void OnEnable()
    {
        PlayerActions playerActions = PlayerObject.GetComponent<PlayerActions>();

        // 将 performed 事件接到 Player 方法
        AddPerformed(PlayerMoveAction , playerActions.PlayerMove);
        AddPerformed(PlayerJumpAction , playerActions.PlayerJump);
        AddPerformed(PlayerSprintAction , playerActions.PlayerSprint);
        AddPerformed(PlayerInteractAction , playerActions.PlayerInteract);
        AddPerformed(PlayerAttackAction , playerActions.PlayerAttack);
        AddPerformed(PlayerAimAction , playerActions.PlayerAim);
        AddPerformed(PlayerReloadAction , playerActions.PlayerReload);

        // 从 InputManager 获取键位表
        InputManager inputManager = InputManager.Instance;
        inputManager.SetInput(PlayerMoveAction , inputManager.GetPartIndex(PlayerMoveAction , "Up") , inputManager.PlayerCharacterCharacter_MoveForward_Key);
        inputManager.SetInput(PlayerMoveAction , inputManager.GetPartIndex(PlayerMoveAction , "Down") , inputManager.PlayerCharacter_MoveBackward_Key);
        inputManager.SetInput(PlayerMoveAction , inputManager.GetPartIndex(PlayerMoveAction , "Left") , inputManager.PlayerCharacter_MoveLeft_Key);
        inputManager.SetInput(PlayerMoveAction , inputManager.GetPartIndex(PlayerMoveAction , "Right") , inputManager.PlayerCharacter_MoveRight_Key);
        inputManager.SetInput(PlayerSprintAction , 0 , inputManager.PlayerCharacter_Sprint_Key);
        inputManager.SetInput(PlayerJumpAction , 0 , inputManager.PlayerCharacter_Jump_Key);
        inputManager.SetInput(PlayerInteractAction , 0 , inputManager.PlayerCharacter_Interact_Key);
        inputManager.SetInput(PlayerAttackAction , 0 , inputManager.PlayerCharacter_Attack_Key);
        inputManager.SetInput(PlayerAimAction , 0 , inputManager.PlayerCharacter_Aim_Key);
        inputManager.SetInput(PlayerReloadAction , 0 , inputManager.PlayerCharacter_Reload_Key);
    }

    void OnDisable()
    {
        PlayerActions playerActions = PlayerObject.GetComponent<PlayerActions>();
        // 将 performed 事件移除自 Player 方法
        RemovePerformed(PlayerMoveAction , playerActions.PlayerMove);
        RemovePerformed(PlayerJumpAction , playerActions.PlayerJump);
        RemovePerformed(PlayerSprintAction , playerActions.PlayerSprint);
        RemovePerformed(PlayerInteractAction , playerActions.PlayerInteract);
        RemovePerformed(PlayerAttackAction , playerActions.PlayerAttack);
        RemovePerformed(PlayerAimAction , playerActions.PlayerAim);
        RemovePerformed(PlayerReloadAction , playerActions.PlayerReload);
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
