using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;

namespace JiggyMarker
{
    internal class UIDMarker
    {
        string FilePath;
        string UniqueID;
        string OutputPath;
        string Piece;
        int Quantity;
        Dictionary<string, XRect> UIDAreas;
        Dictionary<string, XFont> UIDFonts;

        public UIDMarker(FileData data, string output, int sequence, bool combo)
        {
            UIDAreas = new Dictionary<string, XRect>()
            {
                {"insert", new XRect(240, 450, 110, 18) },
                {"puzzle", new XRect(10, 1050, 100, 100) },
                {"sleeve", new XRect(10, 225, 50, 100) },
                {"poster", new XRect(625, -10, 100, 100) },
                {"label", new XRect(215, 100, 100, 100) }
            };
            UIDFonts = new Dictionary<string, XFont>()
            {
                {"insert", new XFont("Verdana", 8) },
                {"puzzle", new XFont("Verdana", 11) },
                {"sleeve", new XFont("Verdana", 8) },
                {"poster", new XFont("Verdana", 8) },
                {"label", new XFont("Verdana", 6) }
            };
            OutputPath = output;
            Piece = data.Piece;
            Quantity = data.Quantity;
            FilePath = data.FilePath;
            UniqueID = String.Format("{0}-{1}-{2}-{3}",DateTime.Now.ToString("MMddyy"), data.Type, data.Order, sequence.ToString("D4"));
            if (combo) UniqueID += "C";
        }

        public void Write()
        {
            PdfDocument document = new PdfDocument();
            document = PdfReader.Open(FilePath);
            if (Piece == "insert" && document.Pages.Count < 2) document.AddPage(GetTemplatePage());
            var gfx = XGraphics.FromPdfPage((Piece == "insert") ? document.Pages[1] : document.Pages[0], XGraphicsPdfPageOptions.Append);
            switch (Piece)
            {
                case "sleeve":
                    gfx.RotateAtTransform(90, new XPoint(35, 275));
                    break;
                case "puzzle":
                    gfx.RotateAtTransform(270, new XPoint(60, 1100));
                    break;
                case "label":
                    gfx.RotateAtTransform(270, new XPoint(250, 150));
                    break;
            }
            gfx.DrawString(UniqueID, UIDFonts[Piece], XBrushes.Black, UIDAreas[Piece], XStringFormats.Center);
            document.Save(OutputPath);
            document.Close();
        }
        private PdfPage GetTemplatePage()
        {
            PdfDocument template = new PdfDocument();
            template = PdfReader.Open(Configurator.Template, PdfDocumentOpenMode.Import);
            PdfPage page = template.Pages[0];
            //template.Close();
            return page;
        }
    }
}
