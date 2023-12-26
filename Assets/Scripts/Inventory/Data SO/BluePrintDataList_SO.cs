using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 家具建造图纸的数据库，存放建造图纸类的数据库
/// </summary>
[CreateAssetMenu(fileName ="BulePrintDataList_SO",menuName ="Inventory/BulePrintDataList_SO")]
public class BluePrintDataList_SO : ScriptableObject
{
    public List<BulePrintDetailes> bulePrintDataList;           // 图纸的数据列表

    /// <summary>
    /// 查找图纸的详情信息
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public BulePrintDetailes GetBulePrintDetailes(int itemID)    
    {
        return bulePrintDataList.Find(b => b.ID == itemID);//b代表返回的BulePrintDetailes结果
    }
}

/// <summary>
/// 建造图纸类
/// </summary>
[System.Serializable]
public class BulePrintDetailes
{
    //建造图纸的属性
    public int ID;
    public InventoryItem[] resourceItem = new InventoryItem[4];//用于建造所需要消耗的资源
    public GameObject buildPrefab;//建筑的预制体
}
