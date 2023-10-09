using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace MFarm.Inventory
{
    /// <summary>
    /// 物品栏格子或者箱子格子的UI
    /// </summary>
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler //调用几个unity自带的点按事件接口(点击,开始拖拽,正在拖拽,拖拽结束)需要的话可以去看unity的官方手册*/
    {
        [Header("组件获取")]
        [SerializeField] private Image slotImage;//[SerializeField]可以在可视化面板中为私有变量直接托取赋值,slotImage单个物品框内的图片
        [SerializeField] private TextMeshProUGUI amountText; // 单个物品框格子内的物品数量
        [SerializeField] public Image slotHightLight;        // 点击后的高亮显示
        [SerializeField] private Button button;              // 获取按钮，这里的按钮作用是当点击单个物品框时会有点击效果，而我们需要设置为当没有物品时不会有点击效果
        [Header("格子类型")]
        public SlotType slotType;               // 判断当前物品框格子是属于在那个类型的容器
        public bool isSelected;                 // 判断格子是否被选中
        public int slotIndex;                   //每一个格子有对应的序号

        //物品信息
        public ItemDetails itemDetails;         // 获得物品信息
        public int itemAmount;                  // 物品数量
        public InventoryLocation Location       //属性
        {
            get//可读
            {
                return slotType switch
                {
                    SlotType.Bag => InventoryLocation.Player,
                    SlotType.Box => InventoryLocation.Box,
                    _ => InventoryLocation.Player
                };
            }
        }

        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();   // 获取父组件(背包)的数据进行操控

        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)//如果当前单独的格子里面的物品ID为0就说明当前物品格子为null
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// 更新格子的UI信息
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">持有物品数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            button.interactable = true;
            slotImage.enabled = true;
        }

        /// <summary>
        /// 更新格子为null，见过Slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHighlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        /// <summary>
        /// 点击事件,对了关于这块记得去 Slot_Bag (Image ，Highlight 和 Amout) 和DragItemImage 取消 Raycast Target,因为会挡住鼠标射线可以去网上查看
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)//接口里面的函数方法,具体是干什么的可以去Unity官方手册中查看
        {
            if (itemDetails == null) return;//如过点击的这个格子没有任何物品，则无法点击
            isSelected = !isSelected;       //切换一下选中的状态，是否被选中
            slotHightLight.gameObject.SetActive(isSelected);  // 该代码是用来测试高亮的可以删除
            inventoryUI.UpdateSlotHighlight(slotIndex);     //更新格子选中时的高光表现，每次只有一个格子能表现高光
            if (slotType == SlotType.Bag)//如果格子类型是背包格子，才能触发切换动画效果（这个动画效果可以是举起，手持工具等等）
            {
                // 通知物品被选中的状态和信息，这样方便切换动画比如拿起工具
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);//触发点击委托事件
            }
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)//都是Unity自带的接口中的函数方法,具体用途可以查看Unity手册查看
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;//将格子中的图片赋值给准备拖拽的图片
                inventoryUI.dragItem.SetNativeSize();//适配一下图片大小，图片原本的尺寸
                isSelected = true;//拖拽的图片默认变为选择状态
                inventoryUI.UpdateSlotHighlight(slotIndex);//变为高光
            }
        }

        /// <summary>
        /// 正在拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;//拖拽过程中不断将鼠标三维向量赋值给图片
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;//拖拽结束,关闭图片
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);

            if (eventData.pointerCurrentRaycast.gameObject != null)//拖拽物体停止的当前射线之下不为null
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)//如果图片触碰到的物品没有SlotUI脚本即无法互动，直接返回
                {
                    return;
                }
                var targatSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();//将目标格子获取
                int targetIndex = targatSlot.slotIndex;//获得目标格子的序号
                ////在Player自身背包中交换
                if (slotType == SlotType.Bag && targatSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
                //else if (slotType == SlotType.Shop && targatSlot.slotType == SlotType.Bag)//买
                //{
                //    EventHandler.CallShowTradeUI(itemDetails, false);
                //}
                //else if (slotType == SlotType.Bag && targatSlot.slotType == SlotType.Shop)//卖
                //{
                //    EventHandler.CallShowTradeUI(itemDetails, true);
                //}
                //else if (slotType != SlotType.Shop && targatSlot.slotType != SlotType.Shop && slotType != targatSlot.slotType)
                //{
                //    //跨背包数据交换物品
                //    InventoryManager.Instance.SwapItem(Location, slotIndex, targatSlot.Location, targatSlot.slotIndex);
                //}

                //拖拽完成后关闭所有的高亮显示
                inventoryUI.UpdateSlotHighlight(-1);
            }
            //else//测试扔在地上,因为某些缺陷和问题这个只是用来当作测试
            //{
            //    if (itemDetails.canCarried)
            //    {// 将物品拖拽到外面并不在如何背包或者格子内，丢在地图外面
            //        //鼠标对应的世界地图上的坐标
            //        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));//将鼠标的屏幕坐标转化为世界坐标
            //        EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //    }
            //}
        }
    }
}
