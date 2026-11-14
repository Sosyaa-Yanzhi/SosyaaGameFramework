
using System.Collections.Generic;
using GameFramework.Profile;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFramework.Editor.GameProfile
{
    class FolderNode
    {
        public int folderID;
        public string folderName;
        public string folderPath;
        public FolderNode(int id , string name , string path)
        {
            folderID = id;
            folderName = name;
            folderPath = path;
        }
    }

    public class GameProfileManagingTool : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset GameProfileManagingToolVisualTreeAsset;
        [SerializeField]
        private VisualTreeAsset AddProfileMapDialog;
        [SerializeField]
        private VisualTreeAsset DeleteProifleMapDialog;

        private bool isAdditionDialogPoped = false;
        private bool isDeleteDialogPoped = false;
        private List<string> existingProfileMapNames = new List<string>();
        private VisualElement _addProfileDialog;
        private VisualElement _deleteProfileDialog;

        // 设置菜单入口
        [MenuItem("Window/Tools/GameProfileManagingTool")]
        public static void ShowWindow()
        {
            var MainWindow = GetWindow<GameProfileManagingTool>();
            if (ProfileManager.Instance == null)
            {
                Debug.LogError("GameProfileManagingTool 出错：未能获取到 ProfileManager");
                return;
            }
        }

        void CreateGUI()
        {
            #region 1. 加载 UXML，构建视觉树
            if (GameProfileManagingToolVisualTreeAsset != null)
            {
                GameProfileManagingToolVisualTreeAsset.CloneTree(rootVisualElement);
            }
            else
            {
                Debug.LogError("未指定 UXML 资源，请在脚本 Inspector 中赋值");
                return;
            }
            #endregion

            #region 2. 通过按钮添加存档数据表
            var addProfileMapButton = rootVisualElement.Q<Button>("button-addprofilemap");
            if (addProfileMapButton == null)
            {
                Debug.LogError("找不到 Button: button-addprofilemap");
                return;
            }

            addProfileMapButton.clicked += () =>
            {
                // 显示对话弹窗
                ShowAddProfileDialogWindow();
            };
            #endregion

            #region 3. 通过按钮删除存档数据表
            var deleteProfileMapButton = rootVisualElement.Q<Button>("button-deleteprofilemap");
            if (deleteProfileMapButton == null)
            {
                Debug.LogError("找不到 Button: button-deleteprofilemap");
                return;
            }

            deleteProfileMapButton.clicked += () =>
            {
                // 显示对话弹窗
                ShowDeleteProfileDialogWindow();
            };
            #endregion
        }
        
        #region 添加存档数据表
        void AddProfileMap(ProfileType profileType , string mapName , string targetPath)
        {
            targetPath += $"/{mapName}.asset";
            AssetDatabase.CreateAsset(new GameProfileMap(profileType , mapName), targetPath);
            Debug.Log($"成功添加数据表：\n数据表种类：{profileType}\n数据表名称：{mapName}\n数据表路径：{targetPath}");
        }
        #endregion

        #region 删除存档数据表
        void DeleteProfileMap(string targetPath)
        {
            AssetDatabase.DeleteAsset(targetPath);
            existingProfileMapNames.RemoveAll(name => name == targetPath);
        }
        #endregion

        #region 添加存档对话弹窗
        void ShowAddProfileDialogWindow()
        {
            if (isAdditionDialogPoped) return;
            isAdditionDialogPoped = true;
            _addProfileDialog = AddProfileMapDialog.CloneTree();
            rootVisualElement.Add(_addProfileDialog);

            
            var addComfirmButton = _addProfileDialog.Q<Button>("button-addconfirm");
            var cancelButton = _addProfileDialog.Q<Button>("button-cancel");
            var typeEnumField = _addProfileDialog.Q<EnumField>("enumfield-profiletype");
            var mapNameTextField = _addProfileDialog.Q<TextField>("textfield-profilemapname");
            var targetPathTextField = _addProfileDialog.Q<TextField>("textfield-profiletargetpath");
            
            typeEnumField.Init(ProfileType.Setting);
            
            addComfirmButton.clicked += () =>
            {
                // 1. 新数据表名字为空————不通过检查
                if (mapNameTextField.value == null)
                {
                    Debug.LogError("无法添加新的存档数据表：名字不能为空！");
                    return;    
                }

                // 2. 新数据表名字已存在————不通过检查
                if (existingProfileMapNames.Contains(mapNameTextField.value))
                {
                    Debug.LogError($"无法添加新的存档数据表：名字 {mapNameTextField.value} 已存在！");
                    return;
                }
                
                // 3. 新数据表路径为空————不通过检查
                if (targetPathTextField.value == null)
                {
                    Debug.LogError("无法添加新的存档数据表：路径不能为空！");
                    return;
                }

                // 通过检查后：
                AddProfileMap((ProfileType)typeEnumField.value , mapNameTextField.value , targetPathTextField.value);
                existingProfileMapNames.Add(mapNameTextField.value);

                _addProfileDialog.style.display = DisplayStyle.None;
                isAdditionDialogPoped = false;
            };
            cancelButton.clicked += () =>
            {
                _addProfileDialog.style.display = DisplayStyle.None;
                isAdditionDialogPoped = false;
            };
        }
        #endregion
    
        #region 删除存档对话弹窗
        void ShowDeleteProfileDialogWindow()
        {
            if (isDeleteDialogPoped) return;
            isDeleteDialogPoped = true;
            _deleteProfileDialog = DeleteProifleMapDialog.CloneTree();
            rootVisualElement.Add(_deleteProfileDialog);

            var deleteConfirmButton = _deleteProfileDialog.Q<Button>("button-deleteconfirm");
            var cancelButton = _deleteProfileDialog.Q<Button>("button-cancel");
            var objectField = _deleteProfileDialog.Q<UnityEditor.UIElements.ObjectField>("objectfield-mapso");

            objectField.objectType = typeof(ScriptableObject);
            objectField.allowSceneObjects = false;

            // 删除确认按钮回调
            deleteConfirmButton.clicked += () =>
            {
                // 获取 ObjectField 中拖入SO的路径
                string _SOPath = AssetDatabase.GetAssetPath(objectField.value);
                string _SOName = (objectField.value as GameProfileMap).name;

                // 执行实际删除操作
                if (_SOPath != null)
                {
                    DeleteProfileMap(_SOPath);
                    Debug.Log($"位于 {_SOPath} 的存档数据表 {_SOName} 已删除！");
                }

                _deleteProfileDialog.style.display = DisplayStyle.None;
                isDeleteDialogPoped = false;
            };

            // 取消按钮回调
            cancelButton.clicked += () =>
            {
                _deleteProfileDialog.style.display = DisplayStyle.None;
                isDeleteDialogPoped = false;
            };
        }
        #endregion
    }
}
