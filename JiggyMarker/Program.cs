using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using System;
using System.IO;

namespace JiggyMarker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //System setup:
            if (!Directory.Exists(Configurator.WorkingDir)) Logger.ErrorExit(["Input directory not found.", Configurator.WorkingDir], 42069);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();
            Logger.JobNumber = GetJobNumber();
            Logger.WriteLog("Starting batch for {0}. Job # {1}.", true, DateTime.Now.ToString("d"), Logger.JobNumber);
            ErrorHandler Errors = new ErrorHandler();


            //Run main batch:
            Logger.WriteLog("Beginning main batch.", false);
            Batch DaysBatch = new Batch(Configurator.WorkingDir, Logger.JobNumber, Errors);
            DaysBatch.WorkBatch();
            Logger.WriteLog("Batch complete.", false);

            //Run reprint batch:
            Batch ReprintBatch = new Batch(Configurator.ReprintFiles, Logger.JobNumber, Errors);
            if (ReprintBatch.OrdersExist())
            {
                Logger.WriteLog("Beginning reprint batch.", false);
                ReprintBatch.WorkBatch("Rework");
                Logger.WriteLog("Reprints complete.", false);
            }

            //Run reporting:
            Logger.WriteLog("Generating reporting.", false);
            ReportBuilder Report = new ReportBuilder(Logger.JobNumber, Errors);
            Report.GenerateBatchReport(DaysBatch.GetBatchSummary());
            DaysBatch.Dispose();
            if (ReprintBatch.OrdersExist())
            {
                //Report.GenerateReprintReport(ReprintBatch.GetBatchSummary());
                ReprintBatch.Dispose();
            }
            Report.SendReport();
            CleanWorkingDir();
        }
        static string GetJobNumber()
        {
            string[] reports = Directory.GetFiles(Path.Combine(Configurator.ReportDir, "ReportHold"));
            if (reports.Length > 1 || reports.Length == 0)
            {
                Logger.WriteLog("Unable to determine job #. {0} files found in report folder.", false, reports.Length.ToString());
                return "#ERROR";
            }
            string reportName = Path.GetFileName(reports[0]);
            if (reportName[0] == '_') return reportName.Substring(7,6);
            else return reportName.Substring(0, 6);
        }
        static void CleanWorkingDir()
        {
            string[] files = Directory.GetFiles(Configurator.WorkingDir);
            string NewArchive = Path.Combine(Configurator.ArchiveDir, String.Format("IndividualFiles-{0}", DateTime.Now.ToString("MMdd")));
            Directory.CreateDirectory(NewArchive);
            for(int i = 0; i < files.Length; i++)
            {
                File.Move(files[i], Path.Combine(NewArchive, Path.GetFileName(files[i])));
            }
        }
    }
}
