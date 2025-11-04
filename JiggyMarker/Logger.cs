using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiggyMarker
{
    
    internal static class Logger
    {
        static string LogPath = Path.Combine(Configurator.LogPath, String.Format("{0}.txt", DateTime.Now.ToString("MMMyyyy")));
        static string CrashPath = Path.Combine(Configurator.LogPath, String.Format("Crash_{0}.txt", DateTime.Now.ToString("MMMyyyy")));
        public static void WriteLog(string message, bool timestamp, params string[] messageArgs)
        {
            message = String.Format(message, messageArgs);
            message += Environment.NewLine;
            File.AppendAllText(LogPath, (timestamp) ? String.Format("{0}: {1}", DateTime.Now.ToString("F"), message) : message);
        }
        public static void ErrorExit(string[] message, int code)
        {
            WriteLog(message[0], true);
            string longMessage = "";
            for (int i = 0; i < message.Length; i++)
            {
                longMessage += message[i];
                if (i != message.Length - 1) longMessage += Environment.NewLine;
            }
            File.AppendAllText(CrashPath, String.Format(
                "*****START*****" + Environment.NewLine +
                "*****{0}*****" + Environment.NewLine +
                "{1}" + Environment.NewLine +
                "******END******", DateTime.Now.ToString("ddMMM"), longMessage));
            Environment.Exit(1);
        }
    }
}
