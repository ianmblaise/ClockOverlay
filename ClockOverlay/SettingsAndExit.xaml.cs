using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;

namespace ClockOverlay
{
    /// <summary>
    /// Interaction logic for SettingsAndExit.xaml
    /// </summary>
    public partial class SettingsAndExit : Window
    {
        public SettingsAndExit()
        {
            InitializeComponent();
        }

        private void ButtonExitClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to close the clock?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void textBlock1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsTop.Visibility == Visibility.Hidden)
            {
                HideGrids();
                gridSettingsTop.Visibility = Visibility.Visible;
                return;
            }

            gridSettingsTop.Visibility = Visibility.Hidden;
        }

        private void HideGrids()
        {
            gridSettingsColor.Visibility = Visibility.Hidden;
            gridSettingsLeft.Visibility = Visibility.Hidden;
            gridSettingsTop.Visibility = Visibility.Hidden;
            
        }

        private void textBlockColor_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsColor.Visibility == Visibility.Hidden)
            {
                HideGrids();
                gridSettingsColor.Visibility = Visibility.Visible;
                textBoxColor.Text = Settings.TextColor;
                return;
            }

            gridSettingsColor.Visibility = Visibility.Hidden;
        }

        private void textBlockTop_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsTop.Visibility == Visibility.Hidden)
            {
                HideGrids();
                gridSettingsTop.Visibility = Visibility.Visible;
                textBoxTop.Text = Settings.TopOffset.ToString();
                return;
            }

            gridSettingsTop.Visibility = Visibility.Hidden;
        }

        private void textBlockLeft_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsLeft.Visibility == Visibility.Hidden)
            {
                HideGrids();
                textBoxLeft.Text = Settings.LeftOffset.ToString();
                gridSettingsLeft.Visibility = Visibility.Visible;
                return;
            }

            gridSettingsLeft.Visibility = Visibility.Hidden;
        }

        private void _buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            if (gridSettingsBar.Visibility == Visibility.Visible)
            {
                Height = Height -= 30;
                gridSettingsBar.Visibility = Visibility.Hidden;
                return;
            }

            gridSettingsBar.Visibility = Visibility.Visible;
            Height = Height += 30;
        }

        private void _settingsAndExit_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void ButtonSaveSettingsClick(object sender, RoutedEventArgs e)
        {
            gridSettingsBar.Visibility = Visibility.Hidden;
            MainWindow.NotifierIcon.ShowBalloonTip("Saved.", "Settings have been saved. Changes should take effect immediately.", BalloonIcon.Info);
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void gridTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
