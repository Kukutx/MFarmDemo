using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityGame.Utilities.Attribute;

namespace MFarm.AStar
{
    /// <summary>
    /// A星算法测试，可挂在在NPC Manager上用来测试A星算法
    /// </summary>
    public class AStarTest : MonoBehaviour
    {
        private AStar aStar;        // A星
        [Header("用于测试")]
        public Vector2Int startPos;     // 起始点
        public Vector2Int finishPos;    // 终点
        public Tilemap displayMap;      // 绘制地图
        public TileBase displayTile;    // 绘制瓦片
        public bool displayStartAndFinish;  // 是否绘制从起点到终点
        public bool displayPath;        // 是否绘制路径
        private Stack<MovementStep> npcMovementStepStack;       // npc的移动步骤

        [Header("测试移动NPC")]
        public NPCMovement npcMovement;
        public bool moveNPC;            // npc 是否移动
        [SceneName] public string targetScene;
        public Vector2Int targetPos;
        public AnimationClip stopClip;

        private void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovementStepStack = new Stack<MovementStep>();
        }

        private void Update()
        {
            ShowPathOnGridMap();
            if (moveNPC)
            {
                moveNPC = false;
                var schedule = new SchedulDetails(0, 0, 0, 0, Season.春天, targetScene, targetPos, stopClip, true);       // 设置npc的日程
                npcMovement.BuildPath(schedule);//让NPC根据这个时间表来构建移动路径
            }
        }

        /// <summary>
        /// 在地图上绘制和显示路径
        /// </summary>
        private void ShowPathOnGridMap()
        {
            if (displayMap != null && displayTile != null)
            {
                if (displayStartAndFinish)
                {
                    displayMap.SetTile((Vector3Int)startPos, displayTile);
                    displayMap.SetTile((Vector3Int)finishPos, displayTile);
                }
                else
                {
                    displayMap.SetTile((Vector3Int)startPos, null);
                    displayMap.SetTile((Vector3Int)finishPos, null);
                }
                if (displayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    aStar.BuildPath(sceneName, startPos, finishPos, npcMovementStepStack);
                    foreach (var step in npcMovementStepStack)
                    {
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                }
                else
                {
                    if (npcMovementStepStack.Count > 0)
                    {
                        foreach (var step in npcMovementStepStack)
                        {
                            displayMap.SetTile((Vector3Int)step.gridCoordinate, null);
                        }
                        npcMovementStepStack.Clear();
                    }
                }
            }
        }
    }
}

