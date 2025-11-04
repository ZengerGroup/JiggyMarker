using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiggyMarker
{
    internal class ReportBuilder
    {
        string JobNumber;
        (int SROrders, int JROrders, int COrders, int SRPuzzles, int JRPuzzles) MainCounts;
        ErrorHandler Errors;
        public int GlueCount;
        public ReportBuilder(string jobNumber, ErrorHandler errors)
        {
            Errors = errors;
            JobNumber = jobNumber;
            GlueCount = GetGlueCount();
            MainCounts.SROrders = -1;
            MainCounts.JROrders = -1;
            MainCounts.COrders = -1;
            MainCounts.SRPuzzles = -1;
            MainCounts.JRPuzzles = -1;
        }
        public void SendReport()
        {
            Mailer ReportMailer = new Mailer(JobNumber, Errors);
            Errors.GenerateErrorReport(JobNumber);
            ReportMailer.SendMail(MainCounts.SROrders, MainCounts.JROrders, MainCounts.COrders, MainCounts.SRPuzzles, MainCounts.JRPuzzles, GlueCount);
        }
        public void GenerateBatchReport((string[][] Srs, string[][] Jrs, string[][] Combos, int SrCount, int JrCount) mainBatch)
        {
            MainCounts = (mainBatch.Srs.Length, mainBatch.Jrs.Length, mainBatch.Combos.Length, mainBatch.SrCount, mainBatch.JrCount);
            string ReportPath = Path.Combine(Configurator.ReportDir, String.Format("{0}-BatchReport-{1}.csv", JobNumber, DateTime.Now.ToString("MMddyy")));
            File.AppendAllText(ReportPath, "\"SR Orders\",\"QTY\",\"JR Orders\",\"QTY\",\"Combo Orders\",\"QTY\"" + Environment.NewLine);
            int index = 0;
            while (index < mainBatch.Srs.Length || index < mainBatch.Jrs.Length || index < mainBatch.Combos.Length)
            {
                File.AppendAllText(ReportPath, String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"" + Environment.NewLine,
                    index < mainBatch.Srs.Length ? mainBatch.Srs[index][0] + "-" + mainBatch.Srs[index][1] : "", index < mainBatch.Srs.Length ? mainBatch.Srs[index][2] : "",
                    index < mainBatch.Jrs.Length ? mainBatch.Jrs[index][0] + "-" + mainBatch.Jrs[index][1] : "", index < mainBatch.Jrs.Length ? mainBatch.Jrs[index][2] : "",
                    index < mainBatch.Combos.Length ? mainBatch.Combos[index][0] + "-" + mainBatch.Combos[index][1] : "", index < mainBatch.Combos.Length ? mainBatch.Combos[index][2] : ""));
                index++;
            }
        }
        public void GenerateReprintReport((int Puzzles, int Sleeves, int Inserts, int Labels, int Posters) reprintBatch)
        {
            string ReportPath = Path.Combine(Configurator.ReportDir, String.Format("{0}-ReprintReport-{1}.csv", JobNumber, DateTime.Now.ToString("MMddyy")));
            File.AppendAllText(ReportPath, "\"Piece Type\",\"Quantity\"" + Environment.NewLine +
                String.Format("\"Puzzles\",\"{0}\"", reprintBatch.Puzzles) + Environment.NewLine +
                String.Format("\"Sleeves\",\"{0}\"", reprintBatch.Sleeves) + Environment.NewLine +
                String.Format("\"Inserts\",\"{0}\"", reprintBatch.Inserts) + Environment.NewLine +
                String.Format("\"Labels\",\"{0}\"", reprintBatch.Labels) + Environment.NewLine +
                String.Format("\"Posters\",\"{0}\"", reprintBatch.Posters) + Environment.NewLine);
        }
        public int GetGlueCount()
        {
            string[] glueFiles = Directory.GetFiles(Configurator.GlueSpreads);
            return glueFiles.Length / 3;
        }
    }
}
