using System.Collections;
using System.Collections.Generic;
using GameFramework.Character;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameFramework.Character.Player
{
    public class PlayerActions : MonoBehaviour , ICharacterAction
    {
        public void Move(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Move");
        }

        public void Jump(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Jump");
        }

        public void Sprint(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Sprint");
        }

        public void Attack(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Attack");
        }

        public void Aim(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Aim");
        }

        public void Reload(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Reload");
        }

        public void Interact(InputAction.CallbackContext ctx)
        {
            Debug.Log("玩家输入反馈-执行：Player Interact");
        }
    }
}
