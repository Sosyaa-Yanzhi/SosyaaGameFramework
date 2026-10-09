using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputKey
{
    public class InputKey : MonoBehaviour
    {
        // 按键
        Key KeyInput;
        // 按键绑定事件
        List<Action> KeyActions;
    }
}
