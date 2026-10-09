using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerActions : MonoBehaviour , IPlayerAction
    {
        public void PlayerMove(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Move");
        }

        public void PlayerJump(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Jump");
        }

        public void PlayerSprint(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Sprint");
        }

        public void PlayerAttack(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Attack");
        }

        public void PlayerAim(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Aim");
        }

        public void PlayerReload(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Reload");
        }

        public void PlayerInteract(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Interact");
        }
    }
}
