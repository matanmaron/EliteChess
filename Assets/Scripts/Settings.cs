using System;
using System.IO;
using UnityEngine;

namespace EliteChess.Managers
{
    [Serializable]
    public class Settings
    {
        public bool ShowDebug = false;
        public bool SaveDebug = false;
        public string Path = string.Empty;

        public override string ToString()
        {
            return $"Path:{Path} | ShowDebug:{ShowDebug} | SaveDebug:{SaveDebug}";
        }
    }

    public static class SettingsManager
    {
        private const string FileName = "settings.json";
        public static Settings GetSettings()
        {
            Settings settings = ImportSettings();
            if (settings == null)
            {
                var tmp = new Settings();
                ExportSettings(tmp);
                return tmp;
            }
            return settings;
        }

        static Settings ImportSettings()
        {
            if (!File.Exists($"{Application.dataPath}/{FileName}"))
            {
                LogManager.Log("no settings file found, creating default", LogType.Warning);
                return null;
            }
            string jsonImport = File.ReadAllText($"{Application.dataPath}/{FileName}");
            if (jsonImport == "{}")
            {
                LogManager.Log("settings file empty, creating default", LogType.Warning);
                return null;
            }
            return JsonUtility.FromJson<Settings>(jsonImport);
        }

        static void ExportSettings(Settings settings)
        {
            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText($"{Application.dataPath}/{FileName}", jsonExport);
            LogManager.Log("settings exported successfully", LogType.Log);
        }

        internal static void Save(Settings settings)
        {
            ExportSettings(settings);
        }
    }
}