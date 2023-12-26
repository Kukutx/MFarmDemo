using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏开始的主菜单界面
/// </summary>
public class MenuUI : MonoBehaviour
{
    public GameObject[] panels;     // 拿到主菜单下的不同菜单界面  (这个是用来做界面按钮的切换让其类似Tab group的效果)

    /// <summary>
    /// 点击按钮来切换菜单界面
    /// </summary>
    /// <param name="index">面板的排序索引，用不同排序的方式来达成Tab Group的效果，排序最高的界面面板会对其他按钮有遮罩效果</param>
    public void SwitchPanel(int index)      
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == index)
            {
                //Tips:Unity在Hierarchy同层级面板中优先显示在底部的Object!
                panels[i].transform.SetAsLastSibling();//SetAsLastSibling()函数:将当前object中的Hierarchy面板的transfrom移动到列表最下面(最后)
            }
        }
    }

    /// <summary>
    /// 推出游戏
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("EXIT GAME");
    }
    //一看到Sibling就应该想到和Hierarchy有关
}
