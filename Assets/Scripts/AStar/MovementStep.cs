using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.AStar
{
    /// <summary>
    /// 移动的每一步类,它有场景名,时间,坐标属性
    /// </summary>
    public class MovementStep
    {
        public string sceneName;    // 场景名
        public int hour;            // 时
        public int minute;          // 分
        public int second;          // 秒
        public Vector2Int gridCoordinate;//坐标
    }
}

