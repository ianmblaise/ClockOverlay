using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Xml;

namespace ClockOverlay
{
    public static class Settings
    {
        public const string SettingsFile = "settings.xml";

        public static Int32 TopOffset => Convert.ToInt32(XmlSettings["top"]);

        public static int LeftOffset => Convert.ToInt32(XmlSettings["left"]);

        public static string TextColor => XmlSettings["color"];

        private static Dictionary<string, string> _xmlSettings; 
        public static Dictionary<string, string> XmlSettings => _xmlSettings ?? (_xmlSettings = ReadSettings());

        public static Dictionary<string, string> ReadSettings()
        {
            var settingsDict = new Dictionary<string, string>();
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
                        case "left":
                            if (reader.Read())
                            {
                                var left = reader.ReadContentAsInt();
                                settingsDict.Add("left", left.ToString());
                            }
                            break;

                        case "top":
                            if (reader.Read())
                            {
                                var top = reader.ReadContentAsInt();
                                settingsDict.Add("top", top.ToString());
                            }
                            break;

                        case "color":
                            if (reader.Read())
                            {
                                try
                                {
                                    var colorConverter = ColorConverter.ConvertFromString(reader.ReadContentAsString());
                                    if (colorConverter != null)
                                    {
                                        var color = (Color) colorConverter;
                                        settingsDict.Add("color", color.ToString());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("bad color input.");
                                }
                            }
                            break;
                    }
                }
            }

            return settingsDict;
        }

        public static void WriteSettings(Dictionary<string, string> settings, bool overwrite = true)
        {
            if (!overwrite && File.Exists(SettingsFile))
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
