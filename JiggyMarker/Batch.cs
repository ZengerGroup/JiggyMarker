using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace JiggyMarker
{
    internal class Batch : IDisposable
    {
        List<Order> Orders;
        List<Order> SROrders;
        List<Order> JROrders;
        List<Order> ComboOrders;
        int SRSequence;
        int JRSequence;
        string JobNumber;
        ErrorHandler Errors;
        public Batch(string inputDir, string jobNumber, ErrorHandler errorHandler) 
        {
            SRSequence = 0; JRSequence = 0;
            Orders = new List<Order>();
            SROrders = new List<Order>();
            JROrders = new List<Order>();
            ComboOrders = new List<Order>();
            JobNumber = jobNumber;
            Errors = errorHandler;
            InitOrders(Directory.GetFiles(inputDir));
        }
        private void InitOrders(string[] batchFiles)
        {
            Logger.WriteLog("Initializing orders.", false);
            for(int i = 0; i < batchFiles.Length; i++)
            {
                int index = GetOrderIndex(batchFiles[i]);
                if (index >= 0) Orders[index].AddFile(batchFiles[i]);
                else Orders.Add(new Order(batchFiles[i], Errors));
            }
            Logger.WriteLog("Initialization complete.", false);
        }
        private int GetOrderIndex(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string orderNumber = fileName.Split("-")[0];
            for(int i = 0; i < Orders.Count; i++) if (Orders[i].OrderNumber == orderNumber) return i;
            return -1;
        }
        private void SortOrders()
        {
            for(int i = 0; i < Orders.Count; i++)
            {
                switch (Orders[i].GetOrderType())
                {
                    case "COMBO":
                        ComboOrders.Add(Orders[i]);
                        break;
                    case "SR":
                        SROrders.Add(Orders[i]);
                        break;
                    case "JR":
                        JROrders.Add(Orders[i]);
                        break;
                    default:
                        Logger.WriteLog("No type found for order {0}.", false, Orders[i].OrderNumber);
                        break;
                }
            }
        }
        public void WorkBatch()
        {
            Logger.WriteLog("Beginning sort.", false);
            SortOrders();
            Logger.WriteLog("Found {0} SR orders, {1} JR orders and {2} combo orders.", false, SROrders.Count.ToString(), JROrders.Count.ToString(), ComboOrders.Count.ToString());
            for(int i = 0; i < SROrders.Count; i++) SRSequence = SROrders[i].WorkOrder("SR", SRSequence, false);
            AssemblePdfs("SR");
            for (int i = 0; i < ComboOrders.Count; i++) SRSequence = ComboOrders[i].WorkOrder("SR", SRSequence, true);
            for (int i = 0; i < ComboOrders.Count; i++) JRSequence = ComboOrders[i].WorkOrder("JR", JRSequence, true);
            AssemblePdfs("Combo");
            for (int i = 0; i < JROrders.Count; i++) JRSequence = JROrders[i].WorkOrder("JR", JRSequence, false);
            AssemblePdfs("JR");
        }
        public void WorkBatch(string rework)
        {
            Logger.WriteLog("Beginning sort.", false);
            SortOrders();
            Logger.WriteLog("Found {0} SR orders, {1} JR orders and {2} combo orders.", false, SROrders.Count.ToString(), JROrders.Count.ToString(), ComboOrders.Count.ToString());
            for (int i = 0; i < SROrders.Count; i++) SRSequence = SROrders[i].WorkOrder("SR", SRSequence, false);
            AssemblePdfs("SR", rework);
            for (int i = 0; i < ComboOrders.Count; i++) SRSequence = ComboOrders[i].WorkOrder("SR", SRSequence, true);
            for (int i = 0; i < ComboOrders.Count; i++) JRSequence = ComboOrders[i].WorkOrder("JR", JRSequence, true);
            AssemblePdfs("Combo", rework);
            for (int i = 0; i < JROrders.Count; i++) JRSequence = JROrders[i].WorkOrder("JR", JRSequence, false);
            AssemblePdfs("JR", rework);
        }
        private void AssemblePdfs(string type)
        {
            CombineFiles(type);
            CleanUp();
        }
        private void AssemblePdfs(string type, string rework)
        {
            CombineFiles(type, rework);
            CleanUp();
        }
        private void CombineFiles(string type)
        {
            string[] puzzleFiles = Directory.GetFiles(Configurator.PuzzleAssembly);
            if(puzzleFiles.Length > 0)
                BuildPuzzleOutput(type, puzzleFiles);
            string[] insertFiles = Directory.GetFiles(Configurator.InsertAssembly);
            if(insertFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.InsertOutput, String.Format("{0}-{1}Inserts{2}.pdf", JobNumber, type, DateTime.Now.ToString("MMddyy"))), insertFiles);
            string[] sleeveFiles = Directory.GetFiles(Configurator.SleeveAssembly);
            if (sleeveFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.SleeveOutput, String.Format("{0}-{1}Sleeves{2}.pdf", JobNumber, type, DateTime.Now.ToString("MMddyy"))), sleeveFiles);
            string[] posterFiles = Directory.GetFiles(Configurator.PosterAssembly);
            if (posterFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.PosterOutput, String.Format("{0}-{1}Posters{2}.pdf", JobNumber, type, DateTime.Now.ToString("MMddyy"))), posterFiles);
            string[] labelFiles = Directory.GetFiles(Configurator.LabelAssembly);
            if (labelFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.LabelOutput, String.Format("{0}-{1}Labels{2}.pdf", JobNumber, type, DateTime.Now.ToString("MMddyy"))), labelFiles);
        }
        private void CombineFiles(string type, string rework)
        {
            string[] puzzleFiles = Directory.GetFiles(Configurator.PuzzleAssembly);
            if (puzzleFiles.Length > 0)
                BuildPuzzleOutput(type, rework, puzzleFiles);
            string[] insertFiles = Directory.GetFiles(Configurator.InsertAssembly);
            if (insertFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.InsertOutput, String.Format("{0}-{1}Inserts{2}.pdf", rework, type, DateTime.Now.ToString("MMddyy"))), insertFiles);
            string[] sleeveFiles = Directory.GetFiles(Configurator.SleeveAssembly);
            if (sleeveFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.SleeveOutput, String.Format("{0}-{1}Sleeves{2}.pdf", rework, type, DateTime.Now.ToString("MMddyy"))), sleeveFiles);
            string[] posterFiles = Directory.GetFiles(Configurator.PosterAssembly);
            if (posterFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.PosterOutput, String.Format("{0}-{1}Posters{2}.pdf", rework, type, DateTime.Now.ToString("MMddyy"))), posterFiles);
            string[] labelFiles = Directory.GetFiles(Configurator.LabelAssembly);
            if (labelFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.LabelOutput, String.Format("{0}-{1}Labels{2}.pdf", rework, type, DateTime.Now.ToString("MMddyy"))), labelFiles);
        }
        private void BuildOutputFile(string outputName, string[] fileList)
        {
            PdfDocument combinedFile = new PdfDocument();
            for (int i = 0; i < fileList.Length; i++)
            {
                PdfDocument individualFile = new PdfDocument();
                individualFile = PdfReader.Open(fileList[i], PdfDocumentOpenMode.Import);
                for (int j = 0; j < individualFile.PageCount; j++) combinedFile.AddPage(individualFile.Pages[j]);
            }
            if (combinedFile.PageCount > 0) combinedFile.Save(outputName);
            combinedFile.Close();
        }
        private void BuildPuzzleOutput(string type, string rework, string[] puzzleFiles)
        {
            if(puzzleFiles.Length < 601)
                BuildOutputFile(Path.Combine(Configurator.PuzzleOutput, String.Format("{0}-{1}Puzzles{2}.pdf", rework, type, DateTime.Now.ToString("MMddyy"))), puzzleFiles);
            else
            {
                IEnumerable<string[]> splitPuzzles = puzzleFiles.Chunk(500);
                int batchCount = 1;
                foreach(var chunk in splitPuzzles)
                {
                    BuildOutputFile(Path.Combine(Configurator.PuzzleOutput, String.Format("{0}-{1}Puzzles{2}_part{3}.pdf", rework, type, DateTime.Now.ToString("MMddyy"), batchCount)), chunk);
                    batchCount++;
                }
            }
        }
        private void BuildPuzzleOutput(string type, string[] puzzleFiles)
        {
            if (puzzleFiles.Length < 601)
                BuildOutputFile(Path.Combine(Configurator.PuzzleOutput, String.Format("{0}-{1}Puzzles{2}.pdf", JobNumber, type, DateTime.Now.ToString("MMddyy"))), puzzleFiles);
            else
            {
                IEnumerable<string[]> splitPuzzles = puzzleFiles.Chunk(500);
                int batchCount = 1;
                foreach (var chunk in splitPuzzles)
                {
                    BuildOutputFile(Path.Combine(Configurator.PuzzleOutput, String.Format("{0}-{1}Puzzles{2}_part{3}.pdf", JobNumber, type, DateTime.Now.ToString("MMddyy"), batchCount)), chunk);
                    batchCount++;
                }
            }
        }
        private void CleanUp()
        {
            foreach (string file in Directory.GetFiles(Configurator.PuzzleAssembly)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.SleeveAssembly)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.InsertAssembly)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.LabelAssembly)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.PosterAssembly)) File.Delete(file);
        }
        public bool OrdersExist()
        {
            return (Orders.Count > 0);
        }
        public (string[][], string[][], string[][], int, int) GetBatchSummary()
        {
            List<string[]> srs = new List<string[]>();
            List<string[]> jrs = new List<string[]>();
            List<string[]> combos = new List<string[]>();
            for (int i = 0; i < SROrders.Count; i++) foreach(string[] recipe in SROrders[i].GetOrderSummary()) srs.Add(recipe);
            for (int i = 0; i < JROrders.Count; i++) foreach (string[] recipe in JROrders[i].GetOrderSummary()) jrs.Add(recipe);
            for (int i = 0; i < ComboOrders.Count; i++) foreach (string[] recipe in ComboOrders[i].GetOrderSummary()) combos.Add(recipe);
            return (srs.ToArray(), jrs.ToArray(), combos.ToArray(), (srs.Count > 1 || combos.Count > 1) ? SRSequence : 0, (jrs.Count > 1 || combos.Count > 1) ? JRSequence : 0);
        }
        public void Dispose()
        {
            for(int i = 0; i <Orders.Count; i++) { Orders[i] = null; }
            Orders.Clear();
            for (int i = 0; i < SROrders.Count; i++) { SROrders[i] = null; }
            SROrders.Clear();
            for (int i = 0; i < JROrders.Count; i++) { JROrders[i] = null; }
            JROrders.Clear();
            for (int i = 0; i < ComboOrders.Count; i++) { ComboOrders[i] = null; }
            ComboOrders.Clear();
        }
    }
}
