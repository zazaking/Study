<div align='center' ><font size='70'>暑期任务</font></div>
<div align='right' >by 郑泽灏</div>

# 1. Git环境配置
## 1.1 目标
- [x] 熟悉git操作
- [x] 熟悉markdown文档编写
- [x] 在Github建立账户，并提交代码（7/28完成）

## 1.2 学习记录
### 1.2.1 我所创建的GitHub地址
- Git UserName：``zazaking``

- Git主页：<https://github.com/zazaking>

- 本次暑期任务地址：``git@github.com:zazaking/Study.git``

- 以前项目地址：``git@github.com:zazaking/QRCodeDuplicateCheck.git``

### 1.2.2 Git的工作区域
- 工作目录（Working Directory）：
电脑中存放平时项目的地方。
- 暂存区（Stage/Index）：
用于临时存放你的改动，事实上他只是一个文件，保存即将提交到文件列表信息。
- 资源库（Repository/Git Directory）：
安全存放数据的位置“.git文件夹”，这里面有你提交到所有版本的数据。其中HEAD文件职想最新放入仓库的版本。
- Remote：
远程仓库，GitHub

### 1.2.3  Git必要配置
```bash
#查看系统配置信息：
git config --system -l #配置信息存放在 [ Git安装目录\etc\gitconfig ] 中
#查看用户配置
git config --global -l #配置信息存放在 [ C:\Users\用户名\gitconfig ] 中
#查看仓库配置
git config --local -l

#配置用户名和密码：
git config --global user.name "Your Name"  
git config --global user.email "email@example.com"
```
### 1.2.4 GitHub下载项目到本地
```bash
git init #把本地的目录变成git本地仓库（本地创建.git可管理的仓库）
git remote add origin + url #添加远程仓库
git remote -v #查看目录下到得所有项目
git clone + url #下载代码到本地,这里``git pull + url``也可行
```
### 1.2.5 提交本地代码到GitHub
```bash
git add . #添加所有文件(更新+新增)到暂存区
git commit -m "提交信息" #提交暂存区文件到本地仓库 -m 提交信息，如“first commit”
git status #查看状态（可不做）
git push + url #此操作目的是把本地仓库push到github上面
```
### 1.2.6 git分支从master切换到main
```bash
git checkout -b main
# Switched to a new branch 'main'
git branch
# * main
#  master
git merge master # 将master分支合并到main上
# Already up to date.
git pull origin main --allow-unrelated-histories # git pull origin main会报错：refusing to merge unrelated histories
git push origin main
```
### 1.2.7 忽略文件
在主目录下建立".gitignore"文件，此文件规则如下：
- 以斜杠/开头表示目录；
- 以星号*通配多个字符；
- 以问号?通配单个字符
- 以方括号[]包含单个字符的匹配列表；
- 以叹号!表示不忽略(跟踪)匹配到的文件或目录；

### 1.2.8 Git常用操作
```bash
#创建仓库命令：
git init：#初始化仓库
git clone：#拷贝一份远程仓库，也就是下载一个项目
#提交与修改：
git add：#添加文件到缓冲区
git status：#查看仓库当前状态，显示有变更的文件
git diff：#比较文件的不同，即缓存区和工作区的差异
git commit：#将缓冲区内容添加到仓库中
git reset：#回退版本
git rm：#删除工作区文件
git mv：#移动或重命名工作区文件
#提交日志：
git log：#查看历史提交记录
git blame <file>：#以列表形式查看指定文件的修改记录
#远程操作：
git remote：#远程仓库操作
git fetch：#从远程获取代码库
git pull：#下载远程代码并合并
git push：#上传远程代码并合并
```
***
# 2. 使用c++/C#平台搭建三维几何模型的显示与交互引擎，可使用OSG或Unity等成熟产品
## 1.1 目标
- [x] 2.1 具备视角转换功能，如俯视图、正视图等
- [x] 2.2 具备视图转换功能，如渲染视图、线框视图等（8/1完成）
- [x] 2.3 具备导入/导出STL和STEP模型的功能（8/8完成）
- [x] 2.4 具备鼠标交互的功能，如旋转、平移、缩放等（8/15完成）
## 1.2 学习记录
### 1.2.1 Unity安装插件proBulider、progrids
- 安装插件proBulider：
    直接在Windows\Package Manager中添加，然后在Tools中打开。
