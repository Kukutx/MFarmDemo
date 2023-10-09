供参考：
https://qt9orch9g4.feishu.cn/wiki/Tbrlwl1HniHDFQkvXjHcv7N2nqc

https://zhuanlan.zhihu.com/p/493063833

https://zhuanlan.zhihu.com/p/494709520



Maps:

- 以下地图素材如果带格式为 @20 代表像素单位为20x20，也就是说一个格子里有20个像素用grid by cell size 来切割，如果没有就代表只是正常图片比如草快和家具等，切割可以用automatic来切割不需要设置像素，单独切割就能使用了
- 在切割地图的时候，将Wrap Mode 改成Clamp,将Compression改成none可以适应像素游戏，尽量不要超过2048，1024就足够了看图片是不是640x4560，将pixels per unit 改成20
- 一个小秘诀，如果按照方法一个一个去点会显得很麻烦。所以我们可以采用预设来为每个图片分配切割配置：
  ![1](D:\liuzh\Pictures\1.png)

点击Save Current to..会让你保存一个预设文件，创建一个文件夹来保存所有预设，然后以后就能采用这个预设来直接配置图片切割了

还有在切割图片的时候 Sprite Editor 的 Slice 在grid by cell size 点击 20 20 然后在Method 选择 smart这样就能保证空白区域不被切割同时可以保证轻量





还有一件事比较重要在切割人物图片的时候，比如人物的手臂肢体：
![image-20230607225130144](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230607225130144.png)

pivot要保证在Bottom 或许Bottom Center,这样锚点就在脚下，因为这是一个俯视角的游戏需要按照Y轴来渲染，来进行图册的排序，这样的话会有很明显的遮挡效果，而且这个包括这游戏里所有的图片包括家具包括场景里的物体，如果想要跟人物有个正确的遮挡效果的话,将锚点选Bottom





这部分需要将 Sprite sort point 改成Pivot：
![image-20230611004246995](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230611004246995.png)



这个组件可以将这些子物体和父物体合并成一个物体来进行渲染

![image-20230611015056024](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230611015056024.png)

![image-20230611015051672](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230611015051672.png)





因为Unity默认是 z轴渲染方向的，因为我们做的是俯视角游戏所以需要做点小修改，将 z 改成 0 将 mode 改成 Custom Axis 这样就能从y开始渲染了：
![image-20230611020020131](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230611020020131.png)





刚刚在 第七集的 2分 53秒学到了 规则瓦片（可选学），可以方便搭建地图瓦片，名叫 Rule tiles,如果不懂可以去我的Rule Tiles 文件夹看看物体都在干什么，还有一个笔刷工具叫Random Brush 可以方便自动随机添加瓦片，让草地或者水浪随机瓦片化，这样布置草地瓦片更方便，如果想单选笔刷一个要换回Default：
![image-20230611233436079](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230611233436079.png)





在第八集我学到了 Sprite atlas 这个是可以让所有的图片素材打包成完整图片（目前作用是解决图片的间隔就是地图瓦片与瓦片产生的缝隙）这样在调用和使用中unty就会忽略他们的缝隙而把他们精密的安排在一起：![image-20230611235632710](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230611235632710.png)

![image-20230611235956956](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230611235956956.png)







Layers的起名意义：
Instance : 实例化，整体化，所有需要与角色有叠层关系的都用这个图层，比如Player和身体部分



Ground Bottom：地图三层中的低层,土地

Ground Middle：地图三层中的中层，草地，花地

Ground Top： 地图三层中的高层，山，花，草，树木背景

Stuff 1 : 装饰物

Stuff 2 ：装饰物

Dig : 种田那个未被垦田过

Water: 被垦田了

Front 1

Front 2

Collision









所引用的Asset 包：
1.Colourful Hierarchy Category GameObject	(可选用于方便区分游戏物体，让游戏物体变成有有色的分类板块)

2.DOTween(HOTween v2) 	  (一个方便的小工具用来辅助变成，其实可以用传统的方式但是会开很多线程所以很麻烦)

3.Fantasy RPG Cursor Pack （一个rpg鼠标样式的的资产）







额外话题也是我不太清楚的东西：

当我创建这个文件夹和这个文件后：
![image-20230625193157784](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625193157784.png)

这是代码：
![image-20230625193225265](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625193225265.png)

然后再在Asset文件夹下创建一个新的文件夹用来存储所有物品的信息：
![image-20230625193437587](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625193437587.png)

出现了这个东西，我也不清楚为啥，可以是之前的代码让菜单栏添加了新东西吧。反正创建后会得到这个：
![image-20230625193550468](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625193550468.png)

用来很方便的管理物品信息的详情。



创建一个新文件夹，用来使用UI Toolkit 和 UI Builder,这个只有在unity编辑器时使用，是一个自定义的编辑器用来更方便的管理游戏数据和数值等等：
![image-20230625212752400](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625212752400.png)

![image-20230625214432690](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625214432690.png)

选中 Editor window，用这个帮我创建一个可以打开的菜单窗口

![image-20230625214835692](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625214835692.png)

点击confirm,就会帮我们创建一个窗口：
![image-20230625214903754](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625214903754.png)

关闭它，如果我们还想要再次调用这个自定义的编辑器可以在这里打开：
![image-20230625215221177](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625215221177.png)

然后双击点击:

![image-20230625221311423](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625221311423.png)

就会出现这个界面，类似Winform 设计UI窗口，用来设计我们的编辑器：

![image-20230625221333279](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230625221333279.png)

而这将是我们的最终效果图：
![image](C:\Users\liuzh\OneDrive\图片\Screenshots\image.png)









这段是在26集的一个重要点，首先Player Slots接收的是所有玩家可存储的格子，而这个格子的顺序就在于当前快捷栏和背包，而我们需要首先将快捷栏的格子先添加拖拽到Player Slots之后才是箱子的格子，因为这样才能做到拾取物品时首先添加到快捷栏然后才是背包格子：

![image-20230728002254598](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230728002254598.png)

![image-20230728002536613](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230728002536613.png)



在创建人物行为动作时，要注意，在Body创建的是这种的而不是普通的Controller:

![image-20230806163953919](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230806163953919.png)

![image-20230806164034164](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230806164034164.png)

这个是将自己制作好的动画一一添加：
![image-20230806164742172](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230806164742172.png)

![image-20230806164756609](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230806164756609.png)

![image-20230806164919309](C:\Users\liuzh\AppData\Roaming\Typora\typora-user-images\image-20230806164919309.png)



























BUG解决：
如果有在游戏开发的过程中，发现在边境的时候有问题，比如人物移动的时候在边境线时灵时不灵的情况下，可以去Player的刚体看看，里面有个属性Collision detection，改成continuous