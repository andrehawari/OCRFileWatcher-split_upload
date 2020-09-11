using System.Collections.Generic;

namespace OCRFileWatcher.WorkerService.Model
{
    public class DocumentType
    {
        public string AdmissionNo { get; set; }
        public int Page { get; set; }
        public string Type { get; set; }
        public string DocumentCode { get; set; }
        public List<PayerDetail> PayerDetails { get; set; }
        public string InvoiceNo { get; set; }

    }
}