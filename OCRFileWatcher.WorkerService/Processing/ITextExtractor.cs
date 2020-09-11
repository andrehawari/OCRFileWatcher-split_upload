using OCRFileWatcher.WorkerService.Model;

namespace OCRFileWatcher.WorkerService.Processing
{
    public interface ITextExtractor
    {
        DocumentMetadata StartExtract(string folderPath);
    }
}