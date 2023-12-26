using System.Collections.Generic;
using UnityEngine;
using UnityGame.Utilities.Attribute;

/// <summary>
/// 场景音效数据列表
/// </summary>
[CreateAssetMenu(fileName ="SceneSoundList_SO",menuName ="Sound/SceneSoundList")]
public class SceneSoundList_SO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundList; //  场景音效数据列表
    /// <summary>
    /// 获取场景音效子项
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SceneSoundItem GetSceneSoundItem(string name)
    {
        return sceneSoundList.Find(s => s.sceneName == name);//拉姆达表达式
    }
}

/// <summary>
/// 场景音效子项类
/// </summary>
[System.Serializable]
public class SceneSoundItem
{
    [SceneName] public string sceneName;// 场景
    public SoundName ambient;           // 环境音
    public SoundName music;             // 背景音
}
