using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using ClockOverlay.Properties;
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
        private readonly Process _clock = Process.GetCurrentProcess();

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
            if (_settingsWindow == null || _settingsWindow.IsLoaded == false)
            {
                _settingsWindow = new SettingsAndExit();
            }
            _settingsWindow.Activate();
            _settingsWindow.Show();
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
        public static IntPtr LeagueWindowHandle
        {
            get
            {
                if (LeagueProcesses.Length == 0 || LeagueProcesses == null)
                {
                    return (IntPtr) 0;
                }
                return LeagueProcesses[0].MainWindowHandle;
            }
        }

        /// <summary>
        ///     The window handle of this instance's process.
        /// </summary>
        private IntPtr ClockWindowHandle => _clock.MainWindowHandle;

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
            Visibility = Visibility.Hidden;

            if (!ThereCanOnlyBeOne())
            {
                Application.Current.Shutdown();
            }

            NotifierIcon = new TaskbarIcon { Icon = Properties.Resources.ClockIcon };
            NotifierIcon.TrayLeftMouseDown += NotifierIconOnTrayLeftMouseDown;         

            var timerRefresher = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timerRefresher.Tick += UpdateTimerTick;
            timerRefresher.Start();

            var timerSettingsCheck = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timerSettingsCheck.Tick += SettingsTimerTick;
            timerSettingsCheck.Start();

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
            Top = LeagueWindow.Top + Settings.Default.offsetTop;
            Left = LeagueWindow.Left + Settings.Default.offsetLeft;

            var colorText = Settings.Default.colorCode;
            var color = Color.FromArgb(colorText.A, colorText.R, colorText.G, colorText.B);
            _textTime.Foreground = new SolidColorBrush(color);
        }
            
        /// <summary>
        ///     Reads the settings file and updates the clocks position and color values if they have changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">args.</param>
        private void SettingsTimerTick(object sender, EventArgs e)
        {
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

        /// <summary>
        ///     An attempt to get rid of the notifier icon that stays after the program closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mainWindow_Closed(object sender, EventArgs e)
        {
            NotifierIcon.Icon = null;
        }

        /// <summary>
        ///     Another attempt to clean it up, if you're reading this and know how to dipose of it before the process dies let me know!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mainWindow_Closing(object sender, CancelEventArgs e)
        {
            NotifierIcon.Icon = null;
            NotifierIcon.Visibility = Visibility.Hidden;
        }
    }
}