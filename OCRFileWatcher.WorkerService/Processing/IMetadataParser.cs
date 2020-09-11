using OCRFileWatcher.WorkerService.Model;

namespace OCRFileWatcher.WorkerService.Processing
{
    public interface IMetadataParser
    {
        Label GetLabelMetadata(string input);
    }
}