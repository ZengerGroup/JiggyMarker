using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiggyMarker
{
    internal class ErrorHandler
    {
        private List<string> GeneralErrors;
        private string[] PageSize;
        public ErrorHandler() 
        {
            string[] pageSizeFiles = Directory.GetFiles(Configurator.PageSizeError);
            PageSize = new string[pageSizeFiles.Length];
            for(int i = 0; i < pageSizeFiles.Length; i++)
            {
                PageSize[i] = GetOrderAndRecipe(pageSizeFiles[i]);
            }
            GeneralErrors = new List<string>();
        }
        
        public void AddGeneralError(string orderAndRecipe)
        {
            if(!PageSize.Contains(orderAndRecipe)) GeneralErrors.Add(orderAndRecipe);
        }
        public int GetErrorCount()
        {
            return GeneralErrors.Count + PageSize.Length;
        }
        public void GenerateErrorReport(string JobNumber)
        {
            string ReportPath = Path.Combine(Configurator.ReportDir, "ReportHold", String.Format("{0}-ErrorReport-{1}.csv", JobNumber, DateTime.Now.ToString("MMddyy")));
            File.AppendAllText(ReportPath, "\"Page Size Errors\",\"All Other Errors\"" + Environment.NewLine);
            int index = 0;
            while(index < PageSize.Length || index < GeneralErrors.Count)
            {
                File.AppendAllText(ReportPath, String.Format("\"{0}\",\"{1}\"" + Environment.NewLine,
                    index < PageSize.Length ? PageSize[index] : "", index < GeneralErrors.Count ? GeneralErrors[index] : ""));
                index++;
            }
        }

        private string GetOrderAndRecipe(string fileName)
        {
            string shortName = Path.GetFileNameWithoutExtension(fileName);
            string[] splitName = shortName.Split('-');
            return String.Format("{0}-{1}", splitName[0], splitName[1]);
        }
    }
}
