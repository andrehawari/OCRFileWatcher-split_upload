using Microsoft.Extensions.Options;
using OCRFileWatcher.WorkerService.Config;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OCRFileWatcher.WorkerService.Processing
{
    public class FileUploader : IFileUploader
    {
        private readonly AppSetting _appSetting;
        private static HttpClient _httpClient = new HttpClient();

        public FileUploader(IOptions<AppSetting> appSetting)
        {
            _appSetting = appSetting.Value;
            _httpClient.BaseAddress = new Uri(_appSetting.DMSApi);
            _httpClient.Timeout = new TimeSpan(0, 0, 15);

        }
        public Task<HttpResponseMessage> GetTicket()
        {
            string auth = "api/v1/auth";
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(auth);
            return response;
        }

        public Task<HttpResponseMessage> CreateDocument()
        {
            string manageNodes = "api/v2/nodes";
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(manageNodes);
            return response;
        }
    }
}
