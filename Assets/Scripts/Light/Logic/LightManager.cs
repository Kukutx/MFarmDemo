using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 灯光光影管理
/// </summary>
public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights; // 场景的灯光 （每当切换场景的时候，遍历和切换对应场景的灯光）
    private LightShift currentLightShift;   // 当前的灯光模式 （白天或黑夜）
    private Season currentSeaon;            // 当前季节
    private float timeDifference = Settings.lightChangeDuration;        // 灯光强度的时间差。比如下午到晚上的时候他们的灯光强度变化
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;//提示:委托函数事件是一个装函数方法的变量,但是并不是每个委托只能装一个函数变量,委托可以装很多不同的函数然后让它们同时执行!
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int index)
    {
        currentLightShift = LightShift.Morning;
    }

    /// <summary>
    /// 灯光模式更改事件
    /// </summary>
    /// <param name="season"></param>
    /// <param name="lightShift"></param>
    /// <param name="timeDifference"></param>
    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeaon = season;
        this.timeDifference = timeDifference;
        if (currentLightShift != lightShift)
        {
            currentLightShift = lightShift;
            foreach (LightControl light in sceneLights)
            {
                //lightcontrol 改变灯光的方法
                light.ChangeLightShift(currentSeaon, currentLightShift, timeDifference);
            }
        }
    }

    /// <summary>
    /// 加载场景后事件
    /// </summary>
    private void OnAfterSceneLoadedEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLights)     // 每当切换场景的时候，遍历和切换对应场景的灯光
        {
            //lightControl 改变灯光的方法
            light.ChangeLightShift(currentSeaon, currentLightShift, timeDifference);
        }
    }
}
