using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.UI
{
    [CreateAssetMenu(fileName = "NewSliderConfigSO" , menuName = "ScriptableObject/UI/Config/SliderConfigSO")]
    public class SliderConfigSO : ScriptableObject
    {
        [Header("最大值")]
        public float _maxValue;
        [Header("最小值")]
        public float _minValue;
        [Header("默认值")]
        public float _defaultValue;
    }
}
