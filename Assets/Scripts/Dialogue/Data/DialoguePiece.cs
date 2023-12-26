using UnityEngine;
using UnityEngine.Events;
namespace MFarm.Dialogue
{
    /// <summary>
    /// 对话框的对话片段             这里目前没有创建DataList，但是如果需要以后可以考虑添加
    /// </summary>
    [System.Serializable]
    public class DialoguePiece  // // 序列化后的脚本,数据会呈现在inspector面板中可以可视化修改
    {
        [Header("对话详情")]
        public Sprite faceImage;    // 人物的绘图
        public bool onLeft;         // 判断是不是左边不是自动默认为右边
        public string name;         // 人物的名字
        [TextArea]//可以输入一大片的文本框
        public string dialogueText; // 对话文本
        public bool hasToPause;     // 是否需要被暂停
        [HideInInspector]public bool isDone;//判断对话是否结束
        //public UnityEvent afterTalkEvent;    // 对话结束后执行事件（这段代码目前仅用于测试，当然未来如果打算在每个对话片段结束后执行事件也可以添加）
    }
}

