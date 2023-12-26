using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 开头动画的时间线管理
/// </summary>
public class TimelineManager : Singleton<TimelineManager>
{
    public PlayableDirector startDirector;   // 用于存储开始的时间线
    private PlayableDirector currentDirector;  // 用于存储当前正在播放的时间线
    private bool isPause;  // 用于存储时间线是否暂停
    private bool isDone;  // 用于存储时间线是否播放完毕
    public bool IsDone { set => isDone = value; }  // 用于设置时间线是否播放完毕,避免就算没有执行完也能按空格键来跳过对话片段

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void Update()
    {
        //  检查时间线是否暂停，是否按下了空格键，以及时间线是否播放完毕
        if (isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            // 如果以上条件都满足，则将isPause设置为false，并将currentDirector的速度设置为1，即正常速度
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if (currentDirector != null)
            currentDirector.Play(); // 开始播放时间线
    }

    /// <summary>
    /// 用于暂停时间线
    /// </summary>
    /// <param name="director"></param>
    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);          // 并将其速度设置为0，即暂停时间线
        isPause = true;  // 时间线已暂停
    }
}
