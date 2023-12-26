using UnityEngine;

/// <summary>
/// 保存数据的时候为每份数据产生出唯一标识码，用来方便标识
/// </summary>
[ExecuteAlways]//代表该脚本需要一直运行
public class DataGUID : MonoBehaviour
{
    public string guid;         // 唯一标识码
    private void Awake()
    {
        if (guid == string.Empty)
        {
            guid = System.Guid.NewGuid().ToString();//生产新的GUID,GUID:是一个十六位的字符串且是唯一性的,用它来保存每一个SaveableItem
        }
    }
}
