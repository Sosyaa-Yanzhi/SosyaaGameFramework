using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace GameFramework.UI
{
    public class SettingSlider : MonoBehaviour
    {
        [SerializeField]private SliderConfigSO sliderConfigSO;
        private Slider sliderComponent;

        [Header("当前数值")]
        public float _currentValue;

        void OnEnable()
        {
            if (sliderConfigSO == null)
            {
                Debug.LogError("错误：缺少 SliderConfigSO");
                return;
            }
            if (sliderComponent == null)
            {
                sliderComponent = gameObject.AddComponent<Slider>();
                if (sliderComponent == null)
                {
                    Debug.LogError("错误：缺少 SliderComponent");
                    return;
                }
            }

            InitializeSlider();
        }

        void InitializeSlider()
        {
            sliderComponent.maxValue = sliderConfigSO._maxValue;
            sliderComponent.minValue = sliderConfigSO._minValue;
            sliderComponent.value = _currentValue;
            sliderComponent.onValueChanged.AddListener(ApplyNewValue);
        }

        void ApplyNewValue(float value)
        {
            _currentValue = value;
            sliderComponent.value = _currentValue;
        }

        #region 公开方法
        // 恢复默认值
        public void SetToDefaultSlider()
        {
            sliderComponent.value = sliderConfigSO._defaultValue;
        }
        #endregion
    }
}
