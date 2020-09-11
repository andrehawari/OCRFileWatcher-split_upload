using System;
using System.Collections.Generic;
using System.Text;

namespace OCRFileWatcher.WorkerService.Config
{
    public class FolderConfig
    {
        public string Input { get; set; }
        public string InputCopy { get; set; }
        public string PdfImages { get; set; }
        public string SplitPdf { get; set; }
        public string Output { get; set; }
        public string TextGoogleVision { get; set; }

    }
}
