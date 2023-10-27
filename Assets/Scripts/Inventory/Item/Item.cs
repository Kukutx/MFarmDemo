using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MFarm.CropPlant;
namespace MFarm.Inventory
{
    /// <summary>
    /// 物品/物体
    /// </summary>
    public class Item : MonoBehaviour //掉落在世界的物品，绑定数据
    {
        public int itemID;
        private SpriteRenderer spriteRenderer;//图片组件
        private BoxCollider2D coll;             // 碰撞体组件（这里用来修改碰撞体尺寸大小）
        public ItemDetails itemDetails;//物品信息

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (itemID != 0) Init(itemID);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ID"></param>
        public void Init(int ID)
        {
            itemID = ID;
            //Inventory获得当前数据
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            if (itemDetails != null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;
            }
            //修改碰撞体尺寸
            Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
            coll.size = newSize;        // 设置尺寸
            coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);        // 让描点pivot在图标最底部
            if (itemDetails.itemType == ItemType.ReapableScenery)
            {
                //gameObject.AddComponent<ReapItem>();
                //gameObject.GetComponent<ReapItem>().InitCropData(itemDetails.itemID);
                //gameObject.AddComponent<ItemInterActive>();
            }
        }
    }
}

