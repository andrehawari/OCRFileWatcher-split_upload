using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCRFileWatcher.WorkerService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCRFileWatcher.WorkerService.Test
{
    [TestClass]
    public class TestDictionary
    {
        [TestMethod]
        public void TestMethod1()
        {
            var payerDetail = new List<PayerDetail>()
            {
                new PayerDetail() { PayerCode = "00952", PayerName = "SIH" }
            };

            var documentTypes = new List<DocumentType>()
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

                };


            var documentTypes2 = new List<DocumentType>()
                {
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = "CIV2003120041", Page = 1, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2003126234", DocumentCode = null, InvoiceNo = "CIV2003450084", Page = 2, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2007230006", DocumentCode = null, InvoiceNo = "CIV2003128345", Page = 3, PayerDetails = payerDetail, Type = "INVOICE"},
                };

            var documentTypes3 = new List<DocumentType>()
                {
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = "CIV2003120041", Page = 1, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2003126234", DocumentCode = null, InvoiceNo = "CIV2003450084", Page = 2, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2007230006", DocumentCode = null, InvoiceNo = "CIV2003128345", Page = 3, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2007230006", DocumentCode = null, InvoiceNo = "CIV2003128345", Page = 4, PayerDetails = payerDetail, Type = "INVOICE"},

                };

            var documentTypes4 = new List<DocumentType>()
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
                    new DocumentType() { AdmissionNo = "CPA2009780006", DocumentCode = null, InvoiceNo = "CIV2003120041", Page = 21, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = null, InvoiceNo = "CIV2003120041", Page = 22, PayerDetails = payerDetail, Type = "INVOICE"},

                };


            var documentTypes5 = new List<DocumentType>()
                {
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 1, PayerDetails = null, Type = "OTHER DOCUMENT"}

                };

            var documentTypes6 = new List<DocumentType>()
                {
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 1, PayerDetails = null, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003126234", DocumentCode = null, InvoiceNo = "CIV2003450084", Page = 2, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 3, PayerDetails = null, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003120006", DocumentCode = "I.11", InvoiceNo = null, Page = 4, PayerDetails = payerDetail, Type = "HASIL MEDICAL CHECK UP"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 5, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = null, DocumentCode = null, InvoiceNo = null, Page = 6, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003834536", DocumentCode = null, InvoiceNo = null, Page = 7, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003800459", DocumentCode = null, InvoiceNo = null, Page = 8, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003834536", DocumentCode = null, InvoiceNo = null, Page = 9, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2003834536", DocumentCode = null, InvoiceNo = null, Page = 10, PayerDetails = payerDetail, Type = "INVOICE"},
                    new DocumentType() { AdmissionNo = "CPA2003834536", DocumentCode = "I.11", InvoiceNo = null, Page = 11, PayerDetails = payerDetail, Type = "HASIL MEDICAL CHECK UP"},
                    new DocumentType() { AdmissionNo = "CPA2003834536", DocumentCode = null, InvoiceNo = null, Page = 12, PayerDetails = payerDetail, Type = "OTHER DOCUMENT"},
                    new DocumentType() { AdmissionNo = "CPA2003126234", DocumentCode = null, InvoiceNo = "CIV2003450084", Page = 13, PayerDetails = payerDetail, Type = "INVOICE"},




                };


            var coba = ToDicty(documentTypes6);

            foreach (var item in coba)
            {
                var admision = item.Value.Select(x => x.AdmissionNo).FirstOrDefault(); // ini musti diganti
                var invoiceNo = item.Value.Select(x => x.InvoiceNo).FirstOrDefault(); // ini musti diganti

            }


            Console.WriteLine(coba);
        }

        //public Lookup<string, List<DocumentType>> ToDict(IEnumerable<DocumentType> documentTypes)
        //{
        //    Lookup<string, List<DocumentType>> output = new Lookup<string, List<DocumentType>>();
        //    string currentType = string.Empty;
        //    string currentAdmission = string.Empty;
        //    int counter = 0;

        //    foreach (var documentType in documentTypes)
        //    {


        //        if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType && documentType.AdmissionNo == currentAdmission)
        //        {
        //            currentType = documentType.Type;
        //            currentAdmission = documentType.AdmissionNo;

        //            if (!output.ContainsKey(currentType))
        //            {
        //                output.Add(currentType, new List<DocumentType>());
        //            }
        //        }
        //        else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType)
        //        {
        //            currentType = documentType.Type;
        //            currentAdmission = documentType.AdmissionNo;

        //            if (!output.ContainsKey(currentType))
        //            {
        //                output.Add(currentType, new List<DocumentType>());
        //            }
        //        }
        //        else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type == currentType && documentType.AdmissionNo != currentAdmission)
        //        {
        //            currentType = documentType.Type;
        //            currentAdmission = documentType.AdmissionNo;

        //            output.Add(currentType, new List<DocumentType>());
        //        }


        //        output[currentType].Add(documentType);
        //    }

        //    return output;
        //}


        public Dictionary<string, List<DocumentType>> ToDicty(IEnumerable<DocumentType> documentTypes)
        {
            var output = new Dictionary<string, List<DocumentType>>();
            string currentType = string.Empty;
            string currentAdmission = string.Empty;
            int currentPage = 0;

            foreach (var documentType in documentTypes)
            {

                if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType && documentType.AdmissionNo == currentAdmission)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;
                    currentPage = documentType.Page;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }
                else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;
                    currentPage = documentType.Page;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }
                else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type == currentType && documentType.AdmissionNo != currentAdmission)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;
                    currentPage = documentType.Page;

                    output.Add(currentType, new List<DocumentType>());
                }
                else if (documentType.Type == "OTHER DOCUMENT" && (currentType == "OTHER DOCUMENT" || currentType != "OTHER DOCUMENT") && documentType.AdmissionNo == null)
                {
                    if (documentType.AdmissionNo != null)
                    {
                        currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                        currentAdmission = documentType.AdmissionNo;
                        currentPage = documentType.Page;
                    }

                    if (string.IsNullOrEmpty(currentType))
                    {
                        currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                        currentAdmission = documentType.AdmissionNo;
                        currentPage = documentType.Page;
                    }

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }

                }
                else if (documentType.Type == "OTHER DOCUMENT" && documentType.AdmissionNo != currentAdmission && documentType.Page > currentPage)
                {
                    if (documentType.AdmissionNo != null)
                    {
                        currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                        currentAdmission = documentType.AdmissionNo;
                        currentPage = documentType.Page;
                    }


                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }


                output[currentType].Add(documentType);
            }

            return output;
        }

    }
}
