# 📚 WordReminder - 桌面背单词小助手

> 一个基于 .NET Framework (WPF) 开发的极简桌面背单词工具，利用碎片化时间每天掌握一个英语单词。

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Language](https://img.shields.io/badge/Language-C%23%20%2F%20WPF-purple)
![License](https://img.shields.io/badge/License-MIT-orange)

## 📖 1. 项目简介

**WordReminder** 是一款轻量级的 Windows 桌面应用程序。它的设计初衷是让“背单词”这件事变得无压力且自然。程序运行后会在桌面右上角显示一个半透明的悬浮窗，每天自动更新一个单词，支持发音播放和系统托盘最小化。

### ✨ 功能特性

- **📅 每日一词**：智能算法控制，每天自动更新一个新单词，重启软件不丢失进度。
- **👻 极简悬浮窗**：无边框、半透明设计，默认置顶于桌面右上角，支持鼠标拖拽移动。
- **🔊 纯正发音**：集成在线发音功能（调用有道词典接口），点击喇叭图标即可朗读。
- **💼 系统托盘集成**：点击关闭按钮自动最小化到托盘，右键菜单支持快速退出，不占用任务栏空间。
- **🚀 开机自启**：支持写入注册表实现开机自动运行。

## 🛠️ 2. 技术架构与目录

- **开发框架**: .NET Framework 4.7.2+ (WPF)
- **依赖库**: `Newtonsoft.Json` (用于 JSON 数据解析)
- **开发工具**: Visual Studio 2022

### 📂 目录结构

```text
WordReminder/
├── WordReminder/
│   ├── Properties/
│   │   └── Resources.resx   # 资源文件（包含 icon.ico）
│   ├── AppSettings.cs       # [代码] 设置类模型
│   ├── MainWindow.xaml      # [代码] 主界面布局
│   ├── MainWindow.xaml.cs   # [代码] 核心业务逻辑
│   ├── WordModel.cs         # [代码] 单词数据模型
│   ├── words.json           # [数据] 核心词库
│   └── ...
├── 单词.py                  # [工具] 数据爬虫脚本 (Python)
├── WordReminder.sln         # 解决方案文件
└── README.md                # 说明文档

### 🕷️ 数据来源与爬虫说明


本项目核心词库数据来源于 单词森林。

数据源地址: https://wordforest.cn/

爬虫脚本: 项目根目录下包含一个名为 单词.py 的 Python 脚本。

功能: 该脚本用于从上述网站抓取单词数据，并清洗格式化为项目所需的 words.json 结构。

使用: 如果你需要自行扩充或更新词库，请确保安装 Python 环境及 requests 库，运行该脚本即可生成新的数据文件。

⚠️ 声明: 本项目及爬虫脚本仅供个人学习与技术交流使用，数据版权归原网站所有，请勿用于商业用途。

