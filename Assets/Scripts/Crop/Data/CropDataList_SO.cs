using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 创建种子数据库
/// </summary>
[CreateAssetMenu(fileName ="CropDataList_SO",menuName ="Crop/CropDataList")]
public class CropDataList_SO : ScriptableObject
{
    public List<CropDetails> cropDetailsList;
}
