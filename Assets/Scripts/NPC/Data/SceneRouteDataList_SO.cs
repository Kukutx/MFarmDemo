using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 场景切换的路由列表
/// </summary>
[CreateAssetMenu(fileName ="SceneRouteDataList_SO",menuName ="Map/SceneRouteData")]
public class SceneRouteDataList_SO : ScriptableObject
{
    public List<SceneRoute> sceneRouteList;
}
