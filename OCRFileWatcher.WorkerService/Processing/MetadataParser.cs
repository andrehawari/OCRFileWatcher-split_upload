using System;
using OCRFileWatcher.WorkerService.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Extensions.Logging;
using OCRFileWatcher.WorkerService.Config;
using Microsoft.Extensions.Options;

namespace OCRFileWatcher.WorkerService.Processing
{
    public class MetadataParser : IMetadataParser
    {
        //Data Invoice Number dan label atau field _label akan mengikuti hasil sebelumnya selama belum ada yang baru.
        //Properties _label baru berganti jika ada nilai yang baru
        private Label _label = new Label();
        private readonly RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
        private readonly ILogger<MetadataParser> _logger;
        private readonly PatternConfig _patternConfig;

        public MetadataParser(ILogger<MetadataParser> logger, IOptions<PatternConfig> patternConfig)
        {
            _logger = logger;
            _patternConfig = patternConfig.Value;
        }

        public Label GetLabelMetadata(string input)
        {
            try
            {
                var temp = input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var dmslabel = Regex.Match(input, _patternConfig.DMSKeyword);

                if (dmslabel.Success)
                {
                    var result = GetDMSLabel(input, temp);
                }
                if (!dmslabel.Success)
                {
                    var result = GetPatientLabel(input, temp);
                }
            }
            catch (NullReferenceException nullEx)
            {
                _logger.LogError(nullEx.Message);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return _label;
        }

        private Label GetDMSLabel(string input, List<string> temp)
        {
            var transactionNumber = Regex.Match(input, _patternConfig.Transaction).ToString();
            var dmslabel = Regex.Match(input, _patternConfig.DMSKeyword);
            var dmscode = Regex.Match(input, _patternConfig.DMSCode).ToString();
            var indexTrNumber = temp.ToList().FindIndex(x => Regex.IsMatch(x, _patternConfig.Transaction, _options));
            var indexDmsLabel = temp.ToList().FindIndex(x => Regex.IsMatch(x, _patternConfig.DMSKeyword));
            var wordList = new List<string>();

            for (int i = indexDmsLabel + 1; i < indexTrNumber; i++)
            {
                wordList.Add(temp.ToList()[i]);
            }

            wordList.Remove(dmscode);

            var documentType = string.Join(" ", wordList.ToArray());

            _label.AdmissionNo = transactionNumber;
            _label.DocumentType = documentType;
            _label.DMSCode = dmscode;

            return _label;
        }

        private Label GetPatientLabel(string input, List<string> temp)
        {
            //TODO try catch
            try
            {
                var isTransactionNo = Regex.Match(input, _patternConfig.Transaction);
                var invoiceNo = temp.FirstOrDefault(x => Regex.IsMatch(x, _patternConfig.Invoice, _options)) ?? null;

                if (isTransactionNo.Success)
                {
                    _label.AdmissionNo = isTransactionNo.Value;
                }

                if (invoiceNo != null)
                {
                    invoiceNo = invoiceNo.Split(' ').ToList().FirstOrDefault(x => Regex.IsMatch(x, _patternConfig.Invoice, _options));
                    _label.InvoiceNo = invoiceNo;
                }

                return _label;
            }
            catch (NullReferenceException nullEx)
            {

                _logger.LogError(nullEx.Message);
            }

            return _label;
        }
    }
}
