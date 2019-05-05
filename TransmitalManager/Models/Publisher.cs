using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TransmittalManager.Models
{
    /// <summary>
    /// This publishes a transmittal.
    /// </summary>
    class Publisher
    {
        private static PdfStandardFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Regular);

        Transmittal Transmittal;
        public Publisher(Transmittal trans)
        {
            Transmittal = trans;

            string CompiledPDF = $"DocumentIssued {Transmittal.Id}.{Transmittal.SentDate}.pdf";
            //TODO define save location & SaveName
            MergePDFs(CompiledPDF, Transmittal.Files);

            //TODO If Size is larger upload to cloud?

            //Send Email
            SendEmail(new List<FileInfo>(new[] { new FileInfo(CompiledPDF) }));
        }
        public void MergePDFs(string outputFileName, List<Document> files)
        {

            using (PdfDocument doc = new PdfDocument())
            {
                foreach (var file in files)
                {
                    //Add a section to PDF document.
                    PdfSection section = doc.Sections.Add();
                    section.PageLabel.Prefix = Path.GetFileNameWithoutExtension(file.FileName);
                    using (PdfLoadedDocument ldoc = new PdfLoadedDocument(file.FileName))
                    {
                        foreach (PdfPage lpage in ldoc.Pages)
                        {
                            //Add pages to the section
                            section.Pages.Add(lpage);
                            MarkPdf(section.Pages[section.Pages.Count - 1], Transmittal.Project.NumberStr, Transmittal.WorkShopJtsk, Transmittal.IssueBy.Initials, file.TotalQty, "");
                        }
                    }
                }

                doc.Save(outputFileName);
                doc.Close(true);
            }
        }

        public bool SendEmail(List<FileInfo> attachments)
        {
            try
            {
                MailMessage mail = new MailMessage("DocumentContro@IAC-Australia.com.au", "");

                foreach (var recipt in Transmittal.Recipients)
                    mail.To.Add(recipt.Email);


                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "smtp.gmail.com";
                mail.Subject = "IAC Document Transmital";
                mail.Body = Transmittal.Comments;

                mail.Body += "\n\n \n\n";
                mail.Body += "\nDocuments \t Description \t Revision";
                foreach (var files in Transmittal.Files)
                {
                    mail.Body += $"{files.FileName}\t {files.Description}\t {files.Revision}";
                }
                foreach (var att in attachments)
                {
                    mail.Attachments.Add(new Attachment(att.FullName));
                }

                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return false;
            }
        }

        public static void MarkPdf(PdfPage page, string jobNo, string jtsk, string userName, int qty = 0, string notes = "")
        {
            PdfGraphics gra = page.Graphics;
            SizeF pgSize = page.Size;

            string qtyText = "";
            if (qty > 0)
                qtyText = "- TOTAL QTY : {qty}   ";

            gra.DrawString($"JOB :{jobNo} | JTSK : {jtsk}     ISSUED BY : {userName}", font, PdfBrushes.Blue, 10, pgSize.Height - 10, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
            gra.DrawString($"ISSUE DATE :{DateTime.Now:d}   {qtyText}{notes}", font, PdfBrushes.Blue, 10, pgSize.Height - 20, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
        }


        //public static bool MarkPdf(string inputFileName, string outputFileName, int jobNo, int jtsk, string userName, int qty = 0, string notes = "")
        // {
        //     using (PdfLoadedDocument ldoc = new PdfLoadedDocument(inputFileName))
        //     {
        //         foreach (PdfPageBase page in ldoc.Pages)
        //         {
        //             //PdfPageBase page = ldoc.Pages[0];
        //             PdfGraphics gra = page.Graphics;

        //             SizeF pgSize = page.Size;

        //             string qtyText = "";
        //             if (qty > 0)
        //                 qtyText = "- TOTAL QTY : {qty}   ";

        //             gra.DrawString($"JOB :{jobNo} | JTSK : {jtsk}     ISSUED BY : {userName}", font, PdfBrushes.Blue, 10, pgSize.Height - 10, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
        //             gra.DrawString($"ISSUE DATE :{DateTime.Now:d}   {qtyText}{notes}", font, PdfBrushes.Blue, 10, pgSize.Height - 20, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
        //         }
        //         ldoc.Save(outputFileName);
        //         ldoc.Close(true);
        //         return true;
        //     }
        // }
    }
}
