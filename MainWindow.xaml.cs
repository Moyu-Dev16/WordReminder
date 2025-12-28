using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WinForms = System.Windows.Forms; // 别名，避免冲突
using Microsoft.Win32; // 用于注册表操作

namespace WordReminder
{
    public partial class MainWindow : Window
    {
        private List<WordItem> _wordList;
        private WordItem _currentWord;
        private MediaPlayer _player = new MediaPlayer();
        private WinForms.NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            InitTrayIcon(); // 初始化托盘
            SetAutoStart(); // 设置开机自启
        }

        // 窗口加载时
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. 设置窗口位置到屏幕右上角
            var desktopWidth = SystemParameters.WorkArea.Width;
            this.Left = desktopWidth - this.Width - 20; // 离右边20像素
            this.Top = 20; // 离顶部20像素

            // 2. 加载数据
            LoadData();
        }

        // 支持拖动窗口
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // 加载 JSON 数据
        private void LoadData()
        {
            try
            {
                // 假设 json 文件名为 words.json，放在程序运行目录下
                // 你需要手动在项目里创建一个 words.json 并设为“如果较新则复制”
                string jsonPath = "./data/words.json";
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    var root = JsonConvert.DeserializeObject<WordRoot>(json);
                    _wordList = root.Data;

                    ShowDailyWord(false);
                }
                else
                {
                    TxtWord.Text = "错误";
                    TxtDefinition.Text = "未找到 words.json 文件";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据加载失败: " + ex.Message);
            }
        }

        int temp = 0;
        int temp2 = 0;

        private string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        // 显示单词逻辑 (这里演示随机，你可以改成根据日期 hash 算法来每天固定)
        private void ShowDailyWord(bool next)
        {
            if (_wordList == null || _wordList.Count == 0) return;

            // 获取今天应该显示的单词下标
            int index = GetOrUpdateDailyIndex();

            // 防止下标越界（比如词库只有100个，算出来是105，就要取余数回到第5个）
            // 这一步非常重要，实现了“循环背单词”
            int safeIndex = index % _wordList.Count;
            if (next) 
            {
                temp = safeIndex;

                temp2 += 1;
                safeIndex = temp2+ safeIndex;
            }

            _currentWord = _wordList[safeIndex];
            UpdateUI();
        }
        private int GetOrUpdateDailyIndex()
        {
            try
            {
                DateTime today = DateTime.Now.Date; // 只取日期，不要时分秒

                // 1. 如果文件不存在，创建默认文件
                if (!File.Exists(_settingsPath))
                {
                    var defaultSettings = new AppSettings { Time = today, Index = 0 };
                    SaveSettings(defaultSettings);
                    return 0; // 第一天，返回 0
                }

                // 2. 如果文件存在，读取它
                string json = File.ReadAllText(_settingsPath);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);

                // 如果读取失败（比如文件是空的），重置
                if (settings == null)
                {
                    settings = new AppSettings { Time = today, Index = 0 };
                }

                // 3. 对比日期
                if (today > settings.Time)
                {
                    // 计算相差几天
                    TimeSpan diff = today - settings.Time;
                    int daysPassed = (int)diff.TotalDays;

                    if (daysPassed > 0)
                    {
                        // 更新 Index：旧索引 + 过去的天数
                        // 这样即使你 3 天没开电脑，今天的单词也会按顺序跳过中间那两天
                        settings.Index += daysPassed;
                        settings.Time = today; // 更新为今天的日期

                        // 保存更改
                        SaveSettings(settings);
                    }
                }
                else if (today < settings.Time)
                {
                    // 防御性编程：如果用户把系统时间往回拨了，或者文件时间是未来的
                    // 我们通常不做处理，直接返回记录的 index，或者你可以选择重置
                }

                return settings.Index;
            }
            catch (Exception ex)
            {
                // 出错兜底：返回0
                System.Diagnostics.Debug.WriteLine("读取设置出错: " + ex.Message);
                return 0;
            }
        }
        // 辅助方法：保存 JSON
        private void SaveSettings(AppSettings settings)
        {
            string output = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_settingsPath, output);
        }

        private void UpdateUI()
        {
            TxtWord.Text = _currentWord.Word;
            TxtPhonetic.Text = $"/{_currentWord.UsPhonetic}/";
            TxtDefinition.Text = _currentWord.SimpleDefinition;
        }

        // 播放声音
        private void BtnPlayAudio_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWord == null) return;
            try
            {
                string url = $"http://dict.youdao.com/dictvoice?audio={_currentWord.Word}";
                _player.Open(new Uri(url));
                _player.Play();
            }
            catch
            {
                // 忽略网络错误
            }
        }

        // 点击关闭按钮 -> 最小化到托盘
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // 隐藏窗口而不是关闭程序
        }

        // --- 系统托盘逻辑 ---
        private void InitTrayIcon()
        {
            _notifyIcon = new WinForms.NotifyIcon();

            // --- 修改代码开始 ---

            // 以前是读文件路径，现在直接从 Properties.Resources 读取
            // 这里的 .icon 是你在资源管理器里看到的名字，如果你的文件名叫 mylogo.ico，这里就是 .mylogo
            _notifyIcon.Icon = Properties.Resources.icon1;

            // --- 修改代码结束 ---

            _notifyIcon.Visible = true;
            _notifyIcon.Text = "背单词助手";

            // 双击显示
            _notifyIcon.DoubleClick += (s, args) => {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Activate();
            };

            // 右键菜单
            var contextMenu = new WinForms.ContextMenuStrip();
            contextMenu.Items.Add("显示单词", null, (s, args) => { this.Show(); });
            contextMenu.Items.Add("下一个", null, (s, args) => { ShowDailyWord(true); }); // 方便测试换词
            contextMenu.Items.Add("-"); // 分隔线
            contextMenu.Items.Add("退出", null, (s, args) => {
                _notifyIcon.Dispose();
                Application.Current.Shutdown();
            });
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        // --- 开机自启逻辑 ---
        private void SetAutoStart()
        {
            try
            {
                string execPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string name = "MyWordReminder";
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (rk.GetValue(name) == null)
                {
                    rk.SetValue(name, execPath);
                }
            }
            catch (Exception)
            {
                // 权限不足可能导致失败，实际发布时最好做个设置选项让用户选
            }
        }
    }
    public class AppSettings
    {
        public DateTime Time { get; set; }
        public int Index { get; set; }
    }
}