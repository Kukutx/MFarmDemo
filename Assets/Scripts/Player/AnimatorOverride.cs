using System.Collections;
using MFarm.Inventory;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画行为切换，比如切换拿起，砍树等动画，这个挂在物体角色身上会形成一个动画行为列表，来对于各种行为动作的动画切换
/// </summary>
public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;       // 获取player下的子物体里的所有animator
    public SpriteRenderer holdItem;     // 用于显示举起的物体是否被显示
    [Header("各部分动画列表")]
    public List<AnimatorType> animatorTypes;
    // 动画行为字典，这样可以通过名字来查找animator，这样方便animatorTypes的切换
    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }
    private void OnEnable()//注册点击事件
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent; // 如果物品被玩家选中了
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        //EventHandler.HarvestAtPlayerPostion += OnHarvestAtPlayerPostion;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        //EventHandler.HarvestAtPlayerPostion -= OnHarvestAtPlayerPostion;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    //private void OnHarvestAtPlayerPostion(int ID)
    //{
    //    //显示对应物品的图片
    //    Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID).itemOnWorldIcon;
    //    if (holdItem.enabled == false)
    //    {
    //        StartCoroutine(ShowItem(itemSprite));
    //    }
    //}

    //private IEnumerator ShowItem(Sprite itemSprite)
    //{
    //    holdItem.sprite = itemSprite;
    //    holdItem.enabled = true;
    //    yield return new WaitForSeconds(1.0f);
    //    holdItem.enabled = false;
    //}

    /// <summary>
    /// 如果物品被玩家选中的事件
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //WORKFLOW:不同的工具返回不同的动画在这里补全
        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => PartType.Carry,//假如itemType是种子,就给currentType赋值PartType.Carry
            ItemType.Commodity => PartType.Carry,
            ItemType.HoeTool => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            ItemType.BreakTool => PartType.Break,
            ItemType.ReapTool => PartType.Reap,
            ItemType.Furniture => PartType.Carry,
            _ => PartType.None          // 空，也就是切换人物本来的基础动画，啥都没有拿的那种
        };//语法糖
        if (isSelected == false)
        {
            currentType = PartType.None;        // 将当前手持物品设置为None
            holdItem.enabled = false;           // 将手持物品设置不显示
        }
        else
        {
            if (currentType == PartType.Carry)      // 判断是不是Carry
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite;        // 手持该物品
                holdItem.enabled = true;            // 启动
            }
            else
            {
                holdItem.enabled = false;
            }
        }
        SwitchAnimator(currentType);
    }

    /// <summary>
    /// 切换动画
    /// </summary>
    /// <param name="partType"></param>
    private void SwitchAnimator(PartType partType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.partType == partType)
            {
                // 在字典里查找并实时执行动画
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
