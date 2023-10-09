using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制  我们为SceneNameAttribute绘制，使用这个意味着会重写Unity的属性方面的UI界面，
/// 我们目前主要针对的是选择跳转场景的名字那个方面
/// 用这个脚本可以自定义Unity的UI比如让选择场景 TransitionManager 的 Start Scena Name 那块有下拉菜单
/// </summary>

namespace UnityGame.Utilities.Attribute
{
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        private readonly string[] PATH_SPLIT_KEYWORD = new string[] { "/", ".unity" };  // 用来切割路径
        private int sceneIndex = -1;
        private GUIContent[] sceneNames;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorBuildSettings.scenes.Length == 0) return;     // 判断有没有场景如果没有就不执行代码
            if (sceneIndex == -1)
            {
                GetAllSceneNames(property);
            }
            int oldIndex = sceneIndex;
            sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);      // 弹出窗口
            if (oldIndex != sceneIndex)
                property.stringValue = sceneNames[sceneIndex].text;
        }

        /// <summary>
        /// 获得所有场景的名字
        /// </summary>
        /// <param name="property"></param>
        private void GetAllSceneNames(SerializedProperty property)
        {
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length == 0)
            {
                sceneNames = new GUIContent[] { new GUIContent("Check your build settings") };
            }
            else
            {
                // 初始化数组
                sceneNames = new GUIContent[scenes.Length];
                for (int i = 0; i < scenes.Length; i++)
                {
                    string[] pathSplit = scenes[i].path.Split(PATH_SPLIT_KEYWORD, System.StringSplitOptions.RemoveEmptyEntries);   // 获得路径并切割
                    if (pathSplit.Length > 0)
                    {
                        var tempName = pathSplit[pathSplit.Length - 1];
                        sceneNames[i] = new GUIContent(tempName);
                    }
                    else
                    {
                        sceneNames[i] = new GUIContent("(Deleted Scene)");
                    }

                }
            }
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                for (int i = 0; i < sceneNames.Length; i++)
                {
                    if (sceneNames[i].text.Equals(property.stringValue))
                    {
                        sceneIndex = i;
                        break;
                    }
                    sceneIndex = 0;
                }
            }
            else
            {
                sceneIndex = 0;
            }
            property.stringValue = sceneNames[sceneIndex].text;
        }
    }
}