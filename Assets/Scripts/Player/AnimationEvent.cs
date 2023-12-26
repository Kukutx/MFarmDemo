using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色和npc移动的音效事件
/// </summary>
public class AnimationEvent : MonoBehaviour
{
    /// <summary>
    /// 走路的声音
    /// </summary>
    public void FootstepSound()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }
}
