using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        /// <summary>
        ///     Instantiates the clock, and calls Setup to start the timers and other needed methods to initialize the clock properly.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }

        /// <summary>
        ///     The process object for this instance.
        /// </summary>
        private readonly Process clock = Process.GetCurrentProcess();

        /// <summary>
        ///     The settings window that will be activated when the user clicks the notifier icon.
        /// </summary>
        static SettingsAndExit _settingsWindow = new SettingsAndExit();

        /// <summary>
        ///     Opens the settings menu when the user left clicks the notifier icon in the task tray.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private static void NotifierIconOnTrayLeftMouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_settingsWindow.IsVisible)
            {
                return;
            }
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsAndExit();
            }
            _settingsWindow.Activate();
            _settingsWindow.ShowDialog();
        }

        /// <summary>
        ///     The notification icon used to relay information to the user.
        /// </summary>
        public static TaskbarIcon NotifierIcon;

        /// <summary>
        ///     All processes named League of Legends currently running.
        /// </summary>
        private static Process[] LeagueProcesses => Process.GetProcessesByName("League Of Legends");

        /// <summary>
        ///     The window handle of League of Legends.
        /// </summary>
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

        /// <summary>
        ///     The window handle of this instance's process.
        /// </summary>
        private IntPtr ClockWindowHandle => clock.MainWindowHandle;

        /// <summary>
        ///     The window handle of the current foreground window.
        /// </summary>
        private static IntPtr ForegroundWindowHandle => NativeMethods.GetForegroundWindow();

        /// <summary>
        ///     The Rect of the League of Legends process window, Left, Top, Right, Bottom values.
        /// </summary>
        public NativeMethods.Rect LeagueWindow
        {
            get
            {
                var leagueWindow = new NativeMethods.Rect();
                NativeMethods.GetWindowRect(LeagueWindowHandle, ref leagueWindow);
                return leagueWindow;
            }
        }

        /// <summary>
        ///     Gets all of the timers set up to update the clock and does other checks to get everything running properly.
        /// </summary>
        public void Setup()
        {
            NotifierIcon = new TaskbarIcon { Icon = Properties.Resources.ClockIcon };
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

        /// <summary>
        ///     True if this instance of the process is the only one running.
        /// </summary>
        /// <returns>Returns false there are any other running ClockOverlay processes that were unable to be closed.</returns>
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

        /// <summary>
        ///     True if League of Legends is the foreground window.
        /// </summary>
        /// <returns></returns>
        public bool LeagueIsActiveWindow()
        {
            return ForegroundWindowHandle == LeagueWindowHandle;
        }

        /// <summary>
        ///     True if the clock is the foreground window.
        /// </summary>
        /// <returns></returns>
        public bool ClockIsActiveWindow()
        {
            return ForegroundWindowHandle == ClockWindowHandle;
        }

        /// <summary>
        ///     Updates the visibility of the clock. 
        /// </summary>
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

        /// <summary>
        ///     Updates the clock's time and visibility.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">args</param>
        private void UpdateTimerTick(object sender, EventArgs e)
        {
            Update();
            _textTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }
            
        /// <summary>
        ///     Reads the settings file and updates the clocks position and color values if they have changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">args.</param>
        private void SettingsTimerTick(object sender, EventArgs e)
        {
            var updatedSettings = Settings.ReadSettings();

            foreach (var item in updatedSettings)
            {
                switch (item.Key)
                {
                    // The top offset setting.
                    case "top":
                        if (item.Value != Top.ToString(CultureInfo.InvariantCulture))
                        {
                            UpdatePosition(Convert.ToInt32(item.Value), (int)Left);
                        }
                        break;
                    // The left offset setting.
                    case "left":
                        if ( item.Value != Left.ToString(CultureInfo.InvariantCulture))
                        {
                            UpdatePosition((int)Top, Convert.ToInt32(item.Value));
                        }
                        break;
                    // The color setting.
                    case "color":
                        var convertFromString = ColorConverter.ConvertFromString(item.Value);
                        if (convertFromString != null)
                        {
                            _textTime.Foreground = new SolidColorBrush((Color)convertFromString);
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///     Sets the clocks position to the values given to the parameters.
        /// </summary>
        /// <param name="top">Top offset, from 0.</param>
        /// <param name="left">Left offset, from 0.</param>
        public void UpdatePosition(int top, int left)
        {
            Top = LeagueWindow.Top + top;
            Left = LeagueWindow.Left + left;
            Console.WriteLine(top + " " + left + " "  +ForegroundWindowHandle) ;
        }

        /// <summary>
        ///     This allows the clock to not accept mouse clicks and gives the effect that the clock is integrated within the game.
        /// </summary>
        /// <param name="e">args.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.MakeTransparent(hwnd);
        }
    }
}