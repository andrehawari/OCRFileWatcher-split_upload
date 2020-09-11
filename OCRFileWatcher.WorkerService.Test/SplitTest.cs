using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OCRFileWatcher.WorkerService.Model;
using OCRFileWatcher.WorkerService.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OCRFileWatcher.WorkerService.Test
{
    [TestClass]
    public class SplitTest
    {

        [TestMethod]
        public void TestMethod1()
        {
            var mockService = new Mock<IFileProcessor>();
            var payerDetail = new List<PayerDetail>()
            {
                new PayerDetail() { PayerCode = "00952", PayerName = "SIH" }
            };

            string fileName = @"C:\Test\Temp\1_inputcopy\_ DEWI SULISTYANINGSIH_20200825_095021.pdf";
            string ouputFolder = @"C:\Test\Output";

            var documentMetadata = new DocumentMetadata()
            {
                DateInsert = DateTime.Now.ToString("yyyy-MM-dd hh:MM:ss"),
                LocalFilePath = fileName,
                DocumentTypes = new List<DocumentType>()
                {
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = "CIV2003120041", Page = 1, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.03", InvoiceNo = "CIV2003120041", Page = 2, PayerDetails = payerDetail, Type = "FOTOKOPI KARTU IDENTITAS"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.11", InvoiceNo = "CIV2003120041", Page = 3, PayerDetails = payerDetail, Type = "HASIL MEDICAL CHECK UP"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 4, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 5, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.04", InvoiceNo = "CIV2003120041", Page = 6, PayerDetails = payerDetail, Type = "HASIL RADIOLOGI"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 7, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 8, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 9, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.10", InvoiceNo = "CIV2003120041", Page = 10, PayerDetails = payerDetail, Type = "HASIL PATOLOGI"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = null, Page = 11, PayerDetails = payerDetail, Type = "HASIL LABORATORIUM"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = null, Page = 12, PayerDetails = payerDetail, Type = "HASIL LABORATORIUM"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 13, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 14, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 15, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 16, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 17, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 18, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 19, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 20, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},

                }
            };

            var listGroup = documentMetadata
                .DocumentTypes
                .Select(x => new
                {
                    x.Type,
                    x.Page
                }
                        )
                .GroupBy(g => g.Type)
                .ToDictionary(g => g.Key, g => g.Select(g => g.Page).ToList());


            var listGroup2 = documentMetadata
                            .DocumentTypes
                            .GroupBy(x => x.Type,
                            (key, group) => group.First())
                            .Select(x => new
                            {
                                x.Type,
                                x.Page
                            })
                            .ToList();

            var listGroup3 = documentMetadata
                            .DocumentTypes
                            .Count();


            var expected = new List<DocumentType>()
            {
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = "CIV2003120041", Page = 1, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.03", InvoiceNo = "CIV2003120041", Page = 2, PayerDetails = payerDetail, Type = "FOTOKOPI KARTU IDENTITAS"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.04", InvoiceNo = "CIV2003120041", Page = 6, PayerDetails = payerDetail, Type = "HASIL RADIOLOGI"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.11", InvoiceNo = "CIV2003120041", Page = 3, PayerDetails = payerDetail, Type = "HASIL MEDICAL CHECK UP"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.10", InvoiceNo = "CIV2003120041", Page = 10, PayerDetails = payerDetail, Type = "HASIL PATOLOGI"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = null, Page = 11, PayerDetails = payerDetail, Type = "HASIL LABORATORIUM"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = null, Page = 12, PayerDetails = payerDetail, Type = "HASIL LABORATORIUM"},
            };



            var output = new Dictionary<string, List<int>>();

            string currentType = String.Empty;

            foreach (var item in documentMetadata.DocumentTypes)
            {
                if (item.Type != "OTHER DOCUMENT" && item.Type != currentType)
                {
                    currentType = item.Type;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<int>());
                    }
                }

                output[currentType].Add(item.Page);
            }

            

            var fileproces = mockService;
            //fileproces.Setup(x => x.SplitPDF(documentMetadata));


            //SPLIT PDF
            foreach (var item in output)
            {
                Console.WriteLine($"{item.Key}:{String.Join(',', item.Value)}");

                var outputName = item.Key + ".pdf";
                var outputFolder = Path.Combine(ouputFolder, outputName);

                ProcessStartInfo pdftkSplit = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = String.Format(@"/c pdftk ""{0}"" cat {1} output ""{2}""",
                                                documentMetadata.LocalFilePath, String.Join(' ', item.Value), outputFolder),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

                Process pdfSplit = Process.Start(pdftkSplit);
                var messageFromCommandLine = pdfSplit.StandardOutput.ReadToEnd();
                (messageFromCommandLine.Any() ? new Action(() => Console.WriteLine(messageFromCommandLine)) : () => Console.WriteLine("Run"))();
                Console.WriteLine("dokumen split done");

            }
        }



    }

    public class SplitDocument
    {
        public string DocumentType { get; set; }
        public string PageRange { get; set; }

        public List<string> PagesRange { get; set; }
    }
}
