using System;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OCRFileWatcher.WorkerService.BackgroundServices;
using OCRFileWatcher.WorkerService.Config;
using OCRFileWatcher.WorkerService.DataAccess;
using OCRFileWatcher.WorkerService.ExternalServices;
using OCRFileWatcher.WorkerService.Processing;
using Serilog;
using Serilog.Events;

namespace OCRFileWatcher.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                    .WriteTo.File(@"Log\LogError.txt"))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                    .WriteTo.File(@"Log\LogWarning.txt"))
                .WriteTo.File(@"Log\log.txt")
                .CreateLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (OperationCanceledException) 
            {
                Log.Warning("Cancelled service");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AppSetting>(hostContext.Configuration.GetSection("App"));
                    services.Configure<AppSetting>(hostContext.Configuration.GetSection("App:ConnectionStrings"));
                    services.Configure<FolderConfig>(hostContext.Configuration.GetSection("Folder"));
                    services.Configure<FolderConfig>(hostContext.Configuration.GetSection("Folder:Temporary"));
                    services.Configure<ExcelConfig>(hostContext.Configuration.GetSection("Excel"));
                    services.Configure<PatternConfig>(hostContext.Configuration.GetSection("RegexPattern"));

                    services.AddTransient<IFileProcessor, FileProcessor>();
                    services.AddTransient<IGoogleOCRService, GoogleOCRService>();
                    services.AddTransient<IMetadataParser, MetadataParser1>();
                    services.AddTransient<IFileUploader, FileUploader>();
                    services.AddSingleton<IDocumentTypeParser, DocumentTypeParser>();
                    services.AddScoped<SQLAccess>();
                    services.AddScoped<ITextExtractor, TextExtractor>();


                    services.AddSingleton(Channel.CreateUnbounded<string>());

                    services.AddHostedService<Producer>();
                    services.AddHostedService<Consumer>();

                    //TODO BUAT SATU HOSTED SERVICE BERTUGAS UPDATE INVOICE

                })
                .UseSerilog();
           

    }
}
