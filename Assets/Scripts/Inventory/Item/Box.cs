using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory
{
    /// <summary>
    /// 储物箱的逻辑
    /// </summary>
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;      // 储物箱标准模板 （在游戏开始的时候如果箱子为空会基于这个赋值一份出来，这样保证游戏开始的时候都是同样是新的空的）
        public InventoryBag_SO boxBagData;          // 储物箱的数据
        public GameObject mouseIcon;                // 鼠标图标
        private bool canOpen = false;               // 是否可以打开关闭  因为可以打开的时候会显示一个鼠标的图标
        private bool isOpen;                        // 是否已经打开
        public int index;                           // 场景中每创建一个新的box该index就累加，计算总共有多少箱子

        private void OnEnable()
        {
            if (boxBagData == null)         // 如果为空
            {
                boxBagData = Instantiate(boxBagTemplate);       // 这时候就赋值储物箱模板，来初始化为空
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isOpen && canOpen && Input.GetMouseButtonDown(1))
            {
                //打开箱子
                EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }
            if (!canOpen && isOpen)
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
            if (isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }

        /// <summary>
        /// 初始化箱子数据
        /// </summary>
        /// <param name="boxIndex"></param>
        public void InitBox(int boxIndex)
        {
            index = boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null)//刷新地图读取数据
            {
                boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(key);
            }
            else//新建箱子
            {
                InventoryManager.Instance.AddBoxDataDict(this);
            }
            //Debug.Log(boxIndex);
        }
    }
}

