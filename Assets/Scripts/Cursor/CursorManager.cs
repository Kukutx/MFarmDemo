using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MFarm.CropPlant;
using MFarm.Inventory;
using MFarm.Map;

/// <summary>
/// 此脚本来控制鼠标在不同功能下切换不同的状态图片
/// </summary>
public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;    // 正常鼠标，使用工具的鼠标，使用种子的鼠标，使用物品的鼠标
    private Sprite currentSprite;//存储当前鼠标图片，存储临时的图片内容
    private Image cursorImage;//Image是组件,Sprite是图片组件中存放的图片内容
    private RectTransform cursorCanvas;
    //建造图标跟随
    private Image buildImage;
    //鼠标检测
    private Camera mainCamera;//拿到摄像机用于屏幕坐标转化为世界坐标
    private Grid currentGrid;//拿到当前网格用于世界坐标转化为格子坐标
    private Vector3 mouseWorldPos;//世界坐标
    private Vector3Int mouseGridPos;//网格坐标
    private bool cursorEnable;      // 鼠标是否启用
    private bool cursorPositionValid;//判断当前鼠标位置是否可以点按
    private ItemDetails currentItem;    // 当前物品
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;//拿到Player的Transform来限制物品的使用范围
 
    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        //拿到建造图标
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);
        currentSprite = normal;         // 设置默认鼠标图标
        SetCursorImage(normal);
        mainCamera = Camera.main;//获取主摄像机
    }

    private void Update()
    {
        if (cursorCanvas == null)
            return;
        cursorImage.transform.position = Input.mousePosition;//图片始终跟随鼠标移动
        if (cursorEnable)          //判断鼠标是否能启用，   也可以用这个 "!InteractWithUI() && cursorEnable"   判断是否与UI互动，这个判断鼠标范围如果超过一定的范围物品图片就会消失，可按需添加 (但是有鼠标闪烁)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            //BugFix:    跟 UI 互动时，关闭建造的物品
            buildImage.gameObject.SetActive(false);
        }
    }

    #region 事件绑定
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;//注册一个当选择物品的时候触发的事件
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent; // 场景卸载之前的事件
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;//切换场景之后触发事件
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    /// <summary>
    /// 当物品被鼠标选中时，用于切换鼠标图片
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else
        {
            currentItem = itemDetails;
            currentSprite = itemDetails.itemType switch//根据物品的类型来返回指定的图片/切换对应鼠标图片
            {
                //WORKFLOW:添加所有类型对应的图片
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal //默认返回的是normal图片
            };
            cursorEnable = true;
            //显示建造物品图片
            if (itemDetails.itemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
            else
            {
                buildImage.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()//检测当前鼠标道具在地图上可用，道具图片完全显示
    {
        cursorPositionValid = true;//鼠标可以点按
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
    }
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInvalid()//检测当前鼠标道具在地图上不可用，道具图片变为红色半透明
    {
        cursorPositionValid = false;//鼠标不能点按
        cursorImage.color = new Color(1, 0, 0, 0.5f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
    }
    #endregion

    /// <summary>
    /// 查看左键鼠标点击事件，比如拿着斧头砍树这些事件
    /// </summary>
    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)//如果按下鼠标左键切鼠标当前是可用状态
        {
            //执行方法
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

    /// <summary>
    /// 实时监测鼠标指针是否可用
    /// </summary>
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));//屏幕坐标转化为世界坐标
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);//世界坐标转化为网格坐标
        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);//拿到人物所在的网格坐标
        //建造图片跟随移动
        buildImage.rectTransform.position = Input.mousePosition;
        //判断在使用范围之内
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInvalid();
            return;
        }

        // 获取当前鼠标的网格坐标返回的瓦片信息
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if (currentTile != null)
        {
            //拿到种植下的种子数据
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDug > -1 && currentTile.seedItemID == -1) SetCursorValid(); else SetCursorInvalid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItm && currentItem.canDropped) SetCursorValid(); else SetCursorInvalid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInvalid();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) SetCursorValid(); else SetCursorInvalid();
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    if (crop != null)
                    {
                        if (crop.canHarvest && crop.cropDetails.CheckToolAvailable(currentItem.itemID)) SetCursorValid(); else SetCursorInvalid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))//判断手里拿的工具ID是否和收割工具ID匹配
                        {
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInvalid();//种子成熟后才可以选中
                        }
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, currentItem)) SetCursorValid(); else SetCursorInvalid();
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);    //需要添加此命令
                    var bluePrintDetails = InventoryManager.Instance.bulePrintData.GetBulePrintDetailes(currentItem.itemID);
                    if (currentTile.canPlaceFurniturn && InventoryManager.Instance.CheckStock(currentItem.itemID) && !HaveFurnitureInRadius(bluePrintDetails))
                        SetCursorValid();
                    else
                        SetCursorInvalid();
                    break;
            }
        }
        else
        {
            SetCursorInvalid();
        }
    }

    /// <summary>
    /// 如果在半径范围内有家具，判断周围是否有其他家具如果有就不能建造
    /// </summary>
    /// <param name="bulePrintDetailes"></param>
    /// <returns></returns>
    private bool HaveFurnitureInRadius(BulePrintDetailes bulePrintDetailes)
    {
        var buildItem = bulePrintDetailes.buildPrefab;      // 要建造的东西
        Vector2 point = mouseWorldPos;                  // 获得要建造的东西的点，用于判断位置
        var size = buildItem.GetComponent<BoxCollider2D>().size;        // 东西的碰撞体大小
        var otherColl = Physics2D.OverlapBox(point, size, 0);       // 多大的范围之内，Physics2D这个能检测到，这个函数通常用于检测玩家的移动路径是否存在障碍物，或者检测玩家是否与某个特定的物体发生碰撞。
        if (otherColl != null)          // 判断范围内是否没有家具
            return otherColl.GetComponent<Furniture>();
        return false;
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
