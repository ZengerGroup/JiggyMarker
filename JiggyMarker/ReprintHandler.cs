using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;

namespace JiggyMarker
{
    internal class ReprintHandler : IDisposable
    {
        public string[] ReprintFiles;
        public int SleeveCount, InsertCount, PuzzleCount, LabelCount, PosterCount;
        public ReprintHandler()
        {
            SleeveCount = InsertCount = PuzzleCount = LabelCount = PosterCount = 0;
            ReprintFiles = Directory.GetFiles(Configurator.ReprintFiles);
            Logger.WriteLog("Found {0} reprint files.", false, ReprintFiles.Length.ToString());
            PrepareAssembly();
        }
        public void AssemblePdfs()
        {
            CombineFiles();
            CleanUp();
        }
        public (int, int, int , int, int) GetBatchSummary()
        {
            return (PuzzleCount, SleeveCount, InsertCount, LabelCount, PosterCount);
        }
        void PrepareAssembly()
        {
            for(int i = 0; i < ReprintFiles.Length; i++)
            {   
                Console.WriteLine(i);
                Console.WriteLine(ReprintFiles[i]);
                string piece = GetPiece(ReprintFiles[i]);
                if (piece == null) Logger.WriteLog("Reprint file [{0}] can not be processed.{1}{2}", false, ReprintFiles[i], Environment.NewLine, "Name could not be parsed.");
                else
                {
                    File.Move(ReprintFiles[i], Path.Combine(GetDestination(piece), Path.GetFileName(ReprintFiles[i])));
                }
            }
        }
        string GetPiece(string filePath)
        {
            try
            {
                string[] fileData = Path.GetFileNameWithoutExtension(filePath).Split('-');
                Console.WriteLine(fileData[fileData.Length - 1]);
                return fileData[fileData.Length - 1].Split('_')[1];
            }
            catch
            {
                return null;
            }
        }
        string GetDestination(string piece)
        {
            Console.WriteLine(piece);
            switch (piece)
            {
                case "sleeve":
                    SleeveCount++;
                    return Configurator.ReprintSleeves;
                case "poster":
                    PosterCount++;
                    return Configurator.ReprintPosters;
                case "puzzle":
                    PuzzleCount++;
                    return Configurator.ReprintPuzzles;
                case "label":
                    LabelCount++;
                    return Configurator.ReprintLabels;
                case "insert":
                    InsertCount++;
                    return Configurator.ReprintInserts;
                default: return Configurator.PieceTypeError;
            }
        }
        private void CombineFiles()
        {
            string[] puzzleFiles = Directory.GetFiles(Configurator.ReprintPuzzles);
            if (puzzleFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.PuzzleOutput, String.Format("ReprintPuzzles{0}.pdf", DateTime.Now.ToString("MMddyy"))), puzzleFiles);
            string[] insertFiles = Directory.GetFiles(Configurator.ReprintInserts);
            if (insertFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.InsertOutput, String.Format("ReprintInserts{0}.pdf", DateTime.Now.ToString("MMddyy"))), insertFiles);
            string[] sleeveFiles = Directory.GetFiles(Configurator.ReprintSleeves);
            if (sleeveFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.SleeveOutput, String.Format("ReprintSleeves{0}.pdf", DateTime.Now.ToString("MMddyy"))), sleeveFiles);
            string[] posterFiles = Directory.GetFiles(Configurator.ReprintPosters);
            if (posterFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.PosterOutput, String.Format("ReprintPosters{0}.pdf", DateTime.Now.ToString("MMddyy"))), posterFiles);
            string[] labelFiles = Directory.GetFiles(Configurator.ReprintLabels);
            if (labelFiles.Length > 0)
                BuildOutputFile(Path.Combine(Configurator.LabelOutput, String.Format("ReprintLabels{0}.pdf", DateTime.Now.ToString("MMddyy"))), labelFiles);
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
        private void CleanUp()
        {
            foreach (string file in Directory.GetFiles(Configurator.ReprintPuzzles)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.ReprintSleeves)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.ReprintInserts)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.ReprintPosters)) File.Delete(file);
            foreach (string file in Directory.GetFiles(Configurator.ReprintLabels)) File.Delete(file);
        }
        public void Dispose()
        {
            ReprintFiles = null;
        }
    }
}
