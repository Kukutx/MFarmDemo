using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MFarm.Dialogue;

/// <summary>
/// 事件中心，用来注册和调用事件的中心，创建一个脚本来控制游戏中所有的事件，静态的，全局的
/// </summary>
public static class EventHandler 
{

    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI; //定义一个委托事件，需要知道更新格子的位置和更新的道具数据
    public static void CallUpdateInventoryUI(InventoryLocation loaction, List<InventoryItem> list)//其他脚本可以随时呼叫该事件
    {
        UpdateInventoryUI?.Invoke(loaction, list);//?.:先判断事件委托是否为空再激活
    }

    public static event Action<int, Vector3> InstantiateItemInScene;//委托事件:在场景中创建一个道具预制体（也可以说在地图上生成物体，毕竟是用于初始化物品在场景里）
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)  // 调用 场景地图上生成物体，物品ID 和 坐标
    {
        InstantiateItemInScene?.Invoke(ID, pos);            
    }

    public static event Action<int, Vector3, ItemType> DropItemEvent;//人物丢弃道具后触发事件委托
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    public static event Action<ItemDetails, bool> ItemSelectedEvent;        // 物品是否被选中事件
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<int, int, Season, int> GameMinuteEvent;      // 游戏分钟的事件
    public static void CallGameMinuteEvent(int minute, int hour, Season season, int day)
    {
        GameMinuteEvent?.Invoke(minute, hour, season, day);
    }

    public static event Action<int, int, int, int, Season> GameDateEvent /*GameDateSeason*/;      // 游戏季节的事件
    public static void CallGameDateEvent(int hour, int day, int month, int year, Season season) /*CallGameDateSeason*/
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }

    public static event Action<string, Vector3> TransitionEvent;//场景切换委托事件
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    public static event Action BeforeSceneUnloadEvent;//场景卸载之前的通知，需要触发一些事件来避免报错，因为容易发生有些场景找不到物体的问题，所以这个事件主要目的就是通知游戏该场景要卸载了，该干嘛的干嘛，要保存数据赶紧保存，要存储坐标赶紧存储然后要卸载场景
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadedEvent;//加载场景之后的事件，需要触发一些事件来切换数据
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;//切换场景人物移动到指定位置委托事件
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    public static event Action<Vector3, ItemDetails> MouseClickedEvent;//鼠标点击事件
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }

    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;//实际事件,放在Player某个动作动画播放完成后调用的实际事件功能
    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    public static event Action<int, Season> GameDayEvent;//每日委托事件,每新的一天，调用一次此委托
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }

    public static event Action<int, TileDetails> PlantSeedEvent; // 种植种子事件，将当前所使用的种子种植在指定的地图瓦片位置上
    public static void CallPlantSeedEvent(int ID, TileDetails tile)
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }

    public static event Action<int> HarvestAtPlayerPostion;//在玩家指定位置生成ID物品
    public static void CallHarvestAtPlayerPostion(int ID)
    {
        HarvestAtPlayerPostion?.Invoke(ID);
    }

    public static event Action RefreshCurrentMap;//刷新当前地图委托事件
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static event Action<ParticaleEffectType, Vector3> ParticaleEffectEvent;//触发特效事件委托
    public static void CallParticaleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        ParticaleEffectEvent?.Invoke(effectType, pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()//直接在场景中生成种子树农作物等事件
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece piece)       // 是否显示对话事件
    {
        ShowDialogueEvent?.Invoke(piece);
    }

    /// <summary>
    /// 商店开启事件
    /// </summary>
    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;//调用这个委托事件,商店就会开启
    public static void CallBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }

    /// <summary>
    /// 商店关闭事件
    /// </summary>
    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;//调用这个委托事件,商店就会关闭
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }

    /// <summary>
    /// 更新游戏当前状态委托事件
    /// </summary>
    public static event Action<GameState> UpdateGameStateEvent;//游戏有进行中状态和暂停状态  ,(进行中状态如正在交易中)
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }

    /// <summary>
    /// 显示交易用户界面 （交易物品的详情界面）
    /// </summary>
    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails item, bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }

    /// <summary>
    /// 建造家具或者其他
    /// </summary>
    public static event Action<int, Vector3> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID, Vector3 pos)
    {
        BuildFurnitureEvent?.Invoke(ID, pos);
    }

    /// <summary>
    /// 灯光
    /// </summary>
    public static event Action<Season, LightShift, float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        LightShiftChangeEvent?.Invoke(season, lightShift, timeDifference);
    }

    /// <summary>
    /// 初始化音效
    /// </summary>
    public static event Action<SoundDetails> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetails soundDetails)
    {
        InitSoundEffect?.Invoke(soundDetails);
    }

    /// <summary>
    /// 播放音效事件
    /// </summary>
    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName)
    {
        PlaySoundEvent?.Invoke(soundName);
    }

    /// <summary>
    /// 开一个新游戏进度事件
    /// </summary>
    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }

    /// <summary>
    /// 结束游戏的事件
    /// </summary>
    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }
}
