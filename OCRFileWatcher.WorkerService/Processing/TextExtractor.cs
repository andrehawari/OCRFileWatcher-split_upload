using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OCRFileWatcher.WorkerService.Config;
using OCRFileWatcher.WorkerService.DataAccess;
using OCRFileWatcher.WorkerService.Extension;
using OCRFileWatcher.WorkerService.ExternalServices;
using OCRFileWatcher.WorkerService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OCRFileWatcher.WorkerService.Processing
{
    public class TextExtractor : ITextExtractor
    {
        private readonly IGoogleOCRService _googleOCRService;
        private readonly IMetadataParser _metadataParser;
        private readonly ILogger<TextExtractor> _logger;
        private readonly IServiceProvider _services;
        private readonly AppSetting _appSetting;
        private readonly FolderConfig _folderConfig;
        private int Count;

        public TextExtractor(
            ILogger<TextExtractor> logger,
            IOptions<AppSetting> appSetting,
            IOptions<FolderConfig> folderConfig,
            IGoogleOCRService googleOCRService,
            IMetadataParser metadataParser,
            IServiceProvider service)
        {
            _logger = logger;
            _appSetting = appSetting.Value;
            _folderConfig = folderConfig.Value;
            _services = service;
            _googleOCRService = googleOCRService;
            _metadataParser = metadataParser;
        }
        public DocumentMetadata StartExtract(string folderPath)
        {
            var documentMetadata = new DocumentMetadata();
            var fileName = Path.GetFileName(folderPath);
            var files = Directory.GetFiles(folderPath);
            var textOutput = new Dictionary<int, string>();
            var documentTypes = new List<DocumentType>();

            using (var scope = _services.CreateScope())
            {
               // var sqlAccess = scope.ServiceProvider.GetRequiredService<SQLAccess>();

                foreach (var file in files)
                {
                    int inc = 1 + Count++;

                    try
                    {

                        var output = _googleOCRService.GetOCRValue(file);
                        var metadataLabel = _metadataParser.GetLabelMetadata(output);

                        //write text from google to in memory dictionary
                        textOutput.Add(inc, output);
                        
                        var payerDetails = new List<PayerDetail>();
                       // payerDetails = sqlAccess.GetPayerDetails(metadataLabel.AdmissionNo);

                        var documentType = new DocumentType()
                        {
                            Page = inc,
                            Type = metadataLabel.DocumentType,
                            AdmissionNo = metadataLabel.AdmissionNo ?? null,
                            DocumentCode = metadataLabel.DMSCode ?? null,
                            PayerDetails = !payerDetails.Any() ? null : payerDetails,
                            InvoiceNo = metadataLabel.InvoiceNo ?? null
                        };

                        if (metadataLabel.DocumentType != null)
                        {
                            documentTypes.Add(documentType);
                        }

                        documentMetadata.DocumentTypes = documentTypes;
                    }
                    catch (NullReferenceException nullEx)
                    {
                        _logger.LogError(nullEx.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }

                }
                documentMetadata.Unit = _appSetting.UnitName;
                documentMetadata.DateInsert = DateTime.Now.ToString("yyyy-MM-dd hh:MM:ss");
                documentMetadata.FileName = string.Format("{0}{1}", fileName, ".pdf");

                //sqlAccess.InsertMetadata(documentMetadata.JsonExtension());
                CreateResultFiles(textOutput, fileName);

                return documentMetadata;
            }

        }

        private void CreateResultFiles(Dictionary<int, string> input, string fileName)
        {
            using (StreamWriter file = new StreamWriter(_folderConfig.TextGoogleVision + fileName + ".txt"))
                foreach (var entry in input)
                    file.WriteLine("[{0} {1}]", entry.Key, entry.Value);
        }

    }
}
