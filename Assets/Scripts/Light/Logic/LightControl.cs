using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
/// <summary>
/// 灯光光影控制
/// </summary>
public class LightControl : MonoBehaviour
{
    public LightPattenList_SO lightData;   // 灯光数据库
    private Light2D currentLight;           // 当前灯光
    private LightDetails currentLightDetails;   // 当前灯光详情
    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }

    /// <summary>
    /// 实际切换灯光
    /// </summary>
    /// <param name="season"> 游戏季节 </param>
    /// <param name="lightShift"> 灯光模式 （白天或黑夜） </param>
    /// <param name="timeDifference"> 灯光强度的时间差。比如下午到晚上的时候他们的灯光强度变化 </param>
    public void ChangeLightShift(Season season,LightShift lightShift,float timeDifference)
    {
        currentLightDetails = lightData.GetLightDetails(season, lightShift);
        if (timeDifference < Settings.lightChangeDuration)
        {
            //计算颜色差值
            var colorOffst = (currentLightDetails.lightColor - currentLight.color) / Settings.lightChangeDuration * timeDifference;
            currentLight.color += colorOffst;
            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Settings.lightChangeDuration - timeDifference);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount, Settings.lightChangeDuration - timeDifference);
        }
        if (timeDifference >= Settings.lightChangeDuration)
        {
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}
