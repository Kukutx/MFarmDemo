using UnityEngine;
using DG.Tweening;  // 使用第三方插件

/// <summary>
/// 物体/物品渐隐渐显
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]          // 确保当前游戏物体的该组件在游戏物体下一定有SpriteRenderer组件
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 逐渐恢复颜色
    /// </summary>
    public void FadeIn()
    {
        Color targetColor = new Color(1, 1, 1, 1);
        spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }

    /// <summary>
    /// 逐渐半透明
    /// </summary>
    public void FadeOut()
    {
        Color targetColor = new Color(1, 1, 1, Settings.targetAlpha);
        spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }
}
