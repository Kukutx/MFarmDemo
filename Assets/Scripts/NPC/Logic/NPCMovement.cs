using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.AStar;
using MFarm.Save;

/// <summary>
/// NPC 移动
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour,ISaveable
{
    public SchedulDataList_SO schedulData;          // npc 接下来的移动步骤。日程数据
    private SortedSet<SchedulDetails> scheduleSet;//始终保持其中内容的排序顺序以及唯一性的集合
    private SchedulDetails currentSchedule;         // 当前日程步骤
    //临时存储信息
    [SerializeField]private string currentScene;  // 当前NPC所处在的场景
    private string targetScene;         // 给npc指定的场景
    private Vector3Int currentGridPostion;//当前网格位置
    private Vector3Int targetGridPostion;//目标网格位置
    private Vector3Int nextGridPosition;//下一步网格位置坐标
    private Vector3 nextWorldPosition;//下一步的世界坐标
    public string StartScene { set => currentScene = value; }//NPC在一开始所在的场景
    [Header("移动属性")]
    public float normalSpeed = 2f;   // 正常速度
    private float minSpeed = 1;      // 最小速度
    private float maxSpeed = 3;      // 最大速度
    private Vector2 dir;                //方向
    public bool isMoving;           // 是否移动
    //Component
    private Rigidbody2D rb;         // 刚体
    private SpriteRenderer spriteRenderer;      // npc渲染
    private BoxCollider2D coll;     // 碰撞体
    private Animator anim;              // 动画
    private Grid grid;                      // 网格
    private Stack<MovementStep> movementSteps;  // 人物移动步骤
    private Coroutine npcMoveRoutine;//NPC移动的一个协程将它作为变量操作
    private bool isInitialised;//判断NPC是否是第一次加载
    private bool npcMove;//判断NPC是否移动
    private bool sceneLoaded;//判断场景是否加载完毕
    public bool interactable;// 是否npc可以互动 (如：对话)
    public bool isFirstLoad;    // 是否是第一次加载
    private Season currentSeason;       // 当前季节
    private float animationBreakTime;//动画计时器
    private bool canPlayStopAnimation;  // 是否可以停止播放动画
    private AnimationClip stopAnimationClip;  // 当npc执行某件事的时候停止时候应该播放什么动画片段
    public AnimationClip blankAnimationClip;//定义一个空白的动画片段
    private AnimatorOverrideController animOverride;//重构一个动画控制器
    private TimeSpan GameTime => TimeManager.Instance.GameTime;  //TimeSpan:表示一个时间戳

    public string GUID => GetComponent<DataGUID>().guid;   // 生成唯一标识码

    //作用:例如,定义一个时间戳 TimeSpan targetTime = new TimeSpan(10,20); 这个时间戳就是10分20秒
    //拥有时间戳之后可以利用时间戳作为条件,比如GameTime游戏时间到达10分20秒时能够触发什么事件

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps = new Stack<MovementStep>();
        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride;
        //runtimeAnimatorController:AnimatorController的运行时表示.使用此表示可在运行时期间更改AnimatorController。
        scheduleSet = new SortedSet<SchedulDetails>();
        foreach (var schedule in schedulData.schedulList)
        {
            scheduleSet.Add(schedule);
        }
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        isInitialised = false;
        isFirstLoad = true;
    }

    private void OnEndGameEvent()
    {
        sceneLoaded= false;
        npcMove = false;
        if (npcMoveRoutine != null)
        {
            StopCoroutine(npcMoveRoutine);
        }
    }

    private void Update()
    {
        if (sceneLoaded)
            SwitchAnimation();
        //计时器
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimation = animationBreakTime <= 0;
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)//如果场景加载了才可以让NPC移动
        {
            Movement();
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        grid = FindObjectOfType<Grid>();
        CheckVisiable();
        if (!isInitialised)         // 保证只有第一次初始化npc在网格中间
        {
            InitNPC();//第一次加载的话就初始化NPC
            isInitialised = true;
        }
        sceneLoaded = true;
        if (!isFirstLoad)  // 如果是第一次加载的话那就不用创建schedule
        {
            currentGridPostion = grid.WorldToCell(transform.position);
            // 创建一个新schedule来继续进行之前加载的存档的日程
            var schedule = new SchedulDetails(0, 0, 0, 0, currentSeason, targetScene, (Vector2Int)targetGridPostion, stopAnimationClip, interactable);
            BuildPath(schedule);
            isFirstLoad = true;
        }
    }

    private void OnGameMinuteEvent(int minute, int hour, Season season, int day)
    {
        int time = (hour * 100) + minute;
        currentSeason = season;
        SchedulDetails matchSchedule = null;
        foreach (var schedule in scheduleSet)
        {
            if (schedule.Time == time)
            {
                if (schedule.day != day && schedule.day != 0)
                    continue;
                if (schedule.season != season)
                    continue;
                matchSchedule = schedule;
            }
            else if (schedule.Time > time)
            {
                break;
            }
        }
        if (matchSchedule != null)
            BuildPath(matchSchedule);
    }

    /// <summary>
    /// 在NPC 场景加载之后判断他是否可见
    /// </summary>
    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
        {
            SetActiveInScene();
        }
        else
        {
            SetInActiveInScene();
        }
    }

    /// <summary>
    /// 初始化npc在场景中
    /// </summary>
    private void InitNPC()
    {
        targetScene = currentScene;//初始化目标场景就是当前场景，第一次生成的时候，目标场景就是当前场景
        //保持在当前坐标的网格中心点
        currentGridPostion = grid.WorldToCell(transform.position);//得到npc的网格坐标，世界坐标转化为网格坐标
        transform.position = new Vector3(currentGridPostion.x + Settings.gridCellSize / 2f, currentGridPostion.y + Settings.gridCellSize / 2f, 0);
        targetGridPostion = currentGridPostion;
    }

    /// <summary>
    /// 主要NPC移动方法
    /// </summary>
    private void Movement()
    {
        if (!npcMove)
        {
            if (movementSteps.Count > 0)//判断移动路径堆栈中是否有构建好的步骤路径
            {
                MovementStep step = movementSteps.Pop();//拿到堆栈中的第一步并且从堆栈中移除它
                currentScene = step.sceneName;//当前场景等于步骤中存放的场景
                CheckVisiable();//实时检测NPC是否可见
                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);//拿到这一步的时间戳
                MoveToGridPosition(nextGridPosition, stepTime);         // 这个好像会不断循环一步一步走直到人物移动到目标地点
            }
            else if (!isMoving && canPlayStopAnimation)
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    /// <summary>
    /// 移动到目标网格位置上
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="stepTime"></param>
    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMoveRoutine = StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    /// <summary>
    /// 因为NPC的移动脱离于主程(自己在场景中自己管理自己移动)所以要用协程辅助
    /// </summary>
    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;//NPC移动状态改为true
        nextWorldPosition = GetWorldPosition(gridPos);
        // 还有时间用来移动
        if (stepTime > GameTime)//如果游戏时间还没到当前到这一步的时间戳
        {
            //获取用来移动的时间差,以秒为单位
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //实际移动距离
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            //计算NPC的实际移动速度
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));//与最小值作比较是为了确保速度不会低于最小值
            //速度=距离/时间(m/s)
            if (speed <= maxSpeed)//同样不能大于最大移动速度
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)//如果NPC当前距离和下一步要到达的距离还大于一个像素点说明还没有走到
                {
                    dir = (nextWorldPosition - transform.position).normalized;//NPC移动方向
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);//移动偏差
                    rb.MovePosition(rb.position + posOffset);//移动刚体到某个位置
                    yield return new WaitForFixedUpdate();//等待一小会儿
                }
            }
        }
        //如果时间已经到了就瞬移
        rb.position = nextWorldPosition;
        currentGridPostion = gridPos;
        nextGridPosition = currentGridPostion;
        npcMove = false;
    }

    /// <summary>
    /// 根据Schedule时间表/日程构建路径
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(SchedulDetails schedule)
    {
        movementSteps.Clear();//清理之前堆栈中的路径
        currentSchedule = schedule; // 设置当前的npc日程
        targetScene = schedule.targetScene; 
        targetGridPostion = (Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;
        this.interactable = schedule.interactable;
        if (schedule.targetScene == currentScene)//如果目标场景恒等于当前场景,则利用AStar开始构建最短路径
        {   // npc 的A星算法
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPostion, schedule.targetGridPosition, movementSteps);
        }
        // TODO: 跨场景移动
        else if (schedule.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);
            if (sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];
                    if (path.fromGridCell.x >= Settings.maxGridSize)
                    {
                        fromPos = (Vector2Int)currentGridPostion;
                    }
                    else
                    {
                        fromPos = path.fromGridCell;
                    }
                    if (path.gotoGridCell.x >= Settings.maxGridSize)
                    {
                        gotoPos = schedule.targetGridPosition;
                    }
                    else
                    {
                        gotoPos = path.gotoGridCell;
                    }
                    AStar.Instance.BuildPath(path.sceneName, fromPos, gotoPos, movementSteps);
                }
            }
        }
        if (movementSteps.Count > 1)//路径大于1代表可以移动了
        {
            UpdateTimeOnPath();     //更新每一步的时间时间戳
        }
    }

    /// <summary>
    /// 更新NPC每走一步的时间戳
    /// </summary>
    private void UpdateTimeOnPath()
    {
        MovementStep previousSetp = null;
        TimeSpan currentGameTime = GameTime;

        foreach (MovementStep step in movementSteps)//这里AStar算法已经构建好了堆栈,里面有了每一步的类
        {
            if (previousSetp == null)//游戏一开始NPC没有上一步,所以直接将第一步传递给父步
                previousSetp = step;

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;//记录当前的游戏时间戳

            TimeSpan gridMovementStepTime;//定义一个新的时间戳来获取走的每一步的时间戳

            if (MoveInDiagonal(step, previousSetp))//判断是否是斜方向移动,斜方向移动和直方向所需要的时间戳不相同
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));

            //累加获得下一步的时间戳
            currentGameTime = currentGameTime.Add(gridMovementStepTime);//游戏时间戳和走路时间戳累加起来形成一个时间戳条件
            //循环下一步此步变为父步
            previousSetp = step;
        }
    }

    /// <summary>
    /// 判断是否斜方向走
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previousStep"></param>
    /// <returns></returns>
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    /// <summary>
    /// 得到网格中心的世界坐标
    /// </summary>
    /// <param name="gridPos">网格坐标</param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }

    /// <summary>
    /// npc的动画切换
    /// </summary>
    /// <returns></returns>
    private void SwitchAnimation()
    {
        isMoving = transform.position != GetWorldPosition(targetGridPostion);           // 判断世界坐标和人物坐标是不是在一个点上
        anim.SetBool("isMoving", isMoving);
        if (isMoving)
        {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }
        else
        {
            anim.SetBool("Exit", false);
        }
    }

    /// <summary>
    /// 设置npc某事件而暂停的动画，比如某个事件的过程
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetStopAnimation()
    {
        //强制面向镜头
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);
        animationBreakTime = Settings.animationBreakTime;
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }
    }

    #region 设置NPC显示情况
    private void SetActiveInScene()     // 设置活动场景
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        //TODO:影子关闭
        //transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInActiveInScene()       // 在场景中设置活动（比如跨场景中途）
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        //TODO:影子关闭
        //transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 生成保存数据
    /// </summary>
    /// <returns></returns>
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add("targetGridPosition", new SerializableVector3(targetGridPostion));
        saveData.characterPosDict.Add("currentPosition", new SerializableVector3(transform.position));
        saveData.dataSceneName = currentScene;          // 保存当前场景
        saveData.targetScene = this.targetScene;        // 保存目标场景
        if (stopAnimationClip != null)          // 判断是否有停止下来的片段
        {
            saveData.animationInstanceID = stopAnimationClip.GetInstanceID();
        }
        saveData.interactable = this.interactable;
        saveData.timeDict = new Dictionary<string, int>
        {
            { "currentSeason", (int)currentSeason }
        };
        return saveData;
    }

    /// <summary>
    /// RestoreData
    /// </summary>
    /// <param name="saveDate"></param>
    public void RestoreData(GameSaveData saveDate)
    {
        isInitialised = true;
        isFirstLoad = false;
        currentScene = saveDate.dataSceneName;
        targetScene = saveDate.targetScene;
        Vector3 pos = saveDate.characterPosDict["currentPosition"].ToVector3();
        Vector3Int gridPos = (Vector3Int)saveDate.characterPosDict["targetGridPosition"].ToVector2Int();
        transform.position = pos;
        targetGridPostion = gridPos;
        if (saveDate.animationInstanceID != 0)
        {
            this.stopAnimationClip = Resources.InstanceIDToObject(saveDate.animationInstanceID) as AnimationClip;
        }
        this.interactable = saveDate.interactable;
        this.currentSeason = (Season)saveDate.timeDict["currentSeason"];
    }
}