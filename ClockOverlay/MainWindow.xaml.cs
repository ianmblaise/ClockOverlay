using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

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
            StartRunning();
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

        public void StartRunning()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += TimerTick;
            timer.Start();
            Visibility = Visibility.Hidden;
            var settingsDictionary = new Dictionary<string, string>
            {
                { "xOffset", "90" },
                { "yOffset", "50" },
                { "Color", "#FFFFFFFF" },
                { "TextEffects", "true" }
            };

            Settings.WriteSettings(settingsDictionary);
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
                Visibility = Visibility.Visible;
                return;
            }
            // if league is not the active window and this is topmost then hide this and set topmost to false. 
            if (!LeagueIsActiveWindow())
            {
                Visibility = Visibility.Hidden;
            }
            // if league is active window and this isn't topmost then make it topmost, show it, and update it's position. 
            if (LeagueIsActiveWindow())
            {
                if (Visibility != Visibility.Visible)
                {
                    Visibility = Visibility.Visible;
                }
                UpdatePosition(this, LeagueWindow.Top, LeagueWindow.Right);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Update();
            _textTime.Text = DateTime.Now.ToString("hh:mm");
        }

        public static void UpdatePosition(MainWindow window, int top, int left)
        {
            window.Top = top + 65;
            window.Left = left - 90;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.makeTransparent(hwnd);
        }
    }
}