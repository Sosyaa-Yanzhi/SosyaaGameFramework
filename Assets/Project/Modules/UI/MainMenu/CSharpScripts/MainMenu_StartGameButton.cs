using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace GameFramework.UI
{
    public class MainMenu_StartGameButton : MonoBehaviour
    {
        private LuaEnv StartGameButton_LuaEnv = new LuaEnv();
        private delegate void InitFunction(GameObject gameObject);

        [Header("需要加载的Lua脚本路径")]
        public string LuaScriptPath;

        void Awake()
        {
            StartGameButton_LuaEnv.AddLoader((ref string filepath) =>
            {
                if (File.Exists(LuaScriptPath))
                {
                    return File.ReadAllBytes(LuaScriptPath);
                }
                return null;
            });
        }

        void Start()
        {
            // 初始化按钮
            InitFunction Init = StartGameButton_LuaEnv.Global.Get<InitFunction>("Init"); 
            Init(this.gameObject);
        }
    }
}
