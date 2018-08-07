# vr_diefire
使用unity3d制作游戏，搭载VR设备实现消防灭火场景
2018/8/12版本

场景说明：
灭火功能是本场景中和使用者交互最多，也是使使用者最能亲身感受到的功能，此功能的设计更加具体化，可视化。
当处在VIVE场景中的演示者观察到火源时，根据常识选择是否断开电源；
按下消火栓箱的按钮打开消火栓箱后，要求使用者根据火源选择使用泡沫灭火器或者消火栓灭火。


目标：使用Unity3D游戏引擎搭载硬件设备HTCVIVE建立消防灭火场景。

准备工作：获取和制作场景模型，获取相关插件（如VRTK等）

实现功能：

1.漫游的实现（在游戏场景中怎么走）：
 顶天立地，首先需要立足点，plane面 为了实现TouchPad 行走，
放在controller上面的三个脚本：
 VRTK_ControllerEvents.cs VRTK_ControllerActions.cs VRTK_InteractTouch.cs 
还需要在CamerRig上面放上两个脚本： VRTK_TouchpadWalking.cs VRTK_PlayerPresence.cs

2.创建透明的模型：
菜单：GameObject->Create Other->Cube（立方体）
随便导入一张图片。
然后选中他，就可以在右侧的inspector窗口中看到它的所有属性，将刚才导入的图片直接从Hierarchy视图中

拖拽到立方体的属性窗口的空白处。这时候你就会发现Material下面多出一个Shader的东西，这个东西有一项是texture，

它里面就是我们刚才拖进去的图片，在看看立方体上面已经是被我们的图片所覆盖了。

接下来就是真正让立方体透明的东西了。

在shader右侧的下拉框中，选择transparant->Diffuse。完了后点击main color的右侧的白色矩形框（这里是设置立方体的颜色），

弹出的框里面，有设置red, green, blue, Aphla, 分别用它们的首字母替代的，我们调整A的值，默认是255， 就是完全不透明，

0是完全透明。接下来就根据你的需要自己调整吧。



3.FBX偏离的问题：

FBX偏离问题： 由于max模型导入U3D总会有轴向偏离问题 解决方法是使用空物体旋转 
用空物体包容实际物理物体 X：180°Y：-90°Z:270° valve X：90°Y：0°Z:90°

4.物体抓取的实现：

Vive Grip需要在两个控制器上加上两个点，再点上放上ViveGrip_GripPoint.cs ViveGrip_ButtonManager.cs 
两个脚本 需要按下的物体上面加上需要加上转向物体，消火栓箱上面的脚本有两个：GX_ExitinguisherBoxButton ViveGrip_Interactable


5.灭火的实现：

 盒碰撞器 Box Collider（） 网格渲染器 Mesh Renderer（网格渲染器从网格过滤器获得几何形状，
并且根据物体的Transform组件的定义位置进行渲染。）


