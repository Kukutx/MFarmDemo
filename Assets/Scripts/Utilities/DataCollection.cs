using System.Collections.Generic;
using UnityEngine;
using UnityGame.Utilities.Attribute;
/// <summary>
/// 该文件用来存放所有类，接口和结构体，这样方便管理
/// </summary>

/// <summary>
/// 物品详情
/// </summary>
[System.Serializable]   //加了序列化之后，实例化该类后里面的公有变量可以直接在UnityInspector面板显示出来，可以十分方便的可视化设置变量
public class ItemDetails
{
    public int itemID;                          //物品的ID
    public string itemName;                     //物品的名字
    public ItemType itemType;                   //物品的类型

    public Sprite itemIcon;                     // 物品图标
    public Sprite itemOnWorldSprite;            // 在地图时显示所产生的图片
    public string itemDescription;              // 物品详情
    public int itemUseRadius;                   // 物品能在多少网格内进行使用,简单的说就是在地图上网格的占用/使用范围

    public bool canPickedup;                    // 是否可以被拾取    
    public bool canDropped;                     // 是否可以被丢在地上        
    public bool canCarried;                     // 是否可以被举起

    public int itemPrice;                       // 物品的价值
    [Range(0, 1)]
    public float sellPercentage;                // 买入卖出的百分比，假设物品买来是50块，卖出时候是原价的百分之五十     //物品销售的百分比(售卖物品会相对于原价打折扣)
}


/// <summary>
/// 存放物品数据的结构
/// </summary>
[System.Serializable]
public struct InventoryItem //相比于class,struct结构体更适合来存放背包数据,因为它会默认为0而不是null,避免背包过多报null
{
    public int itemID;          // 物品id
    public int itemAmount;      // 物品数量
}

/// <summary>
/// 要切换的不同类型的动画
/// </summary>
[System.Serializable]
public class AnimatorType
{
    public PartType partType;           // 身体部位使用工具类型
    public PartName partName;           // 身体部位名称
    public AnimatorOverrideController overrideController; // 动画切换
}

/// <summary>
/// 该类代表在场景中物体的坐标类
/// </summary>
[System.Serializable]
public class SerializableVector3//无论是2D游戏还是3D游戏,都需要使用此方法创建此类来实现物品的坐标获取,这样即方便记录场景里的物品也方便保存
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)//构造函数,该类被触发时立即调用(初始化)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public Vector2Int ToVector2Int()//只返回x,y且为整形
    {
        return new Vector2Int((int)x, (int)y);
    }
}

/// <summary>
/// 代表在场景中的物体的类型，用来存储场景里的物品坐标信息
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int ItemID;
    public SerializableVector3 position;
}

# region 构建地图信息系统

/// <summary>
/// 代表场景中玩家自己新建造的家具类
/// </summary>
[System.Serializable]
public class SceneFurniture
{
    //它们有什么属性↓
    //TODO:更多属性信息
    public int ItemID;                  // 物品ID
    public SerializableVector3 position;//它们有ID和位置信息
    public int boxIndex;            // 框索引
}

/// <summary>
/// 瓦片属性类,定义一片瓦片是否可以种植,挖掘,丢弃道具,放置家具等等
/// </summary>
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;//瓦片坐标
    public GridType gridType;       // 网格属性类型
    public bool boolTypeValue;      // 检定定义的属性是否可丢弃挖掘等等
}

/// <summary>
/// 瓦片的信息，定义一个格子的具体信息类，这个格子上面有什么属性
/// </summary>
[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;//格子的坐标
    public bool canDig;//格子能被挖掘
    public bool canDropItm;//格子上能丢物品
    public bool canPlaceFurniturn;//格子能放置家具
    public bool isNPCObstacle;//格子是NPC的路径
    public int daysSinceDug = -1;//格子被挖掘后开始计算天数，来控制农作物成长的逻辑
    public int daysSinceWatered = -1;//格子被浇水后计算天数
    public int seedItemID = -1;//格子上被种植的种子的ID
    public int growthDays = -1;//植物成长的天数
    public int daysSinceLastHarvest = -1;//某些植物可以反复种植，在收获后回到一定的天数，阶段
}

#endregion

[System.Serializable]
public class NPCPosition
{
    public Transform npc;               // npc方向
    /*[SceneName]*/ public string startScene;   // 可以添加[SceneName]在前面但是因为unity当前版本好像有bug如果有两个节点切换场景会一起切换虽然值没有变化，所以暂时不用 [SceneName] public string startScene;
    public Vector3 position;            // npc位置
}

/// <summary>
/// 场景路由
/// </summary>
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;    // 从那个场景来
    public string gotoSceneName;    // 要去那个场景
    public List<ScenePath> scenePathList;       // 场景路径列表
}

/// <summary>
/// 场景路径,用来NPC切换场景
/// </summary>
[System.Serializable]
public class ScenePath
{
    public string sceneName;         // 场景名称
    public Vector2Int fromGridCell;  // 从那个场景的网格坐标点来
    public Vector2Int gotoGridCell;  // 要去那个场景的网格坐标点
}
