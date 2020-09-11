using Google.Cloud.Vision.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OCRFileWatcher.WorkerService.Extension;
using OCRFileWatcher.WorkerService.Processing;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OCRFileWatcher.WorkerService.BackgroundServices
{
    public class Consumer : BackgroundService
    {
        private readonly Channel<string> _channel;
        private readonly ILogger<Consumer> _logger;
        private readonly IServiceProvider _services;
        private readonly IDocumentTypeParser _documentTypeParser;

        public Consumer(Channel<string> channel, ILogger<Consumer> logger,
            IServiceProvider services, IDocumentTypeParser documentTypeParser
            )
        {
            _channel = channel;
            _logger = logger;
            _services = services;
            _documentTypeParser = documentTypeParser;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync())
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var stopwatch = Stopwatch.StartNew();

                        var fileProcessor = scope.ServiceProvider.GetRequiredService<IFileProcessor>();
                        var textExtractor = scope.ServiceProvider.GetRequiredService<ITextExtractor>();

                        var path = await fileProcessor.MoveFile(item);
                        var imagesPath = fileProcessor.RunPDFToImage(path);
                        var metadata = textExtractor.StartExtract(imagesPath);

                        //Masih mau diganti lagi urutannya.
                        var files = fileProcessor.SplitPDF(metadata, path);

                        //gw mau buat return dari split itu metadata yang bisa langsung dipassing ke CopyOutput
                        fileProcessor.CopyToOutput(metadata, files);
                        stopwatch.Stop();

                        _logger.LogInformation("Time ocr&split:{0}", stopwatch.Elapsed.TotalMilliseconds);

                        //upload document
                        stopwatch.Restart();

                        // logic buat upload ke dms


                        stopwatch.Stop();

                        _logger.LogInformation("Time upload document:{0}", stopwatch.Elapsed.TotalMilliseconds ); //pritn to log json
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
