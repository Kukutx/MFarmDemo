using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储每个道具信息,(用于背包系统，可上网查这个类名)
/// </summary>
[CreateAssetMenu(fileName = "ItemDataList_SO",menuName = "Inventory/ItemDataList")]
public class ItemDataList_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;
}
