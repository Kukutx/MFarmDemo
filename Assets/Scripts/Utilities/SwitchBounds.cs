using Cinemachine;
using UnityEngine;


/// <summary>
/// 切换场景边界
/// 当前主要目标是为追踪摄像头提供,用于在指定的边界内停止追踪。因为场景的边界(Bounds)在场景(Scene)中,
/// 而摄像机(PersistentScena)在另一个对象中,所以无法直接引用挂载到CM vcam1上。因此,
/// 我创建了这个脚本,以便从另一个Scena对象获取Bounds,并挂载到CM vcam1上。
/// </summary>
public class SwitchBounds : MonoBehaviour //控制切换边界时的类，每张地图边界不同，每次切换地图时调用此类，更新一下边界让Cinamachine知道
{
    //TODO: 切换场景后更改调用
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfinerShape;//加载注册事件
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }

    private void SwitchConfinerShape()
    {
        // 获取当前所在地图的场景边界
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();  // 找到并获取组件
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D= confinerShape;          // 给跟随摄像机绑定场景边界

        // Call this if the bounding shape's points change at runtime , 如果边界形状的点在运行时发生变化，则调用此方法
        // 每当切换场景的时候调用此函数清理一下路劲缓存，清除之前的边界信息
        confiner.InvalidatePathCache();             // 调用完了后清除缓存
    }
}
