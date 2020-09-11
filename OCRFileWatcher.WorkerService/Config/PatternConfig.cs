using System;
using System.Collections.Generic;
using System.Text;

namespace OCRFileWatcher.WorkerService.Config
{
    public class PatternConfig
    {
        public string Invoice { get; set; }
        public string Transaction { get; set; }
        public string DMSKeyword { get; set; }
        public string DMSCode { get; set; }
    }
}
