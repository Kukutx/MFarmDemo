using UnityEngine.Timeline;
/// <summary>
/// 这个是用来在Timeline那边用来可创建Track的类
/// </summary>

[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{
    
}



/*
DialogueTrack的类，该类继承自TrackAsset。DialogueTrack类用于在Unity的时间线中创建一个自定义的对话轨道。
使用TrackClipType属性来指定其支持的剪辑类型为DialogueClip。这意味着在这个对话轨道上，只能添加DialogueClip类型的剪辑。
在Unity的时间线中，轨道是用于组织和存储剪辑的容器。每个轨道都可以包含多个剪辑，这些剪辑可以是音频剪辑、
动画剪辑、自定义剪辑等。在这个例子中，DialogueTrack类就是一个用于存储和管理对话剪辑的自定义轨道。
*/