using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.Utilities.Attribute;
namespace MFarm.Transition
{
    /// <summary>
    /// 传送点，用来让人物传送到场景
    /// </summary>
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneToGo;        // 要传送的场景
        public Vector3 positionToGo;    // 要传送的坐标
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))  // 但是触发的碰撞体是否是Player
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}

