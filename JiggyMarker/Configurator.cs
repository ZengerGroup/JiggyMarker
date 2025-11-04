using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JiggyMarker
{
    internal static class Configurator
    {
        public static string Template = ConfigurationManager.AppSettings["Template"];
        public static string LogPath = ConfigurationManager.AppSettings["LogPath"];
        public static string ReportEmail = ConfigurationManager.AppSettings["ReportEmail"];
        public static string ReportDir = ConfigurationManager.AppSettings["ReportDir"];
        public static string WorkingDir = ConfigurationManager.AppSettings["WorkingDir"];
        public static string ArchiveDir = ConfigurationManager.AppSettings["ArchiveDir"];

        public static string PageSizeError = ConfigurationManager.AppSettings["PSErrorDir"];
        public static string PieceTypeError = ConfigurationManager.AppSettings["PTErrorDir"];
        public static string GlueSpreads = ConfigurationManager.AppSettings["GlueSpreads"];

        public static string PuzzleAssembly = ConfigurationManager.AppSettings["PuzzleAssembly"];
        public static string PosterAssembly = ConfigurationManager.AppSettings["PosterAssembly"];
        public static string LabelAssembly = ConfigurationManager.AppSettings["LabelAssembly"];
        public static string InsertAssembly = ConfigurationManager.AppSettings["InsertAssembly"];
        public static string SleeveAssembly = ConfigurationManager.AppSettings["SleeveAssembly"];

        public static string ReprintPuzzles = ConfigurationManager.AppSettings["ReprintPuzzles"];
        public static string ReprintPosters = ConfigurationManager.AppSettings["ReprintPosters"];
        public static string ReprintLabels = ConfigurationManager.AppSettings["ReprintLabels"];
        public static string ReprintInserts = ConfigurationManager.AppSettings["ReprintInserts"];
        public static string ReprintSleeves = ConfigurationManager.AppSettings["ReprintSleeves"];

        public static string PuzzleOutput = ConfigurationManager.AppSettings["PuzzleOutput"];
        public static string PosterOutput = ConfigurationManager.AppSettings["PosterOutput"];
        public static string LabelOutput = ConfigurationManager.AppSettings["LabelOutput"];
        public static string InsertOutput = ConfigurationManager.AppSettings["InsertOutput"];
        public static string SleeveOutput = ConfigurationManager.AppSettings["SleeveOutput"];

        public static string ReprintFiles = ConfigurationManager.AppSettings["ReprintDir"];

        public static string TravelerURI = ConfigurationManager.AppSettings["TravelerURI"];
        public static string TravelerAssembly = ConfigurationManager.AppSettings["TravelerAssembly"];
        public static string TravelerOutput = ConfigurationManager.AppSettings["TravelerOutput"];

        public static string SRSkus = ConfigurationManager.AppSettings["SRSkus"];
        public static string JRSkus = ConfigurationManager.AppSettings["JRSkus"];

        public static string MailAccount = ConfigurationManager.AppSettings["MailAccount"];
        public static string MailSecret = ConfigurationManager.AppSettings["MailSecret"];
    }
}
