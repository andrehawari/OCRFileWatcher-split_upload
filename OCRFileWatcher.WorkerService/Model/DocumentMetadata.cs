using System;
using System.Collections.Generic;

namespace OCRFileWatcher.WorkerService.Model
{
    public class DocumentMetadata
    {
        public string DateInsert { get; set; }
        public string FileName { get; set; }
        public string LocalFilePath { get; set; }
        public int TotalPages { get; set; }
        public List<DocumentType> DocumentTypes { get; set; }
        public string Unit { get; set; }

    }
}
