using System.Net.Http;
using System.Threading.Tasks;

namespace OCRFileWatcher.WorkerService.Processing
{
    public interface IFileUploader
    {
        Task<HttpResponseMessage> CreateDocument();
        Task<HttpResponseMessage> GetTicket();
    }
}