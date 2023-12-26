using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MFarm.Save;

/// <summary>
/// 存档进度框
/// </summary>
public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;    // 存档进度的游戏时间和场景
    private Button currentButton;       // 当前点击的存档进度框 （按钮）
    private DataSlot currentData;       // 当前时间
    private int index => transform.GetSiblingIndex(); // 得到当前object在同层级Hierarchy中的位置索引，存档进度的索引
   
    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);//为当前按钮添加监听点击事件
        //如果点击了当前按钮就调用(LoadGameData)函数
    }

    private void OnEnable()
    {
        SetupSlotUI();
    }

    /// <summary>
    /// 设置存档的进度条
    /// </summary>
    private void SetupSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[index];
        if (currentData != null)
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {
            dataTime.text = "这个世界还没有开始";
            dataScene.text = "梦还没开始";
        }
    }

    /// <summary>
    /// 加载存档进度
    /// </summary>
    private void LoadGameData()
    {
        //Debug.Log(index);
        if (currentData != null)
        {
            SaveLoadManager.Instance.Load(index);
        }
        else
        {
            Debug.Log("新游戏");
            EventHandler.CallStartNewGameEvent(index);          // 在空的存档进度上开启新游戏
        }
    }
}
