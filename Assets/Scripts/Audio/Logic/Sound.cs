using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏的各种音效
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;   // 音源

    /// <summary>
    /// 设置音效
    /// </summary>
    /// <param name="soundDetails"></param>
    public void SetSound(SoundDetails soundDetails)
    {
        audioSource.clip = soundDetails.soundClip;
        audioSource.volume = soundDetails.soundVolume;
        audioSource.pitch = Random.Range(soundDetails.soundPitchMin, soundDetails.soundPitchMax);
    }
}
