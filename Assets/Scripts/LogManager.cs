using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace EliteChess.Managers
{
    public static class LogManager
    {
        private const string FileName = "Log.log";
        public static void Log(string str, LogType enumLog = LogType.Log, [CallerFilePath] string className = "", [CallerMemberName] string functionName = "")
        {
            StringBuilder output = new StringBuilder();
            output.Append("[");
            output.Append(DateTime.UtcNow);
            output.Append(" (UTC)] - [");
            output.Append(enumLog);
            output.Append("] - [");
            output.Append(className.Substring(className.LastIndexOf("Scripts\\") + "Scripts\\".Length));
            output.Append("] - [");
            output.Append(functionName);
            output.Append("]:\t");
            output.Append(str);
            output.Append(Environment.NewLine);
            if (GameManager.Instance.settings == null || GameManager.Instance.settings.ShowDebug)
            {
                switch (enumLog)
                {
                    case LogType.Warning:
                        Debug.LogWarning(output.ToString());
                        break;
                    case LogType.Error:
                        Debug.LogError(output.ToString());
                        break;
                    case LogType.Log:
                    default:
                        Debug.Log(output.ToString());
                        break;
                }
            }
            GameManager.Instance.ShowLog(str);
#if !UNITY_WEBGL
            if (GameManager.Instance.settings == null || GameManager.Instance.settings.SaveDebug)
            {
                File.AppendAllText($"{Application.dataPath}/{FileName}", output.ToString());
            }
#endif
        }
    }
}