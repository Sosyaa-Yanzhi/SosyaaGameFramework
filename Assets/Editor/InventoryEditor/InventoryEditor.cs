using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.InputSystem;
using GameFramework.GameplaySystem.InventorySystem;

namespace GameFramework.Editor.Inventory
{
    public class InventorySlotButton
    {
        public Button _slotElement;
        public Vector2 _slotRelativePosition;
        public bool isSlotSelected;

        public InventorySlotButton(Button newButton , Vector2 newPosition , bool newBoolean = false)
        {
            _slotElement = newButton;
            _slotRelativePosition = newPosition;
            isSlotSelected = newBoolean;
        }
    }

    public class InventoryEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset editorVisualTreeAsset;
        private List<InventorySlotButton> slotButtons = new List<InventorySlotButton>();
        private List<InventorySlotButton> selectedSlotButtons = new List<InventorySlotButton>();
        private int height , width;
        private Vector2 pivotSlotPosition = new Vector2(-1 , -1);
        private Vector2 endSlotPosition = new Vector2(-1 , -1);

        [MenuItem("Window/Tools/InventoryEditor")]
        public static void ShowWindow()
        {
            var MainWindow = GetWindow<InventoryEditor>();
        }

        void CreateGUI()
        {
            #region 加载 UXML，构建视觉树
            if (editorVisualTreeAsset != null)
            {
                editorVisualTreeAsset.CloneTree(rootVisualElement);
            }
            else
            {
                Debug.LogError("未指定 UXML 资源，请在脚本 Inspector 中赋值");
                return;
            }
            #endregion

            #region “应用规格”按钮
            var _currentScaleLabel = rootVisualElement.Q<Label>("label-currentselected");
            var _applySetButton = rootVisualElement.Q<Button>("button-applyset");
            var _settingInputField = rootVisualElement.Q<TextField>("textfield-setscale");
            var _slotVisualElement = rootVisualElement.Q<VisualElement>("visualelement-inventoryslots");
            var _buttonScrollView = rootVisualElement.Q<ScrollView>("scrollview-buttonview");

            _applySetButton.clicked += () =>
            {
                if (TryParseNumberPair(_settingInputField.value , out width , out height))
                {
                    // 在创建UI上网格前先进行清空
                    foreach (var _childVisualElement in _buttonScrollView.contentContainer.Children().ToList())
                    {
                        if (_childVisualElement is Label) continue;
                        _childVisualElement.RemoveFromHierarchy();
                    }
                    slotButtons.Clear();

                    for (int i = 0 ; i < height ; i++)
                    {
                        // 创建弹性盒
                        VisualElement newVisualElement = new VisualElement();
                        _buttonScrollView.contentContainer.Add(newVisualElement);
                        newVisualElement.style.flexDirection = FlexDirection.Row;
                        newVisualElement.style.maxHeight = 40;
                        newVisualElement.style.minHeight = 40;

                        for (int j = 0 ; j < width ; j++)
                        {
                            // 创建按钮
                            Button newButton = new Button();
                            newVisualElement.Add(newButton);
                            newButton.text = "";
                            newButton.style.backgroundColor = new StyleColor(new Color32(188 , 188 , 188 , 255));
                            newButton.style.maxHeight = 36;
                            newButton.style.minHeight = 36;
                            newButton.style.maxWidth = 36;
                            newButton.style.minWidth = 36;

                            InventorySlotButton newInventorySlotButton = new InventorySlotButton(
                                newButton,
                                new Vector2(j , i),
                                false
                            );

                            newButton.userData = newInventorySlotButton;

                            // 每个按钮注册回调
                            newButton.RegisterCallback<PointerDownEvent>(evt =>
                            {
                                var button = evt.target as Button;

                                if (button != null && button?.userData is InventorySlotButton slotButton && slotButton.isSlotSelected)
                                {
                                    if (evt.button == 1)
                                    {
                                        Debug.Log($"取消选中格：{slotButton._slotRelativePosition}");
                                        ChangeSlotButtonState(slotButton , false);
                                    }
                                }
                            });
                            newButton.clicked += () =>
                            {   
                                if (newButton?.userData is InventorySlotButton slotButton && !slotButton.isSlotSelected)
                                {
                                    if (pivotSlotPosition != new Vector2(-1 , -1))
                                    {
                                        Debug.Log($"选中锚点格：{slotButton._slotRelativePosition}");
                                        ChangeSlotButtonState(slotButton , true);
                                    }
                                }
                            };
                            newButton.RegisterCallback<PointerEnterEvent>(evt =>
                            {
                                if (Mouse.current != null && Mouse.current.leftButton.isPressed)
                                {
                                    if (newButton.userData is InventorySlotButton slotButton)
                                    {
                                        endSlotPosition = slotButton._slotRelativePosition;
                                        if (!slotButton.isSlotSelected)
                                        {
                                            if (pivotSlotPosition != new Vector2(-1 , -1))
                                            {
                                                foreach (var btn in slotButtons)
                                                {
                                                    if (btn._slotRelativePosition.x >= pivotSlotPosition.x &&
                                                        btn._slotRelativePosition.x <= endSlotPosition.x &&
                                                        btn._slotRelativePosition.y >= pivotSlotPosition.y &&
                                                        btn._slotRelativePosition.y <= endSlotPosition.y)
                                                    {
                                                        ChangeSlotButtonState(btn , true);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (pivotSlotPosition != new Vector2(-1 , -1))
                                            {
                                                foreach (var btn in slotButtons)
                                                {
                                                    ChangeSlotButtonState(btn , false);
                                                    if (btn._slotRelativePosition.x >= pivotSlotPosition.x &&
                                                        btn._slotRelativePosition.x <= endSlotPosition.x &&
                                                        btn._slotRelativePosition.y >= pivotSlotPosition.y &&
                                                        btn._slotRelativePosition.y <= endSlotPosition.y)
                                                    {
                                                        ChangeSlotButtonState(btn , true);
                                                    }
                                                }
                                            }
                                        }
                                        _currentScaleLabel.text = $"当前选中：{slotButton._slotRelativePosition.x + 1}x{slotButton._slotRelativePosition.y + 1}";
                                    }
                                }
                            });

                            slotButtons.Add(newInventorySlotButton);
                        }
                    }
                }
            };
            #endregion

            #region “创建容器网格”按钮
            var _createButton = rootVisualElement.Q<Button>("button-create");
            var _heightTextField = rootVisualElement.Q<TextField>("textfield-height");
            var _widthTextField = rootVisualElement.Q<TextField>("textfield-width");
            var _inventoryCompObjectField = rootVisualElement.Q<ObjectField>("objectfield-inventorycomp");
            var _slotNameTextField = rootVisualElement.Q<TextField>("textfield-slotname");
            var _parentObjectField = rootVisualElement.Q<ObjectField>("objectfield-parentobject");

            _parentObjectField.objectType = typeof(GameObject);
            _inventoryCompObjectField.objectType = typeof(GameObject);
            
            _createButton.clicked += () =>
            {
                string slotName = _slotNameTextField.value;
                var slotObject = _inventoryCompObjectField.value;
                var parentobject = _parentObjectField.value as GameObject;

                bool isAlphanumericOrUnderscore = !string.IsNullOrEmpty(slotName) 
                && slotName.All(c => char.IsLetterOrDigit(c) || c == '_');

                if (!isAlphanumericOrUnderscore)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "不合法的网格命名！",
                        "确认",
                        "取消"
                    );
                    return;
                }

                if (slotObject == null)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "网格组件不能为空！",
                        "确认",
                        "取消"
                    );
                    return;
                }

                var uniqueSelectedSlotButtons = selectedSlotButtons.Distinct().ToList();
                if (uniqueSelectedSlotButtons.Count <= 0)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "没有选中的网格！",
                        "确认",
                        "取消"
                    );
                    return;
                }

                if (parentobject == null)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "未指定父级物体！",
                        "确认",
                        "取消"
                    );
                    return;
                }

                if (!float.TryParse(_widthTextField.value, out float slotWidth) ||
                    !float.TryParse(_heightTextField.value, out float slotHeight) ||
                    float.IsNaN(slotWidth) || float.IsInfinity(slotWidth) || slotWidth <= 0f ||
                    float.IsNaN(slotHeight) || float.IsInfinity(slotHeight) || slotHeight <= 0f)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "网格宽度和高度必须是大于 0 的数字！",
                        "确认"
                    );
                    return;
                }

                // 开始创建网格
                /// <summary>
                /// 生成网格时，会选取在编辑器中所选择的网格中x & y坐标最接近远点者作为锚点网格，
                /// 锚点网格在实际容器网格阵中作为原点存在，坐标为(0 , 0)。
                /// 注意：此工具主要服务于UI层面，故默认使用 RectTransform 在场景中生成。
                /// </summary>
                float pivotX = uniqueSelectedSlotButtons.Min(x => x._slotRelativePosition.x) , pivotY = uniqueSelectedSlotButtons.Min(y => y._slotRelativePosition.y);

                foreach (var selectedSlotButton in uniqueSelectedSlotButtons)
                {
                    Vector2 slotPosition = new Vector2(
                        (selectedSlotButton._slotRelativePosition.x - pivotX) * slotWidth,
                        (pivotY - selectedSlotButton._slotRelativePosition.y) * slotHeight
                    );

                    var newSlotObject = Instantiate(slotObject as GameObject , (slotObject as GameObject).transform.position ,
                    (slotObject as GameObject).transform.rotation , parentobject.transform);
                    var slotRT = newSlotObject.GetComponent<RectTransform>();

                    slotRT.anchorMax = new Vector2(0.5f , 0.5f);
                    slotRT.anchorMin = new Vector2(0.5f , 0.5f);
                    slotRT.pivot = new Vector2(0.5f , 0.5f);

                    slotRT.anchoredPosition = slotPosition;

                    var slotComp = newSlotObject.GetComponent<InventorySlot>();
                    if (slotComp != null)
                    {
                        slotComp.slotPosition = selectedSlotButton._slotRelativePosition;
                        slotComp.bIsSlotMatched = false;
                    }
                }
            };
            #endregion

            #region “清空选中”按钮
            var _clearButton = rootVisualElement.Q<Button>("button-clear");

            _clearButton.clicked += () =>
            {
                if (slotButtons.Count > 0)
                {
                    foreach (var slotButton in slotButtons)
                    {
                        ChangeSlotButtonState(slotButton , false);
                    }
                    _currentScaleLabel.text = "当前选中：0x0";
                    pivotSlotPosition = new Vector2(-1 , -1);
                    endSlotPosition = new Vector2(-1 , -1);
                }
            };
            #endregion
        
            #region 网格拖拽交互
            // 1. 按下：记录起点（在父容器上监听，覆盖所有按钮）
            _slotVisualElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0) return;

                var element = evt.target as VisualElement;
                var btn = element as Button ?? element?.GetFirstAncestorOfType<Button>();
                if (btn?.userData is InventorySlotButton slotButton)
                {
                    pivotSlotPosition = slotButton._slotRelativePosition;
                    Debug.Log($"起点：{pivotSlotPosition}");
                }
            }, TrickleDown.TrickleDown);

            // 2. 松开：记录终点
            _slotVisualElement.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (evt.button != 0) return;

                var element = evt.target as VisualElement;
                var btn = element as Button ?? element?.GetFirstAncestorOfType<Button>();
                if (btn?.userData is InventorySlotButton slotButton)
                {
                    endSlotPosition = slotButton._slotRelativePosition;
                    Debug.Log($"终点：{endSlotPosition}");
                }
            }, TrickleDown.TrickleDown);
            
            #endregion
        }

        #region 网格状态变更
        void ChangeSlotButtonState(InventorySlotButton inventorySlotButton , bool newState)
        {
            selectedSlotButtons.RemoveAll(slotButton => slotButton == inventorySlotButton);
            inventorySlotButton.isSlotSelected = newState;
            if (!newState)
            {
                inventorySlotButton._slotElement.style.backgroundColor = new StyleColor(new Color32(188 , 188 , 188 , 255));
            }
            else
            {
                selectedSlotButtons.Add(inventorySlotButton);
                inventorySlotButton._slotElement.style.backgroundColor = new StyleColor(new Color32(4 , 160 , 0 , 255));
            }
        }
        #endregion

        #region 输入格式检查
        public bool TryParseNumberPair(string input, out int a, out int b)
        {
            a = 0;
            b = 0;

            if (string.IsNullOrEmpty(input))
            {
                EditorUtility.DisplayDialog(
                    "👿错误",
                    "输入的规格不能为空！",
                    "确认",
                    "取消"
                );
                return false;
            }

            // 按 'x' 分割，最多切成 2 段
            string[] parts = input.Split('x');
            if (parts.Length != 2)
            {
                EditorUtility.DisplayDialog(
                    "👿错误",
                    "输入格式错误！",
                    "确认",
                    "取消"
                );
                return false;
            }

            // 两段都必须是合法整数
            if (!int.TryParse(parts[0].Trim(), out a) && a > 0)
            {
                EditorUtility.DisplayDialog(
                    "👿错误",
                    "输入有不合法整数！",
                    "确认",
                    "取消"
                );
                return false;
            }
            if (!int.TryParse(parts[1].Trim(), out b) && b > 0)
            {
                EditorUtility.DisplayDialog(
                    "👿错误",
                    "输入有不合法整数！",
                    "确认",
                    "取消"
                );
                return false;
            }

            return true;
        }
        #endregion
    }
}
