# Screeshot Tool Lite
一个轻量级的Windows截屏工具，支持多显示器环境，提供快捷键和悬浮按钮等多种截屏方式。


## 📸 功能特性


### 核心功能
- ✅ **智能截屏**：自动检测当前活跃显示器进行截图
- ✅ **多显示器支持**：完美支持多显示器环境，按钮跟随鼠标移动
- ✅ **快捷键触发**：支持 `Ctrl + Shift + S` 快捷键
- ✅ **悬浮按钮**：右下角悬浮按钮，点击即可截屏
- ✅ **自动保存**：截图自动保存到 `Screenshots` 文件夹
- ✅ **剪贴板复制**：截图同时复制到剪贴板，方便粘贴使用
- ✅ **系统托盘**：后台运行，支持托盘菜单操作

### 技术特性
- 🎯 **轻量级设计**：精简的代码结构，最小化资源占用
- 🖥️ **Windows Forms**：基于.NET 8.0 Windows Forms开发
- 🔧 **无依赖部署**：框架依赖部署，仅需200KB
- 🚀 **高性能**：快速响应，低延迟截屏
- 🛡️ **稳定可靠**：完善的错误处理和异常管理

## 🚀 快速开始

### 系统要求
- Windows 10 或更高版本
- .NET 8.0 （[下载地址](https://dotnet.microsoft.com/download/dotnet/8.0)）

### 安装步骤

#### 方法1：直接运行exe
1. 下载编译好的 [ScreenshotTool.exe](https://github.com/LainHE/screenshot_toollite/releases/download/v0.99/ScreenshotToolLite.exe)
2. 确保系统已安装.NET 8.0
3. 双击运行程序

#### 方法2：从源码编译

bash
1. 克隆项目
git clone <repository-url>

2. 进入项目目录
cd screenshot-tool

3. 编译项目
dotnet build

4. 发布（框架依赖部署）
dotnet publish -c Release -r win-x64 --self-contained false

5. 运行程序
dotnet run --project ScreenshotTool.csproj

### 使用方法

#### 启动程序
- 双击 `ScreenshotTool.exe` 启动
- 程序将在系统托盘运行，右下角显示悬浮按钮

#### 截屏方式

1. **快捷键截屏**
   - 按下 `Ctrl + Shift + S`
   - 程序自动截取当前活跃显示器内容

2. **按钮截屏**
   - 点击右下角悬浮按钮
   - 程序自动截取当前活跃显示器内容

3. **托盘菜单截屏**
   - 右键点击系统托盘图标
   - 选择"立即截屏"

4. **双击托盘图标**
   - 双击系统托盘图标
   - 程序自动截取当前活跃显示器内容

#### 截图保存
- **保存位置**：程序目录下的 `Screenshots` 文件夹
- **文件格式**：PNG格式
- **文件命名**：`屏幕截图_YYYYMMDD_HHMMSS.png`
- **剪贴板**：截图自动复制到剪贴板，可直接粘贴到其他应用


## 📁 项目结构

screenshot-tool/
├── build/ # 构建输出目录
│ └── bin/ # 编译生成的文件
├── Screenshots/ # 截图保存目录
├── Directory.Build.props # 构建配置文件
├── Program.cs # 程序入口点
├── ScreenCapturer.cs # 截图功能实现
├── TriggerButtonForm.cs # 主界面和触发逻辑
├── ScreenshotTool.csproj # 项目文件
└── README.md # 项目说明文档

## ⚙️ 配置说明

### 项目配置

xml
<!-- ScreenshotTool.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup>
<TargetFramework>net8.0-windows</TargetFramework>
<UseWindowsForms>true</UseWindowsForms>
<PublishSingleFile>true</PublishSingleFile>
<SelfContained>false</SelfContained>
</PropertyGroup>
</Project>

### 发布选项
- **框架依赖部署**：`--self-contained false`（文件大小：200KB）
- **自包含部署**：`--self-contained true`（文件大小：120MB）

## 🔧 开发说明

### 技术栈
- **开发语言**：C#
- **UI框架**：Windows Forms
- **目标框架**：.NET 8.0-windows
- **构建工具**：.NET CLI

### 核心类说明

#### ScreenCapturer.cs
- 负责屏幕截图功能
- 支持多显示器检测
- 自动保存和剪贴板操作

#### TriggerButtonForm.cs
- 主界面和交互逻辑
- 悬浮按钮管理
- 系统托盘集成
- 快捷键处理

#### Program.cs
- 程序入口点
- 应用程序启动配置

### 关键功能实现

#### 多显示器支持

csharp
// 获取当前鼠标所在的显示器
Screen currentScreen = Screen.FromPoint(Cursor.Position);

#### 悬浮按钮定位

csharp
// 定位到显示器右下角
var work = currentScreen.WorkingArea;
Location = new Point(work.Right - Width - 20, work.Bottom - Height - 20);

#### 截图保存

csharp
// 生成文件名
string fileName = $"屏幕截图_{DateTime.Now:yyyyMMdd_HHmmss}.png";
string filePath = Path.Combine(screenshotsPath, fileName);

## 🐛 故障排除

### 常见问题

#### Q: 程序无法启动
**A**: 确保系统已安装.NET 8.0，从[官方下载地址](https://dotnet.microsoft.com/download/dotnet/8.0)安装。

#### Q: 快捷键不生效
**A**: 检查系统快捷键冲突，或尝试重新启动程序。

#### Q: 悬浮按钮不显示
**A**: 确保程序正常运行，检查系统托盘图标是否显示。

#### Q: 截图功能失败
**A**: 检查显示器权限，确保程序有访问屏幕的权限。

#### Q: 文件过大
**A**: 使用框架依赖部署（`--self-contained false`）可将文件大小减至200KB。

### 日志和调试
- 程序通过系统托盘气泡提示操作结果
- 错误信息会显示在气泡提示中
- 可通过托盘菜单查看操作状态


## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。
