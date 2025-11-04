using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace JiggyMarker
{
    internal class TravelerPuller
    {
        string[][] SrOrders;
        string[][] JrOrders;
        string[][] ComboOrders;
        string[] URIPath;
        string JobNumber;
        public TravelerPuller((string[][], string[][], string[][], int, int) batch, string jobNumber)
        {
            SrOrders = ParseOrders(batch.Item1);
            JrOrders = ParseOrders(batch.Item2);
            ComboOrders = ParseOrders(batch.Item3);
            URIPath = Configurator.TravelerURI.Split('|');
            JobNumber = jobNumber;
        }
        public async Task PullAll()
        {
            await PullDown(SrOrders);
            CombinePdfs("SrTravelers");
            await PullDown(JrOrders);
            CombinePdfs("JrTravelers");
            await PullDown(ComboOrders);
            CombinePdfs("ComboTravelers");
        }
        async Task PullDown(string[][] orderList)
        {
            for(int i = 0; i < orderList.Length; i++)
            {
                try
                {
                    string destinationURI = string.Format("{0}{1}{2}{3}", URIPath[0], orderList[i][1], URIPath[1], orderList[i][0]);
                    HttpResponseMessage httpResponse = await PullTraveler(destinationURI);
                    if(httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var fs = new FileStream(
                        Path.Combine(Configurator.TravelerAssembly, string.Format("{0}-{1}.pdf", orderList[i][0], orderList[i][1])),
                        FileMode.CreateNew))
                            {
                                await httpResponse.Content.CopyToAsync(fs);
                            };
                    }
                }
                catch
                {
                    Logger.WriteLog("Error Pulling Traveler: {0}-{1}", false, orderList[i][0], orderList[i][1]);
                }
            }
        }
        void CombinePdfs(string orderType)
        {
            PdfDocument combinedFile = new PdfDocument();
            string[] fileList = Directory.GetFiles(Configurator.TravelerAssembly);
            for (int i = 0; i < fileList.Length; i++)
            {
                PdfDocument individualFile = new PdfDocument();
                individualFile = PdfReader.Open(fileList[i], PdfDocumentOpenMode.Import);
                for (int j = 0; j < individualFile.PageCount; j++) combinedFile.AddPage(individualFile.Pages[j]);
            }
            if (combinedFile.PageCount > 0) combinedFile.Save(Path.Combine(Configurator.TravelerOutput, string.Format("{0}-{1}{2}.pdf", JobNumber, orderType, DateTime.Now.ToString("ddMMyy"))));
            combinedFile.Close();
            foreach (string file in fileList) File.Delete(file);
        }
        async Task<HttpResponseMessage> PullTraveler(string destination)
        {
            var uri = new Uri(destination);
            HttpClient httpClient = new HttpClient();
            return await httpClient.GetAsync(uri);

        }
        string[][] ParseOrders(string[][] unparsed)
        {
            List<string[]> Parsed = new List<string[]>();
            for(int i = 0; i < unparsed.Length; i++)
            {
                bool Repeat = false;
                for (int ii = 0; ii < Parsed.Count; ii++) if (unparsed[i][0] == Parsed[ii][0]) Repeat = true;
                if (!Repeat) Parsed.Add([unparsed[i][0], unparsed[i][1]]);
            }
            return Parsed.ToArray();
        }
    }
}
