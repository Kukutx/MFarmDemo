using UnityEngine;
/// <summary>
/// 种子详情
/// </summary>
[System.Serializable]//必须序列化之后,在引擎面板才能可视化
public class CropDetails
{
    public int seedItemID;      // 种子物品ID
    [Header("不同阶段成长需要的天数")]
    public int[] growthDays;            // 不同生长阶段的天数
    public int TotalGrowthDays          // 总生长阶段的天数，完整生长周期的时间，种子成长需要的总的天数
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }
    [Header("不同生长阶段物品的Prefab")]
    public GameObject[] growsPrefabs;       // 不同生长阶段的预制体
    [Header("不同阶段的图片")]
    public Sprite[] growthSprites;          // 不同生长阶段的图片
    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemID;
    [Header("每种工具的使用次数")]
    public int[] requireActionCount;
    [Header("转换新物品的ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实/掉落物信息")]
    public int[] producedItemID; //收割果实的物品ID
    public int[] produceMinAmount;//收割果实的最小获得数 以下两个都是用来随机获得的农作物的阈值
    public int[] produceMaxAmount;//收割果实的最大获得数
    [Header("掉落物的最大和最小范围范围")]
    public Vector2 spawnRadius;//生成果实的最小范围
    [Header("再次生长时间")]
    public int daysToRegrow;
    public int regrowTimes;//再次生长几次
    [Header("Options")]
    public bool generateAtPlayerPostion;  // 是否在玩家身上（位置）生成，就是当玩家在地里拔萝卜应该在玩家的头顶上或者身上生成
    public bool hasAnimation;//是否有动画
    public bool hasParticalEffect;//是否有粒子特效

    // TODO:特效 音效等
    public Vector3 effectPos;//特效产生点的坐标
    public SoundName soundEffect;  // 音效
    public ParticaleEffectType effectType;  // 效果类型

    /// <summary>
    /// 判断当前所持工具是否可用
    /// </summary>
    /// <param name="toolID"></param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 返回工具需要使用的次数
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }
}
