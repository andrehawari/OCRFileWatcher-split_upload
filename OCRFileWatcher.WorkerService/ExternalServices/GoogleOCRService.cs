using Google.Cloud.Vision.V1;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OCRFileWatcher.WorkerService.ExternalServices
{
    public class GoogleOCRService : IGoogleOCRService
    {
        private readonly ILogger<GoogleOCRService> _logger;

        public GoogleOCRService(ILogger<GoogleOCRService> logger)
        {
            _logger = logger;
        }

        public string GetOCRValue(string filePath)
        {
            try
            {
                var client = ImageAnnotatorClient.Create();
                var image = Image.FromFile(filePath);
                var response = client.DetectText(image);
                var result = response.FirstOrDefault().Description;
                return result;
            }
            catch(NullReferenceException nullEx)
            {
                _logger.LogError(nullEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

             return null;
        }

        public async Task<string> GetOCRValueAsync(string filePath)
        {
            
            var client = ImageAnnotatorClient.Create();
            var image = await Image.FromFileAsync(filePath);
            var response = await client.DetectTextAsync(image);

            return response.FirstOrDefault().Description;
        }

        public async Task<string> GetOCRValueWithLocation(string filePath)
        {
            var client = ImageAnnotatorClient.Create();
            var image = await Image.FromFileAsync(filePath);
            var response = await client.DetectTextAsync(image);

            return response.FirstOrDefault().Description;
        }
    }

}
