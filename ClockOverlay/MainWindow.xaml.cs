using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
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
        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }

        private readonly Process clock = Process.GetCurrentProcess();

        static SettingsAndExit SettingsWindow = new SettingsAndExit();

        private static void NotifierIconOnTrayLeftMouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            if (SettingsWindow.IsVisible)
            {
                return;
            }
            if (SettingsWindow == null)
            {
                SettingsWindow = new SettingsAndExit();
            }
            SettingsWindow.Activate();
            SettingsWindow.ShowDialog();
        }

        public static TaskbarIcon NotifierIcon;

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

        public void Setup()
        {
            NotifierIcon = new TaskbarIcon { };
            NotifierIcon.TrayLeftMouseDown += NotifierIconOnTrayLeftMouseDown;         

            var timerRefresher = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timerRefresher.Tick += UpdateTimerTick;
            timerRefresher.Start();

            var timerSettingsCheck = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timerSettingsCheck.Tick += SettingsTimerTick;
            timerSettingsCheck.Start();

            if (!ThereCanOnlyBeOne())
            {
                Console.WriteLine(@"I am not the one.");
                Environment.Exit(187);
            }

            Visibility = Visibility.Hidden;

            if (!System.IO.File.Exists("settings.xml"))
            {
                var defaultSettings = new Dictionary<string, string>
                {
                    { "left", "90" },
                    { "top", "50" },
                    { "color", "#FFAC00FF" }
                };

                Settings.WriteSettings(defaultSettings);
            }

            var convertFromString = ColorConverter.ConvertFromString(Settings.TextColor);
            if (convertFromString != null)
            {
                _textTime.Foreground = new SolidColorBrush((Color)convertFromString);
            }
            Top = Settings.TopOffset;
            Left = Settings.LeftOffset;
        }

        public bool ThereCanOnlyBeOne()
        {
            var me = Process.GetCurrentProcess();
            var wannaBeMes = Process.GetProcessesByName("ClockOverlay");

            if (wannaBeMes.Length <= 1)
            {
                return true;
            }

            foreach (var wannaBe in wannaBeMes.Where(wannaBe => wannaBe.Id != me.Id))
            {
                wannaBe.Kill();
            }

            return wannaBeMes.Length == 1;
        }

        public bool LeagueIsActiveWindow()
        {
            return ForegroundWindowHandle == LeagueWindowHandle;
        }

        public bool ClockIsActiveWindow()
        {
            return ForegroundWindowHandle == ClockWindowHandle;
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
        }

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            Update();
            _textTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }
            
        private void SettingsTimerTick(object sender, EventArgs e)
        {
            var updated = Settings.ReadSettings();
            foreach (var item in updated)
            {
                switch (item.Key)
                {
                    case "top":
                        if (item.Value != Top.ToString())
                        {
                            UpdatePosition(Convert.ToInt32(item.Value), (int)Left);
                        }
                        break;
                    case "left":
                        if ( item.Value != Left.ToString())
                        {
                            UpdatePosition((int)Top, Convert.ToInt32(item.Value));
                        }
                        break;
                    case "color":
                        Console.WriteLine("color updated.");
                        Console.WriteLine(_textTime.Foreground.ToString());
                        Console.WriteLine(item.Value);
                        _textTime.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(item.Value));
                        break;
                }
                
            }
            Console.WriteLine(@"updated.");
        }

        public void UpdatePosition(int top, int left)
        {
            Top = top;
            Left = left;

            Console.WriteLine("moved.");
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.MakeTransparent(hwnd);
        }
    }
}