using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MFarm.Inventory
{
    /// <summary>
    /// 控制背包打开，道具触碰显示信息等UI的控制
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public ItemTooltip itemToolTip;     //  悬浮物品提示框
        [Header("拖拽的图片")]
        public Image dragItem;
        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;      // 用来控制玩家背包的对象
        private bool bagOpened;             //判断背包是否被打开的状态
        [Header("通用背包-储物箱")]                 // 给npc的背包可以用于跟npc交易和其他  （当然这个可以作为所有背包的模板或者仓库之类的）
        [SerializeField] private GameObject baseBag;        // 定义通用背包
        public GameObject shopSlotPrefab;   //商店格子预制体
        public GameObject boxSlotPrefab;    //储物箱箱子格子预制体
        [Header("交易UI")]
        public TradeUI tradeUI;      // 交易界面
        public TextMeshProUGUI playerMoneyText;     // 玩家金钱数额的文本（在M Coin 的Money BG 下的text）
        [SerializeField] private SlotUI[] playerSlots;//玩家的每一个背包格子
        [SerializeField] private List<SlotUI> baseBagSlots;  // 通用背包的格子列表
        private void OnEnable()//当脚本执行时为委托事件添加方法(注册方法)
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI += OnShowTradeUI;
        }
        private void OnDisable()//当脚本关闭时去除/卸载委托中的函数方法
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;
        }

        private void Start()
        {
            //给每一个格子序号
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;//判断该GameObject在Hierarchy是否是激活状态
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))            // 键盘控制，按下B打开背包
            {
                OpenBagUI();
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHighlight(-1);
        }

        /// <summary>
        /// 显示交易界面事件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell">如果是true 卖出，false买入</param>
        private void OnShowTradeUI(ItemDetails item, bool isSell)       
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(item, isSell);
        }

        /// <summary>
        /// 打开通用包裹UI事件
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            //TODO:通用箱子Prefab
            GameObject prefab = slotType switch//语法糖,因为这里是通用事件,不只是打开商店,也可以用来打开箱子,所以Switch语法糖来判断传入不同的预制体
            {
                SlotType.Shop => shopSlotPrefab,
                SlotType.Box => boxSlotPrefab,
                _ => null,
            };
            //生成背包UI
            baseBag.SetActive(true);//激活背包UI面板
            baseBagSlots = new List<SlotUI>();
            for (int i = 0; i < bagData.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(1)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());//强制刷新,否则UI显示不正确
            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);      // 设置玩家背包的描点位置
                bagUI.SetActive(true);
                bagOpened = true;
            }
            //更新UI显示
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);
        }

        /// <summary>
        /// 关闭通用UI包裹事件
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateSlotHighlight(-1);
            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();
            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);        // 这里将玩家背包改回默认的描点位置
                bagUI.SetActive(false);
                bagOpened = false;
            }
        }

        /// <summary>
        /// 更新库存UI
        /// </summary>
        /// <param name="location"></param>
        /// <param name="list"></param>
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)//只有背包中的物品数量大于0才能调用更新格子的函数
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)//只有背包中的物品数量大于0才能调用更新格子的函数
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }

        /// <summary>
        /// 打开关闭背包UI，按钮点击事件
        /// </summary>
        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }

        /// <summary>
        /// 更新Slots高亮显示
        /// </summary>
        /// <param name="index">每一个格子的序号</param>
        public void UpdateSlotHighlight(int index)//更新格子选中时的高光表现，每次只有一个格子能表现高光
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHightLight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHightLight.gameObject.SetActive(false);
                }
            }
        }
    }
}

