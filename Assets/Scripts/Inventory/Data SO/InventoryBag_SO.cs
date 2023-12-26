using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 背包数据库
/// </summary>
[CreateAssetMenu(fileName = "InventoryBag_SO", menuName = "Inventory/InventoryBag_SO")]
public class InventoryBag_SO : ScriptableObject//背包数据库 - Bag表示容器，背包的意思
{
    public List<InventoryItem> itemList;

    /// <summary>
    /// 查找库存物品
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public InventoryItem GetInventoryItem(int ID)
    {
        return itemList.Find(i => i.itemID == ID);//拉姆达表达式
    }
}


// 然后在GameData/Inventory/Bags 创建PlayBag 文件表示这是玩家背包的数据库