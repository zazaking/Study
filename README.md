# 更新日志
- 2021/07/25：
    - 创建该库用于学习项目
- 2021/07/26：
    - 创建暑期学习笔记文件
- 2021/07/28：
    - 创建Unity_Proect
    - 上传Unity贪吃蛇的练习项目
- 2021/07/29：
    - Update Unity Project "Demo_Snake_1"
    - Update Summer_Plan.md
- 2021/08/06：
    - Update Unity Project "Demo_Snake_1"
    - Update Summer_Plan.md
	- Add STL_Model
- 2021/08/09：
    - Add New Project "Study\Unity_Project\STL_Im&Exporter"
    - Update Summer_Plan.md
- 2021/08/13：
    - 删除之前的项目"Demo_Snake_1"，已废弃
    - 项目"STL_Im&Exporter"中重写了线框显示功能，实现了截图
- 2021/08/17：
    - 更名"STL_Im&Exporter"项目为"STL_ImExporter",为了解决FFMPEG无法找到绝对路径的问题
    - 添加游戏内录屏功能，使用FFMPEG实现。
- 2021/08/20：
    - 重写视屏录制功能，改用自定义格式+stl模型
- 2021/08/27：
    - 完成包围盒（球型）功能
    - 完成HTML文档导出功能
# 已知问题
- STL_ImExporter
    - 线框模式旋转有问题，可能是轴心问题，移动计算也有问题
    - 还未完成截图转化为视频 功能
    - 删除临时文件（录频中间的截图文件）时概率在游戏结束后报文件丢失。
    - HTML文档导出生成图片时，由于精度像素问题，容易出现画线缺失
