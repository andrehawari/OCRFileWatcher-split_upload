using System.Threading.Tasks;

namespace OCRFileWatcher.WorkerService.ExternalServices
{
    public interface IGoogleOCRService
    {
        string GetOCRValue(string filePath);
        Task<string> GetOCRValueAsync(string filePath);
        Task<string> GetOCRValueWithLocation(string filePath);
    }
}