using System.Collections;
using System.Collections.Generic;
using MFarm.Map;
using UnityEngine;

namespace MFarm.CropPlant
{
    /// <summary>
    /// 农作物生成器，用来生成我指定在地图上的crop物体比如种子树或者果树等
    /// </summary>
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;//Grid是当前整张地图的一片Grid而不是一个格子
        public int seedItemID;  // 种子物体id
        public int growthDays;//预先设置放置在地图当中的农作物已经生长到多少天了
        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }
        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }
        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }

        /// <summary>
        /// 生成农作物 
        /// </summary>
        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);//世界坐标转化为格子网格坐标并拿到(这里只是拿到坐标,没有拿到格子)
            if (seedItemID != 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);//拿到格子信息
                if (tile == null)//如果当前瓦片信息为null,就new一个瓦片信息
                {
                    tile = new TileDetails();
                    tile.gridX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }
                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;//重新给予信息
                GridMapManager.Instance.UpdateTileDetails(tile);//更新瓦片信息
            }
        }
    }
}

