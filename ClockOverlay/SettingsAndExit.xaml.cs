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
using Xceed.Wpf.Toolkit;

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
            var result = System.Windows.MessageBox.Show("Are you sure you want to close the clock?", "Confirm", MessageBoxButton.YesNo);
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

        private void TextBlockColorPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsColor.Visibility == Visibility.Hidden)
            {
                HideGrids();
                gridSettingsColor.Visibility = Visibility.Visible;
                var convertFromString = ColorConverter.ConvertFromString(Settings.TextColor);
                if (convertFromString != null)
                {
                    colorPicker.SelectedColor = (Color) convertFromString;
                }
                return;
            }

            gridSettingsColor.Visibility = Visibility.Hidden;
        }

        private void textBlockTop_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSettingsTop.Visibility == Visibility.Hidden)
            {
                HideGrids();
                textBoxTop.Text = Settings.TopOffset.ToString();
                gridSettingsTop.Visibility = Visibility.Visible;
                return;
            }

            gridSettingsTop.Visibility = Visibility.Hidden;
        }

        private void TextBlockLeftPreviewMouseUp(object sender, MouseButtonEventArgs e)
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

        private void ButtonSettingsClick(object sender, RoutedEventArgs e)
        {
            if (gridSettingsBar.Visibility == Visibility.Visible)
            {
                HideGrids();
                Height = Height -= 30;
                gridSettingsBar.Visibility = Visibility.Hidden;
                return;
            }
            HideGrids();
            gridSettingsBar.Visibility = Visibility.Visible;
            Height = Height += 30;
        }

        private void SettingsAndExitClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void ButtonSaveSettingsClick(object sender, RoutedEventArgs e)
        {
            gridSettingsBar.Visibility = Visibility.Hidden;
            MainWindow.NotifierIcon.ShowBalloonTip("Saved.", "Settings have been saved. Changes should take effect immediately.", BalloonIcon.Info);
        }

        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMinimizeClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void GridTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void ButtonApplyLeftClick(object sender, RoutedEventArgs e)
        {
            var leftOffset = 0;
            var isInt = int.TryParse(textBoxLeft.Text, out leftOffset);
            if (!isInt)
            {
                System.Windows.MessageBox.Show("Bad value. Whole numbers only.");
                return;
            }
            var writeValues = new Dictionary<string, string>
            {
                { "left", leftOffset.ToString() },
                { "top", Settings.TopOffset.ToString() },
                { "color", Settings.TextColor }
            };

            Settings.WriteSettings(writeValues);
            Xceed.Wpf.Toolkit.MessageBox.Show("Saved!");
        }

        private void ButtonApplyTopClick(object sender, RoutedEventArgs e)
        {
            var topOffset = 0;
            var isInt = int.TryParse(textBoxTop.Text, out topOffset);
            if (!isInt)
            {
                System.Windows.MessageBox.Show("Bad value. Whole numbers only.");
                return;
            }
            var writeValues = new Dictionary<string, string>
            {
                { "left", Settings.LeftOffset.ToString() },
                { "top", topOffset.ToString() },
                { "color", Settings.TextColor }
            };

            Settings.WriteSettings(writeValues);
            Xceed.Wpf.Toolkit.MessageBox.Show("Saved!");
        }

        private void _settingsAndExit_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource settingsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("settingsViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // settingsViewSource.Source = [generic data source]
        }

        private void buttonApplyColor_Click(object sender, RoutedEventArgs e)
        {
            Color color = colorPicker.SelectedColor;
            
            var writeValues = new Dictionary<string, string>
            {
                { "left", Settings.LeftOffset.ToString() },
                { "top", Settings.TopOffset.ToString() },
                { "color", color.ToString() }
            };

            Settings.WriteSettings(writeValues);
        }
    }
}
