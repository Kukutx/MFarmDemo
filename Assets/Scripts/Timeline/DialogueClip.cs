using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// 在Timeline 添加对话片段
/// </summary>
public class DialogueClip : PlayableAsset/*改基类可以完成Playable的实例化*/,ITimelineClipAsset//该接口可以实现Timeline剪辑的高级功能
{
    public ClipCaps clipCaps => ClipCaps.None;  // 返回时间线片段支持的功能
    public DialogueBehaviour dialogue = new DialogueBehaviour();  // 用于存储对话行为

    /// <summary>
    /// 重写CreatePlayable方法，用于创建Playable实例
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dialogue);   // 使用ScriptPlayable<DialogueBehaviour>类创建一个Playable实例，简单的说就是以dialogue这个作为模板在每次新建的时候创建一个可以编辑的片段给playable
        return playable;
    }
}
