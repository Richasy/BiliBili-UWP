<p align="center">
<img src="https://i.loli.net/2020/08/30/sn8ov9cYDCGeWPk.png"/>
</p>

<div align="center">

# 哔哩

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/Richasy/BiliBili-UWP)](https://github.com/Richasy/BiliBili-UWP/releases) ![GitHub Release Date](https://img.shields.io/github/release-date/Richasy/BiliBili-UWP) ![GitHub All Releases](https://img.shields.io/github/downloads/Richasy/BiliBili-UWP/total) ![GitHub stars](https://img.shields.io/github/stars/Richasy/BiliBili-UWP?style=flat) ![GitHub forks](https://img.shields.io/github/forks/Richasy/BiliBili-UWP)

`哔哩`是一款设计精美的第三方UWP应用

</div>

## 项目声明

> 我做这个应用一方面是出于对B站的喜爱，另一方面是对自己的一个挑战。我想尝试将一款移动应用解构并移植到桌面上，这能让我对移动交互与桌面交互有更深的理解。
>
> 但该项目由于一些不可抗力，已经停止开发。不再主动尝试修复bug（除非我自己遇到），也不再开发新的功能，目前已经在 **Microsoft Store** 中下架。
>
> 本应用出于学习的目的进行开发，不进行任何商业相关的行为。同时尽管应用能显示4K清晰度选项，但如果你不是大会员，则片源也不会是 4K。
>
> 尽管是原生 UWP ，但应用在 ARM 设备，比如` Surface Pro X` 上可能无法运行，我也不知道具体原因，可能要等有相关设备了才能测试。
>
> 应用不提供观看直播及视频下载的功能，原因固然是多方面的，但主要原因还是我没这需求。

## 简介

应用提供了两种显示模式：`桌面模式`及`平板模式`，截图如下：

**桌面模式**

![桌面](https://i.loli.net/2020/08/30/Y4vV6LjIxwidhBk.png)

**平板模式**

![平板](https://i.loli.net/2020/08/30/ywEBWn3Vr94k16x.png)

桌面模式适用于1080P以上的显示器，信息密度较高，由于同屏渲染资源较多，对配置要求也相对较高。

平板模式适用于小平板或者XBOX，在操作上比较适合触摸，信息密度较低，所以对配置要求也更低一些。

## 开发环境

|||
|-|-|
|开发工具|Visual Studio 2019|
|最低版本|Windows10 1809|
|目标版本|Windows10 2004|

## 常见问题

<https://www.richasy.cn/document/bilibili/qa.html>

## 如何使用及安装

1. 克隆项目到本地
2. 使用 Visual Studio 2019 打开项目
3. 在 `package.appxmanifest` 中的 `Package` 选项卡下，重新生成一个测试证书
4. 重新生成项目并部署

---

如果你要安装，请在旁边[Release](https://github.com/Richasy/BiliBili-UWP/releases)中下载对应你系统的压缩包，解压后右键`install.ps1`，根据提示进行安装。

*如果出现需要手动安装证书的情况，请双击包内的证书，将其导入到本地计算机->受信任的根证书目录中，然后再走一遍应用安装流程即可。*

## 设计思路

BiliBili是以移动应用为主的，在尝试将BiliBili移植到桌面端或平板端的时候，我会用自己浅薄的知识去尝试进行解构并重组，使之符合桌面的审美及操作逻辑。

哔哩的页面分为桌面页面、平板页面和共享页面。顾名思义，这是根据不同的模式创建的UI页面。在哔哩的UI设计中，副页（Sub）是一个非常重要的模块，由于其独立性，在桌面和平板模式中都可以使用，所以共享页面的主体就是副页，这能为我节约很多时间。

同时由于API的限制（比如根据设备返回固定的视频条目），不是所有的模块都能完成转化，所以副页就成了沟通移动应用与桌面应用的桥梁。用户能在副页中找到移动界面操作的感觉，不会有很高的迁移成本。这算是我想出来的比较优雅的解决方案了。

在整体的应用界面设计上，桌面模式是一种相对经典的Master-Detail设计思路，即左侧导航，右侧显示详情。当然，应用根据实际情况进行了一些改动，丰富了左侧的导航界面，比如将分区集成在导航栏右上角，在导航栏中显示用户信息等。

平板模式的设计思路来源于XBOX新界面的游戏详情，以及以前的WIN8卷轴式设计。在不播放视频的时候会有很大的留白（出于实际设备机能的考虑，比如我的小surface go，无力承担大量图片的渲染），在滚动方向上基本都是以横向滚动为主。为了丰富视觉表现，在选择视频后，应用的背景会变成虚化的视频封面，以提供一种相对比较骚气的界面。

虽然在整体界面设计上我行我素，但对于细分控件则尽量向官方应用看齐。比如图标、动态卡片、评论等，这些控件的排版和移动界面上相差不大。这同样是为了保留一种B站的味道。

我的想法是，尽管是第三方应用，但总要让人能找到熟悉的感觉。红烧牛肉面如果换了紫色的包装，就算配料不变，那也“不是那个味儿”了；反之，只要保留了那熟悉的包装，即便logo不是康师傅，也会让人感觉“有那味儿了”。

## 代码说明

除了Warframe Alerting Prime，哔哩算是我写过的最大的软件了，单论结构复杂性，哔哩还犹有过之。

都是摸着石头过河，我的代码结构可能显得有些凌乱，注释也并不多，但总体结构如下：

- **BiliBili-Controls**：这里存放的是一些比较特殊的控件，要么是协作者开发的，要么是我从其他地方移植过来的，当时还考虑到后续的扩展性，还单独创建了一个类库（现在嘛……杞人忧天了）
- **BiliBili-Lib**：这里放着一些公共类、枚举定义以及哔哩哔哩的核心服务（在`Service`文件夹中）
- **BiliBili-Notification**：这是一个运行时组件，用于创建后台任务，进行动态的通知提醒。原本还打算做消息通知的，出了那档子事儿就没再做了
- **BiliBili-UWP**：应用主程序。主程序通过`AppViewModel`作为应用内的运行时状态管理器，`BiliViewModel`作为与BiliBili服务挂钩的处理模块来连接控件、改变状态以及管理数据。与UI有关的数据类、接口以及枚举定义等，我都放在了主项目中，而不是Lib类库里，主要还是为了调用方便以及分开来方便管理。应用创建了大量的自定义控件以及控件样式模板，这些被分别放在了`Components`和`Template`文件夹中，主题定义在`Theme`文件夹内。由于哔哩是一款中文应用，上面的视频基本都是中文的，所以不提供多语言配置

应用的核心播放器被命名为`VideoPlayer`，放在`Components -> Controls`文件夹中，魔改的MediaTransportControls在`Models -> UI -> Others`文件夹中，MTC的样式定义在`Template -> Media.xaml`资源字典内。

其它的基本没啥好说的了，项目文件比较多，结构也稍微复杂一些，全写到说明文件里不太现实。好在我写代码的时候喜欢用有意义的名字命名（尽管名字有时候会比较长），通过阅读代码，你应该也可以找到你想要的。

## API 定义

[Api.cs](https://github.com/Richasy/BiliBili-UWP/blob/master/BiliBili-Lib/Models/Others/Api.cs)

## 鸣谢

*(排名不分先后)*

- [蓝火火](https://github.com/cnbluefire)
- [Dino Chen](https://github.com/DinoChan)
- [逍遥橙子](https://github.com/xiaoyaocz)
- [NSDanmaku](https://github.com/xiaoyaocz/NSDanmaku)
- [SYEngine](https://github.com/xqq/SYEngine)
- [JustinXinLiu/Continuity](https://github.com/JustinXinLiu/Continuity)

