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
        SetInputFromSettings(inputManager, PlayerMoveAction, "Up", "MoveForwardKey");
        SetInputFromSettings(inputManager, PlayerMoveAction, "Down", "MoveBackwardKey");
        SetInputFromSettings(inputManager, PlayerMoveAction, "Left", "MoveLeftKey");
        SetInputFromSettings(inputManager, PlayerMoveAction, "Right", "MoveRightKey");
        SetInputFromSettings(inputManager, PlayerSprintAction, 0, "SprintKey");
        SetInputFromSettings(inputManager, PlayerJumpAction, 0, "JumpKey");
        SetInputFromSettings(inputManager, PlayerInteractAction, 0, "InteractKey");
        SetInputFromSettings(inputManager, PlayerAttackAction, 0, "AttackKey");
        SetInputFromSettings(inputManager, PlayerAimAction, 0, "AimKey");
        SetInputFromSettings(inputManager, PlayerReloadAction, 0, "ReloadKey");
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

    void SetInputFromSettings(InputManager inputManager, InputAction action, string compositePart, string settingName)
    {
        int bindingIndex = inputManager.GetPartIndex(action, compositePart);
        SetInputFromSettings(inputManager, action, bindingIndex, settingName);
    }

    void SetInputFromSettings(InputManager inputManager, InputAction action, int bindingIndex, string settingName)
    {
        if (bindingIndex >= 0 &&
            inputManager.KeyInputs.TryGetValue(settingName, out string inputPath) &&
            !string.IsNullOrEmpty(inputPath))
        {
            inputManager.SetInput(action, bindingIndex, inputPath);
        }
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
