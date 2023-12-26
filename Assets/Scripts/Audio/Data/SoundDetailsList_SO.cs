using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音效详情数据列表
/// </summary>
[CreateAssetMenu(fileName ="SoundDetailsList_SO",menuName ="Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDetails> soundDetailsList;  // 音效详情数据列表

    /// <summary>
    /// 获取音效详情
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SoundDetails GetSoundDetails(SoundName name)
    {
        return soundDetailsList.Find(s => s.soundName == name);
    }
}

/// <summary>
/// 音效详情类
/// </summary>
[System.Serializable]
public class SoundDetails
{
    public SoundName soundName;     // 音效名称
    public AudioClip soundClip;     // 音效片段
    [Range(0.1f, 1.5f)]
    public float soundPitchMin;     // 声音最小音效
    [Range(0.1f, 1.5f)]
    public float soundPitchMax;     // 声音最大音效
    [Range(0.1f, 1f)]
    public float soundVolume;       // 音效的音量
}
