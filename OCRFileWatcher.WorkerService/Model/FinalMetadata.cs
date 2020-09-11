using System;
using System.Collections.Generic;
using System.Text;

namespace OCRFileWatcher.WorkerService.Model
{
    /// <summary>
    /// Pendingan bakal ada payer disini. Kalem aja oke?
    /// InvoiceNo, ada kasus satu admission dua invoice jadi bakal dipakein list;
    /// </summary>
    public class FinalMetadata
    {
        public string DateInsert { get; set; }
        public string FileName { get; set; }
        public string AdmissionNo { get; set; }
        public string InvoiceNo { get; set; }
        public string LocalFilePath { get; set; }
        public int TotalPages { get; set; }
        public string DocumentType { get; set; }
        public List<PayerDetail> PayerDetails { get; set; }
        public string Unit { get; set; }

    }
}
