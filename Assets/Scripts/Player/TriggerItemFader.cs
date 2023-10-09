using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触碰物体时，颜色渐变淡出
/// </summary>
public class TriggerItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();        // 获取组件的所有子元素

        if(faders.Length > 0)
        {
            foreach(var item in faders)
            {
                item.FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();        // 获取组件的所有子元素 (目前对树，碰到树的时候所有itemFader类都会被抓取到)

        if (faders.Length > 0)
        {
            foreach (var item in faders)
            {
                item.FadeIn();
            }
        }
    }
}
