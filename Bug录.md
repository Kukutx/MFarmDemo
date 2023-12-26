level 0：
1.我觉得可以做个地图编辑器，类似那个kukutx的物品添加器一样，可以添加地图物品建筑拖拽建筑物品，设置地图属性，和鼠标的跟随提示框，这个提示框会在鼠标选中的位置提示当前的网格地图上的瓦片的所有信息，未来如果用godot我觉得可以整合成一个mod制作器

2.我觉得对话内容可以放在mod编辑器里来设定，对话内容的文件格式可以采用json做作为读取



3.可以考虑一个事件就是，工具自身默认有自己的音效然后对应的农作物有特别要求可以添加特别的音效来覆盖默认工具音效，判断CropDataList_SO的Sound是否为none如果不是就使用定义好的音效。，之前看视频的时候M老师说 BreakTool ChopTool CollectTool 这些都记录在CropDataList_SO所以不用担心，这方面可以考虑入手一下，所以砍树和挖坑之所以没工具默认的声音也是因为这个

![image-20231215011049564](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20231215011049564.png)

![image-20231215010126240](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20231215010126240.png)



4.解决添加!InteractWithUI() 会不断的让家具图片和鼠标不断闪烁问题，为什么在其他项目里没有这问题

![image-20231226020521431](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20231226020521431.png)



5.解决在点击箱子的时候点击一个物品会显示高亮圈，然后将物品拖拽到我的背包里，高亮圈依旧遗留并没有被取消

![image-20231226020740469](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20231226020740469.png)





level 1:

1.在使用其他工具的时候或者切换人物行为动画的时候会出现冲突问题，比如arm或者切换工具时的动画不匹配，有些时候使用工具出现头发遮挡问题

2.NPC移动时有点bug，比如第一次移动的时候会到达终点时一直走路（后面我发现是我错了因为我最开始在Schedul List里的头一个没有设置事件可能是因为这个所以发生逻辑错误，我设置成8点后又能正常走路了），然后就是在测试npc移动的IsMoving点击的时候npc就跟瞬移了几个格子后在移动，而且在一些格子的时候速度明显快了几下

3.有些时候 NPC Obstacle 作为npc障碍感觉并没有生效

level 5：

1.使用通用背包的时候那个ESC不能点击

2.当打开暂停界面的时候，我希望点击ESC可以退出这个界面还有再加个返回游戏的按钮

![image-20231226011628908](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20231226011628908.png)

3.使用Reap工具割草的的时候在点击其他工具进行互动，除了工具的动画其他都变成了Reap的动画

4.将 CallGameDateEvent 设置为 CallGameDateSeasonEvent

5.季节的enum 我希望从中文转为英文

6.我希望在每个Scene里传送的GameObject能都在一个叫Transition Parent 的父组件下，至于其他多出或者个性化的都放在Other的父组件下
