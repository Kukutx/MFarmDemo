using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.Utilities.Attribute;
/// <summary>
/// 地图数据
/// </summary>
[CreateAssetMenu(fileName ="MapData_SO",menuName ="Map/MapData")]
public class MapData_SO : ScriptableObject
{
    [SceneName] public string sceneName;
    [Header("地图信息,宽选整张地图")]
    public int gridWidth;
    public int gridHeight;
    [Header("左下角原点")]
    public int originX;
    public int originY;
    public List<TileProperty> tileProperties;
}
