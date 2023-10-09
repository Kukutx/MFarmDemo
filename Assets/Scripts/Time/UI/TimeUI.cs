using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 时间UI和代码进行绑定，将代码逻辑与UI连接到一起的脚本
/// </summary>
public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;         // 用来旋转日夜的
    public RectTransform clockParent;           // 用来设置时间块的那个显示
    public Image seasonImage;                   // 用来切换季节图片
    public TextMeshProUGUI dataText;            // 日期的文本
    public TextMeshProUGUI timeText;            // 时间文本
    public Sprite[] seasonSprites;//存放四张季节图片的数组
    private List<GameObject> clockBlocks = new List<GameObject>();  // 时间块

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);           // 将时间块的子物体添加到clockBlocks里
            clockParent.GetChild(i).gameObject.SetActive(false);           // 并设置时间块不显示
        }
    }

    private void OnEnable()//注册需要的委托事件
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        //EventHandler.GameDateSeason += OnGameDateSeason;
        EventHandler.GameDateEvent += OnGameDateEvent;   /*GameDateSeason*/
    }

    private void OnDisable()//注销需要的委托事件
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        //EventHandler.GameDateSeason -= OnGameDateSeason;
        EventHandler.GameDateEvent -= OnGameDateEvent;          /*GameDateSeason*/
    }

    private void OnGameMinuteEvent(int minute, int hour, Season season, int day)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dataText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        seasonImage.sprite = seasonSprites[(int)season];
        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }

    //private void OnGameMinuteEvent(int minute, int hour)
    //{
    //    timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    //}
    ////日期与季节
    //private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    //{
    //    dataText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日"; // 设置日期
    //    seasonImage.sprite = seasonSprites[(int)season];    // 设置季节
    //    SwitchHourImage(hour);              // 设置显示时间块
    //    DayNightImageRotate(hour);          // 设置切换日夜的旋转
    //}

    /// <summary>
    /// 根据小时切换时间块显示
    /// </summary>
    /// <param name="hour"></param>
    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;   // 在24小时内切换时间块（6个时间快为24点，1为开始），当前小时 / 4个不同时间状态下的日夜，得出当前的时间块是多少个，

        if (index == 0)     // 代表新的一天开始了，所有时间快都应该关闭
        {
            foreach (var item in clockBlocks)
            {
                item.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < clockBlocks.Count; i++)
            {
                if (i < index + 1)
                    clockBlocks[i].SetActive(true);
                else
                    clockBlocks[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 控制日夜图片旋转
    /// </summary>
    /// <param name="hour"></param>
    private void DayNightImageRotate(int hour)//控制日夜图片旋转
    {
        var target = new Vector3(0, 0, hour * 15 - 90);//将要旋转到的目标角度位置
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);//使用DOWTeen动画
    }
}
