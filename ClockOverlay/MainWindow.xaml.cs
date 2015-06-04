using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;

namespace ClockOverlay
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Process clock = Process.GetCurrentProcess();

        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }

        private static Process[] LeagueProcesses => Process.GetProcessesByName("League Of Legends");

        private static IntPtr LeagueWindowHandle
        {
            get
            {
                if (LeagueProcesses.Length <= 0)
                {
                    return (IntPtr) 0;
                }
                return LeagueProcesses[0].MainWindowHandle;
            }
        }

        private IntPtr ClockWindowHandle => clock.MainWindowHandle;

        private static IntPtr ForegroundWindowHandle => NativeMethods.GetForegroundWindow();

        public NativeMethods.Rect LeagueWindow
        {
            get
            {
                var leagueWindow = new NativeMethods.Rect();
                NativeMethods.GetWindowRect(LeagueWindowHandle, ref leagueWindow);
                return leagueWindow;
            }
        }

        public TaskbarIcon Tb { get; set; }

        public void Setup()
        { 
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += TimerTick;
            timer.Start();

            Visibility = Visibility.Hidden;

            var defaultSettings = new Dictionary<string, string>
            {
                { "xOffset", "90" },
                { "yOffset", "50" },
                { "TextColor", "#FFFFFFFF" },
                { "TextEffects", "false" }
            };

            if (!System.IO.File.Exists("settings.xml"))
            {
                Settings.WriteSettings(defaultSettings);
            }
            var a = Settings.ReadSettings();
            if (a == null)
            {
                return;
            }
            _textTime.Foreground = new SolidColorBrush(Settings.TextColor);
            Top = Settings.TopOffset;
            Left = Settings.LeftOffset;
        }

        public bool LeagueIsActiveWindow()
        {
            if (ForegroundWindowHandle == LeagueWindowHandle)
            {
                return true;
            }

            return false;
        }

        public bool ClockIsActiveWindow()
        {
            if (ForegroundWindowHandle == ClockWindowHandle)
            {
                return true;
            }
            return false;
        }

        public void Update()
        {
            // if this is the active window then do nothing.
            if (ClockIsActiveWindow())
            {
                return;
            }
            // if league is not the active window then hide this.
            if (!LeagueIsActiveWindow())
            {
                Visibility = Visibility.Hidden;
                return;
            }

            Visibility = Visibility.Visible;
            UpdatePosition(this);
            _textTime.Foreground = new SolidColorBrush(Settings.TextColor);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Update();
            _textTime.Text = DateTime.Now.ToString("hh:mm tt");
        }

        public static void UpdatePosition(MainWindow window)
        {
            Settings.ReadSettings();
            window.Top = Settings.TopOffset;
            window.Left = Settings.LeftOffset;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.makeTransparent(hwnd);
        }
    }
}