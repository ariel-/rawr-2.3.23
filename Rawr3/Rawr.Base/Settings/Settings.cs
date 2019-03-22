using System;
using Rawr.Properties;
using System.IO;
using System.Xml.Serialization;

namespace Rawr
{
    [GenerateSerializer]
    public class Settings
    {
        public CacheSettings Cache { get; set; }
        public GeneralSettings General { get; set; }
        public NetworkSettings Network { get; set; }
        public OptimizerSettings Optimizer { get; set; }
        public RecentSettings Recent { get; set; }

        public static void Save(TextWriter writer)
        {
            bool wasAspx = NetworkSettings.UseAspx;
            NetworkSettings.UseAspx = false;

            Settings current = new Settings()
            {
                Cache = CacheSettings.Default,
                General = GeneralSettings.Default,
                Network = NetworkSettings.Default,
                Optimizer = OptimizerSettings.Default,
                Recent = RecentSettings.Default
            };
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            serializer.Serialize(writer, current);
            writer.Close();

            NetworkSettings.UseAspx = wasAspx;
        }

        public static void Load(TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                Settings loaded = (Settings)serializer.Deserialize(reader);
                reader.Close();

                if (loaded.Cache != null) CacheSettings.Default = loaded.Cache;
                if (loaded.General != null) GeneralSettings.Default = loaded.General;
                if (loaded.Network != null) NetworkSettings.Default = loaded.Network;
                if (loaded.Optimizer != null) OptimizerSettings.Default = loaded.Optimizer;
                if (loaded.Recent != null) RecentSettings.Default = loaded.Recent;
            }
            catch { }
        }

    }
}