- 安装插件progrids：
    在“项目名\Packages\manifest.json“中粘贴"com.unity.progrids":"3.0.3-preview.4"，然后切换至Unity，在Tools中打开。
### 1.2.2 编写Unity3D项目“Demo_Sanke_1”，并上传至GitHub
- 场景一为学习的贪吃蛇游戏，通过C#脚本实现贪吃蛇游戏基本逻辑，通过C#脚本实现相机位置的变换，具备视角转换功能，如俯视图、正视图。

    ![image](https://i.ibb.co/vLDR85f/9-C753726-A94645-B89847-D2582326-D57-E.jpg)

    ![image](https://i.ibb.co/6tV34zz/BE372295-FA094-F0-C996480-C587-DEEC74.jpg)
    
    ![image](https://i.ibb.co/wgTbp1R/47-B7-B4977-CD54177-BF8-B911892-FB422-A.jpg)

- 场景二为简单矩形，在Asset Store上下载纹理，实现矩形Wire Frame的显示。
  
    ![image](https://i.ibb.co/ssrys2g/H8-CC7-ASGD4-4-R181-U61-CK3-T.png)

- 场景一、二可以通过按钮进行切换。

    ![image](https://media.giphy.com/media/ymrsVTzONTFUVxeF1v/giphy.gif)

### 1.2.3 在Unity3D项目“Demo_Sanke_1”的“Scene3”场景中增加按钮实现外部STL模型的导入，导出功能。
- STL模型导入：

    ![image](https://i.ibb.co/34GRPbk/F0-DB58-D0-FB554-E179-D6628030-EA3-EFFB.jpg)

    ![image](https://i.ibb.co/SxFzYpx/D8-DDD5-E596-D44-A7-B87-B1-A96066310176.jpg)

    ![image](https://i.ibb.co/GV6WQZH/YGI-F-F-N2-PVU-6-U7-70.png)

- STL模型导出：
    
    ![image](https://i.ibb.co/444FYcT/72-FABEE921-FC4-A29-A426-C8-D387675-D18.jpg)

    ![image](https://i.ibb.co/3BxHdxd/627273-E856-FF4-E4-BA1-AFD3-FBD623-F6-C2.jpg)

    ![image](https://i.ibb.co/GJ9Wwxj/8-FE93643303-E4-AB4937-EADA1961-E51-D0.jpg)
- STL模型导入、导出已重写：
    新项目位置为："Study\Unity_Project\STL_Im&Exporter"

### 1.2.4 并实现导入模型动态添加运动控制脚本，实现缩放、鼠标跟随功能。
- 实现滚轮缩放功能
- 实现鼠标跟随功能，方向始终跟随，点击左键运动跟随

    ![image](https://media.giphy.com/media/UsGHTnYvffgOSlVyZZ/giphy.gif)

### 1.2.5 2021/08/13重写了线框（WireFrame）模式
- 重写了线框模式，改为提取对象Mesh信息再用OpenGL划线并设置对象Skybox透明的方式，担由于运动控制每次都需要提取点，可能性能较低：

![image](https://media.giphy.com/media/w30rdTs0xRqLaXV5ou/giphy.gif)

- 重写了旋转，改为鼠标欢动旋转，新线框模式支持：

![image](https://media.giphy.com/media/5yREawGn4voAeMyFa8/giphy.gif)


# 3. 基于上述引擎，构建一个包含多物体的几何模型，并用几何模型展示其工作过程
- [ ] 3.1 从不同视角展示所建立的三维模型，并生成视频文件
- [ ] 3.2 展现关键部件的运动过程， 并生成视频文件（8/22完成）
# 4. 显示模型概述信息，并生成图文报告
- [ ] 4.1 显示几何信息和质量，如包围盒、体积，质量等
- [ ] 4.2 生成图文报告（HTML或Word均可），包括图片格式的三维模型和表格形式的模型信息（8/29完成）