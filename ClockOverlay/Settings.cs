using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Media;
using System.Xml;

namespace ClockOverlay
{
    public static class Settings
    {
        public static string SettingsFile = "settings.xml";


        public static float TopOffset { get; set; }
        public static float LeftOffset { get; set; }
        public static Color TextColor { get; set; }
        public static bool TextEffects { get; set; }

        public static Dictionary<string, string> ReadSettings()
        {
            using (var reader = XmlReader.Create(SettingsFile, new XmlReaderSettings() { IgnoreWhitespace = true }))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement())
                    {
                        continue;
                    }

                    switch (reader.Name)
                    {
                        case "xOffset":
                            if (reader.Read())
                            {
                                LeftOffset = reader.ReadContentAsInt();
                            }
                            break;

                        case "yOffset":
                            if (reader.Read())
                            {
                                TopOffset = reader.ReadContentAsInt();
                            }
                            break;

                        case "TextColor":
                            if (reader.Read())
                            {
                                var colorConverter = ColorConverter.ConvertFromString(reader.Value);
                                if (colorConverter != null)
                                {
                                    TextColor = (Color)colorConverter;
                                }
                            }
                            break;
                    }
                }
            }
            var dict = new Dictionary<string, string>
            {
                { "xOffset", LeftOffset.ToString(CultureInfo.InvariantCulture) },
                { "yOffset", TopOffset.ToString(CultureInfo.InvariantCulture) },
                { "TextColor", HexConverter(TextColor) }
            };

            return dict;
        }

        private static string HexConverter(System.Windows.Media.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }


        public static void WriteSettings(Dictionary<string, string> settings, bool overwrite = true)
        {
            if (!overwrite && System.IO.File.Exists(SettingsFile))
            {
                return;
            }
            using (
                var writer = new XmlTextWriter(SettingsFile, Encoding.ASCII) { Formatting = Formatting.Indented }
                )
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");

                foreach (var setting in settings)
                {
                    writer.WriteElementString(setting.Key, setting.Value);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
