using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OCRFileWatcher.WorkerService.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OCRFileWatcher.WorkerService.BackgroundServices
{
    class Producer : BackgroundService
    {

        private readonly ILogger<Producer> _logger;
        private FileSystemWatcher _folderWatcher;
        private readonly FolderConfig _folderConfig;
        private readonly Channel<string> _channel;

        public Producer(ILogger<Producer> logger,
            Channel<string> channel, IOptions<FolderConfig> folderConfig)
        {
            _logger = logger;
            _folderConfig = folderConfig.Value;
            _channel = channel;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Starting");
            if (!Directory.Exists(_folderConfig.Input))
            {
                _logger.LogWarning($"Please make sure the InputFolder [{_folderConfig.Input}] exists, then restart the service.");
                return Task.CompletedTask;
            }

            _logger.LogInformation($"Binding Events from Input Folder: {_folderConfig.Input}");
            _folderWatcher = new FileSystemWatcher(_folderConfig.Input, "*.pdf")
            {
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                  NotifyFilters.DirectoryName
            };
            _folderWatcher.Created += Input_OnChanged;
            _folderWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        protected void Input_OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                _logger.LogInformation($"InBound Change Event Triggered by [{e.FullPath}]");
                _channel.Writer.WriteAsync(e.FullPath);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");
            _folderWatcher.EnableRaisingEvents = false;
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing Service");
            _folderWatcher.Dispose();
            base.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}
