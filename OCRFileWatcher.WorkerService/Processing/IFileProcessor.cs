using OCRFileWatcher.WorkerService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OCRFileWatcher.WorkerService.Processing
{
    public interface IFileProcessor
    {
        Task<string> MoveFile(string source);
        void CopyToOutput(DocumentMetadata documentMetadata, List<FinalMetadata> originFiles);
        List<FinalMetadata> SplitPDF(DocumentMetadata documentMetadata, string originFile);

        string RunPDFToImage(string source);
    }
}