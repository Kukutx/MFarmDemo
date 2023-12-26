using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace MFarm.Inventory
{
    /// <summary>
    /// 是否显示提示框
    /// </summary>
    [RequireComponent(typeof(SlotUI))]//本脚本挂载的前提是目标物品必须拥有SlotUI脚本,确保这个组件不会乱被使用
    public class ShowItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//调用两个接口
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        /// <summary>
        /// 鼠标移入事件
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                inventoryUI.itemToolTip.gameObject.SetActive(true);     // 设置提示框显示
                inventoryUI.itemToolTip.SetupTooltip(slotUI.itemDetails, slotUI.slotType);          // 设置提示框信息
                inventoryUI.itemToolTip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0); // 设置提示框的锚点位置  
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;  // 显示位置

                // 是否显示建造资源面板
                if (slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    inventoryUI.itemToolTip.resoursePanel.SetActive(true);
                    inventoryUI.itemToolTip.SetupResouresPanel(slotUI.itemDetails.itemID);
                }
                else
                {
                    inventoryUI.itemToolTip.resoursePanel.SetActive(false);  // 设置提示框不显示
                }
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 鼠标移出事件
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }
    }
}

