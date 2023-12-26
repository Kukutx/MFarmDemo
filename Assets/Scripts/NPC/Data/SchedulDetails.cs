using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 日程数据或者每日行程，时间表脚本,根据游戏中的时间变化来操作NPC接下来的每一步行为， NPC每次移动的步骤
/// </summary>
[Serializable]
public class SchedulDetails:IComparable<SchedulDetails>
{
    public int hour, minute, day; // 时，分，天
    public int priority;//优先级,优先级越小越先执行
    public Season season;       // 当前季节
    public string targetScene;      // 目标场景
    public Vector2Int targetGridPosition;       // 目标网格坐标
    public AnimationClip clipAtStop;          // 动画片段，当停止的时候播放什么动画片段
    public bool interactable;             // 是否npc可互动的

    public int Time => (hour * 100) + minute;//时间合并为分钟

    public SchedulDetails(int hour, int minute, int day, int priority, Season season, string targetScene, Vector2Int targetGridPosition, AnimationClip clipAtStop, bool interactable)//构造函数
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.priority = priority;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.clipAtStop = clipAtStop;
        this.interactable = interactable;
    }

    /// <summary>
    /// 时间相比于
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(SchedulDetails other)
    {
        if (Time == other.Time)//如果时间相等则比较优先级
        {
            if (priority > other.priority)
                return 1;
            else
                return -1;
        }
        else if (Time > other.Time)
        {
            return 1;
        }
        else if (Time < other.Time)
        {
            return -1;
        }
        return 0;
    }
}
