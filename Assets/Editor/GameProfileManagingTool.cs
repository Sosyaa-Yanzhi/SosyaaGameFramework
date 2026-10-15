using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework.Profile;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.PackageManager.UI;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFramework.Editor
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

        private bool isAdditionDialogPoped = false;
        private VisualElement dialog;

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

            #region 2. 读取存档文件目录结构并可视化
            // 获取根目录相对路径
            var folderField = rootVisualElement.Q<UnityEditor.UIElements.ObjectField>("objectfield-rootfile");
            if (folderField == null)
            {
                Debug.LogError("找不到 ObjectField: objectfield-rootfile");
                return;
            }
            folderField.objectType = typeof(DefaultAsset);

            // 获取TreeView
            var folderTreeView = rootVisualElement.Q<UnityEngine.UIElements.TreeView>("treeview-filestructure");
            if (folderTreeView == null)
            {
                Debug.LogError("找不到 TreeView: treeview-filestructure");
                return;
            }

            folderTreeView.makeItem = () => new Label();
            folderTreeView.bindItem = (element, index) =>
            {
                var item = folderTreeView.GetItemDataForIndex<FolderNode>(index);
                ((Label)element).text = item.folderName;
            };
            
            folderField.RegisterValueChangedCallback(evt =>
            {
                var asset = evt.newValue as DefaultAsset;
                if (asset == null)
                {
                    folderTreeView.SetRootItems(new List<TreeViewItemData<FolderNode>>());
                    return;
                }

                string rootDirectoryPath = AssetDatabase.GetAssetPath(asset);
                if (!AssetDatabase.IsValidFolder(rootDirectoryPath))
                {
                    Debug.LogError($"所选资源不是有效文件夹: {rootDirectoryPath}");
                    folderTreeView.SetRootItems(new List<TreeViewItemData<FolderNode>>());
                    return;
                }

                string absoluteRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", rootDirectoryPath));
                string[] profileDirs = Directory.GetDirectories(
                    absoluteRootPath,
                    "*",
                    SearchOption.TopDirectoryOnly
                );
                var children = new List<TreeViewItemData<FolderNode>>(profileDirs.Length);
                for (int i = 0; i < profileDirs.Length; i++)
                {
                    string dirName = Path.GetFileName(profileDirs[i]);
                    string relativePath = $"{rootDirectoryPath}/{dirName}";
                    var childNode = new FolderNode(i + 1, dirName, relativePath);
                    children.Add(new TreeViewItemData<FolderNode>(childNode.folderID, childNode));
                }

                var rootNode = new FolderNode(0, asset.name, rootDirectoryPath);
                var rootItem = new TreeViewItemData<FolderNode>(rootNode.folderID, rootNode, children);
                folderTreeView.SetRootItems(new List<TreeViewItemData<FolderNode>> { rootItem });
                folderTreeView.ExpandItem(rootNode.folderID);
            });
            #endregion

            #region 3. 通过按钮添加存档数据表
            var addProfileMapButton = rootVisualElement.Q<Button>("button-addprofilemap");
            if (addProfileMapButton == null)
            {
                Debug.LogError("找不到 Button: button-addprofilemap");
                return;
            }

            addProfileMapButton.clicked += () =>
            {
                // 显示对话弹窗
                ShowDialogWindow();
            };
            #endregion

            #region 4. 通过按钮删除存档数据表
            var deleteProfileMapButton = rootVisualElement.Q<Button>("button-deleteprofilemap");
            if (deleteProfileMapButton == null)
            {
                Debug.LogError("找不到 Button: button-deleteprofilemap");
                return;
            }
            #endregion
        }

        #region 在ListView显示现有存档数据
        public void ShowExistingGameProfileData(string ProfilePath)
        {
            ListView GameProfileListView = rootVisualElement.Q<ListView>("listview-gameprofiledatas");
            
        }
        #endregion
        
        #region 添加存档数据表
        public void AddProfileMap(ProfileType profileType , string mapName , string targetPath)
        {
            AssetDatabase.CreateAsset(new GameProfileMap(profileType , mapName), targetPath);
            Debug.Log($"成功添加数据表：\n数据表种类：{profileType}\n数据表名称：{mapName}\n数据表路径：{targetPath}");
        }
        #endregion

        #region 删除存档数据表
        public void DeleteProfileMap()
        {
            
        }
        #endregion

        #region 显示对话弹窗
        public void ShowDialogWindow()
        {
            if (isAdditionDialogPoped) return;
            isAdditionDialogPoped = true;
            dialog = AddProfileMapDialog.CloneTree();
            rootVisualElement.Add(dialog);

            
            var addComfirmButton = dialog.Q<Button>("button-addconfirm");
            var cancelButton = dialog.Q<Button>("button-cancel");
            var typeEnumField = dialog.Q<EnumField>("enumfield-profiletype");
            var mapNameTextField = dialog.Q<TextField>("textfield-profilemapname");
            var targetPathTextField = dialog.Q<TextField>("textfield-profiletargetpath");
            
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
                
                // 3. 新数据表路径为空————不通过检查
                if (targetPathTextField.value == null)
                {
                    Debug.LogError("无法添加新的存档数据表：路径不能为空！");
                }

                // 通过检查后：
                AddProfileMap((ProfileType)typeEnumField.value , mapNameTextField.value , targetPathTextField.value);

                HideDialogWindow();
            };
            cancelButton.clicked += () =>
            {
                HideDialogWindow();
            };
        }
        #endregion

        #region 关闭对话弹窗
        public void HideDialogWindow()
        {
            dialog.style.display = DisplayStyle.None;
            isAdditionDialogPoped = false;
        }
        #endregion
    }
}
