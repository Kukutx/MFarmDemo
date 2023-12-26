using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 灯光模式列表，存储灯光和光影的颜色
/// </summary>
[CreateAssetMenu(fileName ="LightPattenList_SO",menuName ="Light/Light Patten")]
public class LightPattenList_SO : ScriptableObject
{
    public List<LightDetails> lightPattenList;      // 灯光模式的列表
    /// <summary>
    /// 根据季节和周期返回灯光详情
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">周期</param>
    /// <returns></returns>
    public LightDetails GetLightDetails(Season season,LightShift lightShift)
    {
        return lightPattenList.Find(l => l.season == season && l.lightShift == lightShift);//拉姆达表达式
    }
}

/// <summary>
/// 灯光详情
/// </summary>
[System.Serializable]
public class LightDetails//灯光类
{
    public Season season;//每个季节不同,散发的灯光也不同
    public LightShift lightShift;   // 游戏灯光模式(早上还是晚上)
    public Color lightColor;        // 灯光颜色
    public float lightAmount;//灯光强度
}
