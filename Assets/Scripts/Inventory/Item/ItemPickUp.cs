using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory
{
    /// <summary>
    /// 物品是否被拾取
    /// </summary>
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                if (item.itemDetails.canPickedup)       // 判断物品属性是否可被拾取起来
                {
                    ////拾取物品添加到背包
                    InventoryManager.Instance.AddItem(item, true);
                    ////播放音效
                    //EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}

