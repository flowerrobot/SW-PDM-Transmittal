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

        public string ArchivePath = "";
        public string EmailFrom = "DocumentContro@IAC-Australia.com.au";
        string smtpServer = "smtp.gmail.com";
        int smtpPort = 25;
        string OutputDir;
        Transmittal Transmittal;
        List<FileInfo> files = new List<FileInfo>();
        public Publisher(Transmittal trans)
        {
            Transmittal = trans;            
            OutputDir = Path.Combine(System.IO.Path.GetTempPath(), "TransmitalMgr", $"Prj{Transmittal.Id}");        
        }

        /// <summary>
        /// Packages the files together ready for email
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PackageAsync()
        {

            
            foreach (var doc in Transmittal.Files)
            {
                string hash = doc.FileId.ToString("X"); //Convert to hash table
                string folder = hash.Substring(hash.Length - 1, 1); //TODO Check if it should be -2

                string fileName = Path.Combine(ArchivePath, folder, doc.FileId.ToString(), doc.FileId.ToString("0000000") + ".pdf");
                string newPath = Path.Combine(OutputDir, fileName);
                if (File.Exists(fileName)) //TODO Check number zeros required.
                {
                    await CopyFileAsync(fileName, newPath);
                    MarkPdf(newPath, Transmittal.Project.NumberStr, Transmittal.WorkShopJtsk, Transmittal.WorkShopJtsk, doc.TotalQty);
                    files.Add(new FileInfo(newPath));
                }
            }

            await ZipFiles(Path.Combine(OutputDir, $"Documents{Transmittal.Id}.zip"), files);
            return true;            
        }


        async Task<bool> ZipFiles(string outputFileName, List<FileInfo> files)
        {
            await Task.Run(() =>
            {
                using (FileStream zipToOpen = new FileStream(outputFileName, FileMode.Create))
                {
                    using (System.IO.Compression.ZipArchive la = new System.IO.Compression.ZipArchive(zipToOpen, System.IO.Compression.ZipArchiveMode.Create))
                    {

                        foreach (var f in files)
                        {
                            la.CreateEntry(f.FullName, System.IO.Compression.CompressionLevel.Optimal);
                        }
                    }
                }
            }
            );
            return true;
        }

        /// <summary>
        /// Merges PDF's together - Used when documents issued to workshop
        /// </summary>
        /// <param name="outputFileName"></param>
        /// <param name="files"></param>
        void MergePDFs(string outputFileName, List<Document> files)
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
        /// <summary>
        /// Send the email asycn
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync()
        {
            //TODO If Size is larger upload to cloud?
            try
            {
                await Task.Run(() =>
                 {
                     MailMessage mail = new MailMessage(EmailFrom, "");
                    //Add all the recpients
                    foreach (var recipt in Transmittal.Recipients)
                         mail.To.Add(recipt.Email);


                     SmtpClient client = new SmtpClient();
                     client.Port = smtpPort;
                     client.DeliveryMethod = SmtpDeliveryMethod.Network;
                     client.UseDefaultCredentials = false;
                     client.Host = "";
                     mail.Subject = "IAC Document Transmital";

                     ///Replace std formats in comment
                     string FilledComment = Transmittal.Comments;
                     FilledComment = FilledComment.Replace("<status>", Transmittal.IssueType.ToString());
                     FilledComment = FilledComment.Replace("<issuer>", Transmittal.IssueBy.FullName);
                     FilledComment = FilledComment.Replace("<issueDate>", Transmittal.SentDate?.ToString("d"));
                     mail.Body = FilledComment;

                     mail.Body += "\n\n \n\n";
                     mail.Body += "\nDocuments \t Description \t Revision";
                     foreach (var files in Transmittal.Files)
                     {
                         mail.Body += $"{files.FileName}\t {files.Description}\t {files.Revision}";
                     }
                     foreach (var att in files)
                     {
                         mail.Attachments.Add(new Attachment(att.FullName));
                     }
                     string msg = "Message";
                     client.SendAsync(mail, msg);
                     return true;
                 });
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return false;
            }
            return true;
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

        /// <summary>
        /// Mark the pdf with file numbers
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="jobNo"></param>
        /// <param name="jtsk">The manufacturing code for the workshop</param>
        /// <param name="userName"></param>
        /// <param name="qty"></param>
        /// <param name="notes"></param>
        public static void MarkPdf(string fileName, string jobNo, string jtsk, string userName, int qty = 0, string notes = "")
        {
            using (PdfLoadedDocument ldoc = new PdfLoadedDocument(fileName))
            {
                foreach (PdfPageBase page in ldoc.Pages)
                {
                    PdfGraphics gra = page.Graphics;
                    SizeF pgSize = page.Size;

                    string qtyText = "";
                    if (qty > 0)
                        qtyText = "- TOTAL QTY : {qty}   ";

                    gra.DrawString($"JOB :{jobNo} | JTSK : {jtsk}     ISSUED BY : {userName}", font, PdfBrushes.Blue, 10, pgSize.Height - 10, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
                    gra.DrawString($"ISSUE DATE :{DateTime.Now:d}   {qtyText}{notes}", font, PdfBrushes.Blue, 10, pgSize.Height - 20, new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Bottom));
                }
                ldoc.Save();
                ldoc.Close(true);
            }
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

        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }
    }
}
