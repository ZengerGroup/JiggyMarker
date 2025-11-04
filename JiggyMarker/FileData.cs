using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiggyMarker
{
    internal class FileData
    {
        private static string[] SRSkus = Configurator.SRSkus.Split('|');
        private static string[] JRSkus = Configurator.JRSkus.Split('|');

        public string FilePath;
        public string FileName;
        public string Order;
        public string Recipe;
        public string Type;
        public int Quantity;
        public string Piece;
        public FileData(string file)
        {
            FilePath = file;
            FileName = Path.GetFileName(file);
            string[] parsedData = FileName.Split('-');
            Order = parsedData[0];
            Recipe = parsedData[1];
            Type = GetFileType(parsedData[2]);
            Quantity = (parsedData[2].ToLower().Contains("sub")) ? 1 : Int32.Parse(parsedData[3].Substring(3));
            Piece = parsedData[4].Split('_')[1].Split('.')[0];
        }
        private string GetFileType(string typeData)
        {
            if (typeData.Contains("500") || SRSkus.Contains(typeData.ToLower())) return "SR";
            else if (typeData.Contains("100") || JRSkus.Contains(typeData.ToLower())) return "JR";
            else return "ERROR";
        }
        
    }
}
