using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ClockOverlay.Properties;
using Microsoft.Win32;

namespace ClockOverlay
{
    /// <summary>
    ///     Interaction logic for SettingsAndExit.xaml
    /// </summary>
    public partial class SettingsAndExit
    {
        private readonly UserSettings _settings = new UserSettings();

        /// <summary>
        ///     Constructor for the Settings and Exit form. Default operation..
        /// </summary>
        public SettingsAndExit()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Returns true if there is an existing key in the registry start up path.
        /// </summary>
        public static bool IsInStartUp
        {
            get
            {
                var startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");

                if (startupKey == null) throw new ArgumentNullException(nameof(startupKey));
                return startupKey.GetValue("lolClock") != null;
            }
        }

        /// <summary>
        ///     Terminates the application if the user pushes Yes on the dialog presented to them
        ///     after they press the Exit button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void ButtonExitClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to close the clock?", "Confirm", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Just hides all of the settings grids to prepare the UI for a different one to be visible.
        /// </summary>
        private void HideGrids()
        {
            gridSettingsColor.Visibility = Visibility.Hidden;
            gridSettingsLeft.Visibility = Visibility.Hidden;
            gridSettingsTop.Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///     Shows the grid with the color selector on it.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void TextBlockColorPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsColor.Visibility == Visibility.Hidden)
            {
                HideGrids();
                colorPicker.SelectedColor = _settings.TextColor;
                gridSettingsColor.Visibility = Visibility.Visible;
                return;
            }

            gridSettingsColor.Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///     Shows the grid that is used to configure the value stored for the top offset.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void textBlockTop_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsTop.Visibility == Visibility.Hidden)
            {
                HideGrids();
                textBoxTop.Text = _settings.TopOffset.ToString();
                gridSettingsTop.Visibility = Visibility.Visible;
                return;
            }

            gridSettingsTop.Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///     Shows the grid that is used to configure the value stored for the left offset.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void TextBlockLeftPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsLeft.Visibility == Visibility.Hidden)
            {
                HideGrids();
                textBoxLeft.Text = _settings.LeftOffset.ToString();
                gridSettingsLeft.Visibility = Visibility.Visible;
                return;
            }

            gridSettingsLeft.Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///     Toggles the visibility for the settings bar at the bottom of the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            if (gridSettingsBar.Visibility == Visibility.Visible)
            {
                HideGrids();
                gridSettingsBar.Visibility = Visibility.Hidden;
                Height = 140;
                return;
            }
            Height = 170;
            gridSettingsBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Raised after ButtonClose is clicked and calls <see cref="System.Windows.Window.Close()"/>. Just hides the form, doesn't terminate application.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void SettingsAndExitClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        ///     Raised when ButtonClose is clicked, calls Close() which gets intercepted by the Closing event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Raised when ButtonMinimize is clicked. Just sets the WindowState to <see cref="WindowState.Minimized"/>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Args.</param>
        private void ButtonMinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        ///     Raised while the user is holding down on the mouse, drags the form around.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        /// <summary>
        ///     Saves the Left offset setting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonApplyLeftClick(object sender, RoutedEventArgs e)
        {
            int leftOffset;
            var isInt = int.TryParse(textBoxLeft.Text, out leftOffset);
            if (!isInt)
            {
                MessageBox.Show("Bad value. Whole numbers only.");
                return;
            }

            _settings.LeftOffset = leftOffset;
        }

        /// <summary>
        ///     Saves the Top offset setting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonApplyTopClick(object sender, RoutedEventArgs e)
        {
            int topOffset;
            var isInt = int.TryParse(textBoxTop.Text, out topOffset);
            if (!isInt)
            {
                MessageBox.Show("Bad value. Whole numbers only.");
                return;
            }

            _settings.TopOffset = topOffset;
        }

        /// <summary>
        ///     Saves the Color setting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonApplyColorClick(object sender, RoutedEventArgs e)
        {
            var color = colorPicker.SelectedColor;
            _settings.TextColor = color;
        }

        /// <summary>
        ///     When it is checked then a registry key value is written to the startup path in the current user's registry.
        ///     When it is unchecked then the registry key is deleted from that path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            var startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startupKey != null && (!IsInStartUp || File.Exists(startupKey.GetValue("lolClock").ToString())))
            {
                startupKey.SetValue("lolClock",
                    "\"" + AppDomain.CurrentDomain.BaseDirectory + "ClockOverlay.exe" + "\"");
                startupKey.Close();
                return;
            }

            if (startupKey != null)
            {
                startupKey.DeleteValue("lolClock");
                startupKey.Close();
            }
        }
    }
}