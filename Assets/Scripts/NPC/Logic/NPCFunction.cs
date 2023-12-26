using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC 的交互逻辑，如商店等
/// </summary>
public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;//先拿到商店中售卖的道具数据库
    private bool isOpen;        // 是否打开了npc通用背包
    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            //关闭背包
            CloseShop();
        }
    }

    /// <summary>
    /// 打开商店
    /// </summary>
    public void OpenShop()
    {
        isOpen = true;
        EventHandler.CallBaseBagOpenEvent(SlotType.Shop, shopData);//触发打开背包委托事件
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);//触发游戏暂停事件               // 如果在交易中暂停游戏事件，人物不能移动
    }

    /// <summary>
    /// 关闭商店
    /// </summary>
    public void CloseShop()
    {
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotType.Shop, shopData);//触发关闭背包委托事件
        EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);//触发游戏继续事件          // 人物可移动
    }
}
