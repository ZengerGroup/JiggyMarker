using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiggyMarker
{
    internal class Recipe
    {
        FileData PuzzleData;
        FileData InsideData;
        FileData OutsideData;
        public string RecipeID;
        public string QTY;
        public string JobType { get
            {
                if (PuzzleData == null || InsideData == null || OutsideData == null) return "ERROR";
                else return PuzzleData.Type;
            } }
        public Recipe(string filePath) 
        {
            string[] fileData = Path.GetFileName(filePath).Split("-");
            RecipeID = fileData[1];
            QTY = (fileData[2].ToLower().Contains("sub")) ? "1" : fileData[3].Substring(3);
            AddFile(filePath);
        }
        public void AddFile(string filePath)
        {
            string[] fileData = Path.GetFileName(filePath).Split("-");
            string fileType = fileData[fileData.Length - 1];
            switch(fileType.ToLower())
            {
                case "render_insert.pdf":
                case "render_poster.pdf":
                    InsideData = new FileData(filePath);
                    break;
                case "render_label.pdf":
                case "render_sleeve.pdf":
                    OutsideData = new FileData(filePath);
                    break;
                case "render_puzzle.pdf":
                    PuzzleData = new FileData(filePath);
                    break;
                default:
                    Logger.WriteLog("File error {0}-{1}", false, fileData[0], fileData[1]);
                    break;
            }
        }
        public int WorkRecipe(int sequence, bool combo)
        {
            int index = 0;
            if(JobType != "Error")
            {
                for(; index < PuzzleData.Quantity; index++)
                {
                    string puzzleOut = Path.Combine(Configurator.PuzzleAssembly, BuildFileName(PuzzleData, index, combo));
                    UIDMarker PuzzleMarker = new UIDMarker(PuzzleData, puzzleOut, sequence + index, combo);
                    PuzzleMarker.Write();
                    string insideOut = Path.Combine((InsideData.Type == "JR") ? Configurator.PosterAssembly : Configurator.InsertAssembly, BuildFileName(InsideData, index, combo));
                    UIDMarker InsideMarker = new UIDMarker(InsideData, insideOut, sequence + index, combo);
                    InsideMarker.Write();
                    string outsideOut = Path.Combine((OutsideData.Type == "JR") ? Configurator.LabelAssembly : Configurator.SleeveAssembly, BuildFileName(OutsideData, index, combo));
                    UIDMarker OutsideMarker = new UIDMarker(OutsideData, outsideOut, sequence + index, combo);
                    OutsideMarker.Write();
                }
            }
            return sequence + index;
        }
        private string BuildFileName(FileData data, int countNumber, bool combo)
        {
            string answer = "";
            if (combo) answer += (data.Type == "JR") ? "000_" : "ZZZ_";
            answer += Path.GetFileNameWithoutExtension(data.FilePath);
            answer += "_" + countNumber.ToString("D3") + ".pdf";
            return answer;
        }
    }
}
