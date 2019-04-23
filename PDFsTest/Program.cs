using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using System;
using System.Drawing;

namespace PDFsTest
{
    internal class Program
    {
        private static PdfStandardFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Regular);

        private static void Main(string[] args)
        {

            //PdfLoadedDocument ldoc = new PdfLoadedDocument(@"C:\temp\7108810852097.pdf");
            //var page = ldoc.Pages[0];
            //var gra = page.Graphics;

            //SizeF aa = page.Size;

            //string notes = "";
            //gra.DrawString($"JOB :{123456} | JTSK : {1234}     ISSUED BY : {"SETH"}", font, PdfBrushes.Blue, 10,aa.Height - 10, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
            //gra.DrawString($"ISSUE DATE :{DateTime.Now:d}   - TOTAL QTY : {3}   {notes}", font, PdfBrushes.Blue, 10,aa.Height - 20, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));


            //ldoc.Save(@"C:\temp\output.pdf");
            //ldoc.Close(true);


            MarkPdf(@"C:\temp\7108810852097.pdf", @"C:\temp\output.pdf", 123456, 1234, "Seth", 3);
        }

        public static bool MarkPdf(string inputFileName, string outputFileName, int jobNo, int jtsk, string userName, int qty = 0, string notes = "")
        {
            using (PdfLoadedDocument ldoc = new PdfLoadedDocument(inputFileName))
            {
                foreach (PdfPageBase page in ldoc.Pages)
                {
                    //PdfPageBase page = ldoc.Pages[0];
                    PdfGraphics gra = page.Graphics;

                    SizeF pgSize = page.Size;

                    string qtyText = "";
                    if (qty > 0)
                        qtyText = "- TOTAL QTY : {qty}   ";

                    gra.DrawString($"JOB :{jobNo} | JTSK : {jtsk}     ISSUED BY : {userName}", font, PdfBrushes.Blue, 10, pgSize.Height - 10, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
                    gra.DrawString($"ISSUE DATE :{DateTime.Now:d}   {qtyText}{notes}", font, PdfBrushes.Blue, 10, pgSize.Height - 20, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
                }
                ldoc.Save(outputFileName);
                ldoc.Close(true);
                return true;
            }
        }
    }
}
