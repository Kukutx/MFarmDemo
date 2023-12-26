using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MFarm.Inventory
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;      // 物品图标
        public Text itemName;       // 物品名称
        public InputField tradeAmount;  // 交易数量
        public Button submitButton;     // 确认按钮
        public Button cancelButton;     // 取消按钮
        private ItemDetails item;       // 物品
        private bool isSellTrade;       // 是否卖出交易  ，（如果不是那就是买）

        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);//用代码为按钮添加点击事件
            submitButton.onClick.AddListener(TradeItem);
        }

        /// <summary>
        /// 设置TradeUI显示详情
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell"></param>
        public void SetupTradeUI(ItemDetails item,bool isSell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }

        /// <summary>
        /// 确认交易物品
        /// </summary>
        private void TradeItem()
        {
            var amount = Convert.ToInt32(tradeAmount.text);    //将string强制转换为int
            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);
            CancelTrade();
        }

        /// <summary>
        /// 取消交易
        /// </summary>
        private void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}

