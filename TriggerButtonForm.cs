using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Timers;

namespace ScreenshotTool
{
    internal sealed class TriggerButtonForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int VK_S = 0x53;
        private const int WS_EX_TOPMOST = 0x00000008;

        private readonly ScreenCapturer _capturer = new ScreenCapturer();
        private readonly NotifyIcon _tray;
        private Screen _currentScreen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
        private System.Timers.Timer _monitorTimer;

        public TriggerButtonForm()
        {
            // 确保窗口总是置顶
            SetTopLevel(true);

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Size = new Size(84, 84);
            BackColor = Color.FromArgb(30, 144, 255);

            var button = new Button
            {
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 144, 255),
                ForeColor = Color.White,
                Text = "截屏",
                Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (s, e) => TriggerCapture();
            Controls.Add(button);

            _tray = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "截屏工具"
            };
            var menu = new ContextMenuStrip();
            menu.Items.Add("立即截屏", null, (s, e) => TriggerCapture());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("退出", null, (s, e) => Application.Exit());
            _tray.ContextMenuStrip = menu;
            _tray.DoubleClick += (s, e) => TriggerCapture();

            // 添加事件监听
            this.MouseMove += TriggerButtonForm_MouseMove;
            this.Deactivate += TriggerButtonForm_Deactivate;
            this.VisibleChanged += TriggerButtonForm_VisibleChanged;
            this.Activated += TriggerButtonForm_Activated;

            // 创建定时器定期检查显示器变化
            _monitorTimer = new System.Timers.Timer(500); // 每500ms检查一次
            _monitorTimer.Elapsed += MonitorTimer_Elapsed;
            _monitorTimer.Enabled = true;

            PositionInCorner();
        }

        // 确保窗口总是置顶
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOPMOST;
                return cp;
            }
        }

        private new void SetTopLevel(bool top)
        {
            if (top)
            {
                // 设置窗口为总是置顶
                this.TopMost = true;
                // 确保窗口在最前面
                this.Activate();
                this.BringToFront();
            }
        }

        private void TriggerButtonForm_MouseMove(object? sender, MouseEventArgs? e)
        {
            CheckAndMoveScreen();
        }

        private void TriggerButtonForm_Deactivate(object? sender, EventArgs? e)
        {
            // 当窗口失去焦点时，确保它仍然在最前面
            this.Activate();
            this.BringToFront();
            CheckAndMoveScreen();
        }

        private void TriggerButtonForm_VisibleChanged(object? sender, EventArgs? e)
        {
            // 当窗口可见性变化时，检查显示器位置
            if (this.Visible)
            {
                CheckAndMoveScreen();
            }
        }

        private void TriggerButtonForm_Activated(object? sender, EventArgs? e)
        {
            // 当窗口激活时，检查显示器位置
            CheckAndMoveScreen();
        }

        private void MonitorTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // 定时器定期检查显示器变化
            if (this.Visible)
            {
                CheckAndMoveScreen();
            }
        }

        private void CheckAndMoveScreen()
        {
            try
            {
                Screen currentScreen = Screen.FromPoint(Cursor.Position);
                if (currentScreen != _currentScreen)
                {
                    _currentScreen = currentScreen;
                    PositionInCorner();
                }
            }
            catch
            {
                // 忽略异常，避免崩溃
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (RegisterHotKey(Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_S))
                _tray.ShowBalloonTip(1000, "截屏工具",
                    "已就绪：快捷键 Ctrl+Shift+S，或点击右下角按钮。", ToolTipIcon.Info);
            else
                _tray.ShowBalloonTip(1500, "截屏工具",
                    "快捷键注册失败，请使用按钮或托盘菜单。", ToolTipIcon.Warning);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 停止定时器
            _monitorTimer?.Stop();
            _monitorTimer?.Dispose();

            UnregisterHotKey(Handle, HOTKEY_ID);
            _tray.Visible = false;
            _tray.Dispose();
            base.OnFormClosing(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
                TriggerCapture();
        }

        private void TriggerCapture()
        {
            // 确保窗口在截图前完全隐藏
            this.Hide();
            Application.DoEvents();
            Thread.Sleep(120);

            string? file = _capturer.CaptureActiveScreen();

            // 重新显示并定位到当前显示器
            CheckAndMoveScreen();
            this.Show();
            this.Activate();
            this.BringToFront();

            if (file != null)
                _tray.ShowBalloonTip(1000, "截屏完成",
                    "已保存并复制到剪贴板：\n" + file, ToolTipIcon.Info);
            else
                _tray.ShowBalloonTip(1500, "截屏失败", "截图过程中发生错误。", ToolTipIcon.Error);
        }

        private void PositionInCorner()
        {
            // 根据当前鼠标所在的显示器定位
            _currentScreen = Screen.FromPoint(Cursor.Position);
            var work = _currentScreen.WorkingArea;
            Location = new Point(work.Right - Width - 20, work.Bottom - Height - 20);
        }
    }
}
