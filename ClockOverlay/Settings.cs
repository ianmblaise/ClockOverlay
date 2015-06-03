using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClockOverlay
{
    public static class Settings
    {
        public static string SettingsFile = "settings.xml";

        public static XmlDocument SettingsDocument
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SettingsFile);
                return doc;
            }
        }

        public static void WriteSettings(Dictionary<string, string> settings)
        {
            using (var writer = new XmlTextWriter(SettingsFile, Encoding.ASCII)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = '.'
            })
            {
                writer.WriteStartDocument(true);
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
