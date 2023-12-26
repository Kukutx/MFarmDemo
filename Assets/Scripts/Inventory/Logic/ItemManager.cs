using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;
namespace MFarm.Inventory
{
    /// <summary>
    /// 专门用来管理世界地图中所有的物品的管理类
    /// </summary>
    public class ItemManager : Singleton<ItemManager>, ISaveable
    {
        public Item itemPrefab;             // 物品预制体
        public Item bounceItemPrefab;       // 丢弃物品预制体
        private Transform itemParent;       // 场景里物品的集合
        private Transform playerTransform => FindObjectOfType<Player>().transform;      // 获取玩家方向组件

        public string GUID => GetComponent<DataGUID>().guid;         // 生成唯一标识码

        //记录场景Item，用来方便保存
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();//该字典通过场景名来保存场景中存在的物体
        //记录场景中的家具(string:场景名字)
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
        }

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;//为委托事件添加函数方法
            EventHandler.DropItemEvent += OnDropItemEvent;//扔东西委托事件
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;      // 切换场景前委托事件
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;         // 切换场景后委托事件
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;              // 生成家具事件
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;              // 开启新游戏进度事件
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }

        private void OnStartNewGameEvent(int index)
        {
            sceneItemDict.Clear();
            sceneFurnitureDict.Clear();
        }

        /// <summary>
        /// 生成建造家具事件
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="mousePos"></param>

        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            BulePrintDetailes bulePrint = InventoryManager.Instance.bulePrintData.GetBulePrintDetailes(ID);
            var buildItem = Instantiate(bulePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);
            if (buildItem.GetComponent<Box>())
            {
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);
            }
        }


        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
            GetAllSceneFurnitures();
        }

        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;    //场景加载之后再寻找ItemParent避免报空
            RecreateAllItems();
            RebuildFurniturn();
        }

        /// <summary>
        /// 场景中生成物品
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pos"></param>
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);//克隆物体
            item.itemID = ID;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
        }

        /// <summary>
        /// 在场景丢弃东西
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="mousePos"></param>
        /// <param name="itemType"></param>
        private void OnDropItemEvent(int ID, Vector3 mousePos, ItemType itemType)
        {
            if (itemType == ItemType.Seed) return;
            //TODO:扔东西的效果
            var item = Instantiate(bounceItemPrefab, playerTransform.position, Quaternion.identity, itemParent);//克隆物体
            item.itemID = ID;
            var dir = (mousePos - playerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }

        /// <summary>
        /// 获得当前场景中的所有Item（用来存储和重新加载的和下面配套）
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();//存放当前场景中物体的瓶子
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem()
                {
                    ItemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };//找到当前场景中的所有Item并将它的属性传入到sceneItem中
                currentSceneItems.Add(sceneItem);//再将sceneItem放入临时列表
            }
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新Item数据列表
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else//如果是新场景
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }

        /// <summary>
        /// 刷新重建当前场景中的物品
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            //TryGetValue:根据键尝试得到值,返回bool
            //out currentSceneItems: 反向输出,如果之前的bool为true则反向输出currentSceneItems将sceneItemDict的Value输出到currentSceneItems中
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    //无论场景中有没有物品先清场
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }
                    //将当前场景中有的物品给复制到场景中
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.ItemID);
                    }
                }
            }
        }

        /// <summary>
        /// 获取场景中的所有家具
        /// </summary>
        private void GetAllSceneFurnitures()
        {
            List<SceneFurniture> currentSceneFurnitures = new List<SceneFurniture>();//存放当前场景中物体的瓶子
            foreach (var item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture()
                {
                    ItemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };//找到当前场景中的所有Furniture并将它的属性传入到sceneFurniture中

                // FixBug : 没有这个可能会将所有箱子的序号刷新为0
                if (item.GetComponent<Box>())
                {
                    sceneFurniture.boxIndex = item.GetComponent<Box>().index;           // 获取所有箱子编号并存储
                }
                currentSceneFurnitures.Add(sceneFurniture);//再将sceneFurniture放入临时列表
            }
            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新Furniture数据列表
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurnitures;
            }
            else//如果是新场景
            {
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurnitures);
            }
        }

        /// <summary>
        /// 重建当前场景家具
        /// </summary>
        private void RebuildFurniturn()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();        // 当前场景的所有家具
            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if (currentSceneFurniture != null)
                {
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                    {
                        BulePrintDetailes bulePrint = InventoryManager.Instance.bulePrintData.GetBulePrintDetailes(sceneFurniture.ItemID);  // 获取蓝图
                        var buildItem = Instantiate(bulePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);
                        if (buildItem.GetComponent<Box>())
                        {
                            buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成保存数据
        /// </summary>
        /// <returns></returns>
        public GameSaveData GenerateSaveData()
        {
            GetAllSceneItems();
            GetAllSceneFurnitures();//因为场景中的物品是在场景加载卸载后才保存的,未卸载前拿不到所有物品或家具,必须这里手动拿一下
            GameSaveData saveData = new GameSaveData();
            saveData.sceneItemDict = this.sceneItemDict;
            saveData.sceneFurnitureDict = this.sceneFurnitureDict;
            return saveData;
        }

        /// <summary>
        /// 恢复数据
        /// </summary>
        /// <param name="saveDate"></param>
        public void RestoreData(GameSaveData saveDate)
        {
            this.sceneItemDict = saveDate.sceneItemDict;
            this.sceneFurnitureDict = saveDate.sceneFurnitureDict;
            RecreateAllItems();
            RebuildFurniturn();//刷新
        }
    }
}

