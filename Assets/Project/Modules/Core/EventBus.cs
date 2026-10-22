using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameFramework.Core
{
    public class EventBus : MonoBehaviour
    {
        #region 单例实现
        static private EventBus _instance;
        static public EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<EventBus>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("EventBus");
                        _instance = go.AddComponent<EventBus>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        } 
        #endregion

        private static readonly Dictionary<Type , Delegate> events
        = new Dictionary<Type, Delegate>();

        #region 订阅
        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (events.TryGetValue(type , out var d))
            {
                events[type] = Delegate.Combine(d , callback);
            }
            else
            {
                events[type] = callback;
            }
        }
        #endregion

        #region 取消订阅
        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (events.TryGetValue(type , out var d))
            {
                var newD = Delegate.Remove(d , callback);
                if (newD == null)
                {
                    events.Remove(type);
                }
                else
                {
                    events[type] = newD;
                }
            }
        }
        #endregion

        #region 发布
        public static void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (events.TryGetValue(type , out var d))
            {
                (d as Action<T>)?.Invoke(evt);
            }
        }
        #endregion

        #region 清空（切换场景时防止内存泄漏）
        public static void Clear() => events.Clear();
        #endregion
    }
}
