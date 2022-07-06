# UdonSpy
Right-click on the web page and click on Translate to 'your language' in the popup.

---
# 安装方法
安装最新版Melon Loader至`steamapps\common\VRChat`，并将`UdonSpy.dll`放在`steamapps\common\VRChat\Mods`目录下

---
# 功能
在游戏中开启主菜单锁定玩家移动后，F10开启/关闭输入界面，输入挂有UdonBehaviour组件的GameObject的完整路径名后点击`Find`，将会在MelonLoad控制台中输出Program中所有public变量名以及toString()后的值。
<br />
例：`ObjectPool/Object1`
<br />  <br />  <br />
结尾加上` -all`可输入Program堆中变量(明明使从堆中拿的，但好像临时变量也会输出)
<br />
例：`ObjectPool/Object1 -all`
<br />  <br />  <br />
按下例输入可直接从对象获取某一具体的变量(从Object1的udon中获取variable1)
<br />
例：`-gpv ObjectPool/Object1 -nyh variable1`
<br />  <br />  <br />
- 由于Object是Il2CppSystem命名空间下的，所以直接用ToSting()进行的输出，目前常见的值类型是正常显示的，数组也做过额外处理会遍历每一个元素进行输出。引用类型根据对象不同可能会有差异。

---
# 留言
<p>
<a href="https://discordapp.com/users/774129741422788618"><img border="0" src="https://s3.bmp.ovh/imgs/2022/06/17/431929905ac3837e.png" width=30/></a>
</p>
有BUG或建议联系我
