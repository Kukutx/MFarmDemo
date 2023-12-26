using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;
/// <summary>
/// 玩家
/// </summary>
public class Player : MonoBehaviour, ISaveable //控制玩家基本操作的类
{
    private Rigidbody2D rb;
    public float speed;         // 速度
    // 监测键盘输入的Y X
    private float inputX;
    private float inputY;
    private Vector2 movementInput;      // 移动输入
    private Animator[] animators;       // 用来获取所有动画行为的数组
    private bool isMoving;              // 是否移动
    private bool inputDisable;          // 玩家此时不能操作，比如当场景进行切换或卸载时，玩家是不能行动的
    //动画使用工具
    private float mouseX;
    private float mouseY;
    private bool useTool;

    public string GUID => GetComponent<DataGUID>().guid;        // 生成唯一标识码

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();        // 获取当前的子组件下的所有子行为
        inputDisable = true;                                    // 玩家的控制暂时关掉
    }

    private void Start()
    {
        // 注册存档方法
        ISaveable saveable = this;//因为Player类继承了ISaveable接口所有能够直接赋值进ISaveable接口类型
        saveable.RegisterSaveable();//注册Player类放入SaveLoadManager列表中代表是要被保存为数据的类
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void Update()
    {
        if (!inputDisable)
        {
            PlayerInput();  // 实时获取玩家的输入
        }
        else
        {
            isMoving = false;
        }
        SwitchAnimator();
    }
    private void FixedUpdate()
    {
        if (!inputDisable) Movement();         // 因为用到了刚体也就是物理需要在FixedUpdate调用
    }

    /// <summary>
    /// 结束游戏事件
    /// </summary>
    private void OnEndGameEvent()
    {
        inputDisable = true;
    }

    /// <summary>
    /// 开启新游戏进度事件
    /// </summary>
    /// <param name="index"></param>
    private void OnStartNewGameEvent(int index)
    {
        inputDisable = false;
        transform.position = Settings.playerStartPos;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputDisable = false;//加载完场景之后玩家才能操控，和行动
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    /// <summary>
    /// 更新游戏状态事件
    /// </summary>
    /// <param name="gameState"></param>
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Pause:
                inputDisable = true;//玩家不能控制
                break;
            case GameState.Gameplay:
                inputDisable = false;//玩家可以控制
                break;
        }
    }

    /// <summary>
    /// 鼠标点击事件，针对左键，比如手持斧头砍树的时候点击触发事件
    /// </summary>
    /// <param name="mouseWorldPos"></param>
    /// <param name="itemDatils"></param>
    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDatils)
    {
        if (useTool)
            return;
        //TODO:执行动画
        if (itemDatils.itemType != ItemType.Seed && itemDatils.itemType != ItemType.Commodity && itemDatils.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDatils));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDatils);
        }
    }

    /// <summary>
    /// 使用工具的携程
    /// </summary>
    /// <param name="mouseWorldPos"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>
    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetTrigger("useTool");
            //人物的面朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);             // 这个是在Animation 动画的执行动画时间片段。差不多是0.4到0.5之间
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);
        //等待动画结束
        useTool = false;
        inputDisable = false;
    }

    /// <summary>
    /// 玩家输入
    /// </summary>
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        if (inputX != 0 && inputY != 0)      // 因为 斜移动 速度明显比 x y移动要快些所以需要限制以下
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }
        //走路状态速度，这个默认是跑步状态只有按键才能走路，当然也可以反一下
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);
        isMoving = movementInput != Vector2.zero;
    }

    /// <summary>
    /// 人物移动
    /// </summary>
    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    /// <summary>
    /// 切换动画控制器
    /// </summary>
    private void SwitchAnimator()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

    /// <summary>
    /// 生成并保存要存储的数据
    /// </summary>
    /// <returns></returns>
    public GameSaveData GenerateSaveData()
    {
        //写好了将当前人物坐标数据存入到一个新的GameSaveData
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));
        return saveData;
    }

    /// <summary>
    /// 恢复存档存储的对应数据，简称读取进度
    /// </summary>
    /// <param name="saveDate"></param>
    public void RestoreData(GameSaveData saveDate)
    {
        var targetPosition = saveDate.characterPosDict[this.name].ToVector3();      // 在存档的字典里获得人物的坐标
        transform.position = targetPosition;                    // 赋值人物坐标
    }
}
