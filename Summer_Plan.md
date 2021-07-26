<div align='center' ><font size='70'>暑期任务安排</font></div>
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
### 1.2.2 Git工作区、暂存区和版本库
- 工作区：就是你在电脑里能看到的目录。
- 暂存区：英文叫 stage 或 index。一般存放在 ``.git ``目录下的 index 文件（.git/index）中，所以我们把暂存区有时也叫作索引（index）。
- 版本库：工作区有一个隐藏目录 ``.git``，这个不算工作区，而是 Git 的版本库。、
### 1.2.3  配置用户名和密码
```bash
#配置用户名和密码：
git config user.name "Your Name"  
git config user.email "email@example.com"
#查看当前用户信息： 
git config --list
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
git add . #别忘记后面的.，此操作是把文件夹下面的文件(更新+新增)都添加进来
git commit -m "提交信息" #“提交信息”里面换成你需要，如“first commit”
git status #查看状态
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
### 1.2.7 Git常用操作
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
- [ ] 2.1 具备视角转换功能，如俯视图、正视图等
- [ ] 2.2 具备视图转换功能，如渲染视图、线框视图等（8/1完成）
- [ ] 2.3 具备导入/导出STL和STEP模型的功能（8/8完成）
- [ ] 2.4 具备鼠标交互的功能，如旋转、平移、缩放等（8/15完成）
# 3. 基于上述引擎，构建一个包含多物体的几何模型，并用几何模型展示其工作过程
- [ ] 3.1 从不同视角展示所建立的三维模型，并生成视频文件
- [ ] 3.2 展现关键部件的运动过程， 并生成视频文件（8/22完成）
# 4. 显示模型概述信息，并生成图文报告
- [ ] 4.1 显示几何信息和质量，如包围盒、体积，质量等
- [ ] 4.2 生成图文报告（HTML或Word均可），包括图片格式的三维模型和表格形式的模型信息（8/29完成）
