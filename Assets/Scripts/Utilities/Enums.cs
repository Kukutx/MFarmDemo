/// <summary>
/// 该文件存放所有Enum 结构体
/// </summary>
public enum ItemType            // 物品类型
{
    Seed, Commodity/*商品*/, Furniture,
    HoeTool/*锄地的工具*/, ChopTool/*砍树的工具*/, BreakTool/*砸石头的工具*/, ReapTool/*割草的工具*/, WaterTool/*浇水的工具*/, CollectTool/*搜集菜的工具*/,
    ReapableScenery/*可以被割的杂草*/
    /*
      种子、商品、家具、
      锄头工具、砍树工具、挖矿工具、收割工具、浇水工具、收集工具(菜篮子)、
      可收割的物品(杂草等)
    */
}

public enum SlotType//UI中放东西的格子在不同地方是不同的类型(人物身上，商店里，盒子里等等)
{
    Bag/*背包*/, Box /*盒子/箱子*/, Shop/*商店*/
}

public enum InventoryLocation//判断UI更新到哪里，是更新人物身上的格子还是箱子里的格子
{
    Player, Box
}

public enum PartType//判断拿到手上的
{
    None, Carry, Hoe, Break, Water, Collect, Chop, Reap
}

public enum PartName//身体的各个部分，物体需要身上那个部位拿取
{
    Body, Hair, Arm, Tool
}

public enum Season //季节类型
{
    春天, 夏天, 秋天, 冬天
}

public enum GridType//网格的类型 （构建地图信息系统）
{
    Digable, DropItem, PlaceFurniture, NPCObstacle//NPC行走的路径
}

public enum ParticaleEffectType//特效的类型
{
    None, LeavesFalling01, LeavesFalling02, Rock, ReapableScenery/*割稻草特效*/
}

public enum GameState//游戏状态枚举
{
    Gameplay/*游戏正常运行状态*/, Pause/*游戏暂停状态*/
}

public enum LightShift //游戏灯光(早上还是晚上)
{
    Morning, Night
}

public enum SoundName               // 音效的类型
{
    none, FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Reap, Water, Basket, Chop,
    Pickup, Plant, TreeFalling, Rustle,
    AmbientCountryside1, AmbientCountryside2, MusicCalm1, MusicCalm2, MusicCalm4, MusicCalm5, MusicCalm6, MusicCalm3, AmbientIndoor1
}
