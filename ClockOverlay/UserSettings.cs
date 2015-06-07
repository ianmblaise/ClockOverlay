using System.Windows.Media;
using ClockOverlay.Properties;

namespace ClockOverlay
{
    /// <summary>
    /// User settings class. Self explanitory.
    /// </summary>
    public class UserSettings
    {
        /// <summary>
        ///     Offset from the top down. 
        ///     Example: Form with width: 300 and height: 400,
        ///     half way down the form is Top = 200 and 
        ///     half way across the form is Left = 150.
        /// </summary>
        public int TopOffset
        {
            get { return Settings.Default.offsetTop; }
            set
            {
                Settings.Default.offsetTop = value;
                Settings.Default.Save();
            }
        }

        /// <summary>
        ///     Explained this one already, it's just the left offset instead of the top offset. 
        ///     <see cref="TopOffset"/> should be a sufficient description.
        /// </summary>
        public int LeftOffset
        {
            get { return Settings.Default.offsetLeft; }
            set
            {
                Settings.Default.offsetLeft = value;
                Settings.Default.Save();
            }
        }

        /// <summary>
        ///     This is the color to be painted for the clock's time.
        /// </summary>
        public Color TextColor
        {
            get { return Settings.Default.colorCode; }
            set
            {
                Settings.Default.colorCode = value;
                Settings.Default.Save();
            }
        }
    }
}
