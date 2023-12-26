using UnityEngine;
using UnityEditor;

/// <summary>
/// (Editor)制作[SceneName]Attribute 特性，这个是42集的东西是专门给编辑器定义的，记住在创建文件的时候后面一定要跟着Attribute
/// </summary>

namespace UnityGame.Utilities.Attribute
{
    /// <summary>
    /// SceneNameAttribute ,这个玩意千万别放在scripts文件夹里一起拿去打包，会报错
    /// </summary>
    public class SceneNameAttribute : PropertyAttribute  // 编辑器的属性类 
    {
    }
}