using System;
using OCRFileWatcher.WorkerService.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Extensions.Logging;
using OCRFileWatcher.WorkerService.Config;
using Microsoft.Extensions.Options;
using System.IO;

namespace OCRFileWatcher.WorkerService.Processing
{
    public class MetadataParser1 : IMetadataParser
    {
        //field label di matikan, karena akan mengcopy data label dari yang sebelumnya. transient dalam singleton akan selalu aktif.
        //label di instansiasi tiap method digunakan. contoh line 29 dan line 59.
        //private Label _label = new Label();
        //penentuan jenis file dipindah kesini, tadinya di textExtractor.
        private readonly RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
        private readonly ILogger<MetadataParser> _logger;
        private readonly PatternConfig _patternConfig;
        private readonly IDocumentTypeParser _documentTypeParser;


        public MetadataParser1(
            ILogger<MetadataParser> logger,
            IDocumentTypeParser documentTypeParser,
            IOptions<PatternConfig> patternConfig)
        {
            _logger = logger;
            _patternConfig = patternConfig.Value;
            _documentTypeParser = documentTypeParser;

        }

        public Label GetLabelMetadata(string input)
        {
            var result = new Label();
            try
            {
                var temp = input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var dmslabel = Regex.Match(input, _patternConfig.DMSKeyword);
                var isTransactionNo = Regex.Match(input, _patternConfig.Transaction);
                var mrNoExist = Regex.Match(input, "MR No");

                bool patientlabel = isTransactionNo.Success && mrNoExist.Success;

                if (dmslabel.Success)
                {
                    result = GetDMSLabel(input, temp);
                }
                else if (!dmslabel.Success && patientlabel) //coba tambahin sesuatu
                {
                    result = GetPatientLabel(input, temp);
                }
                else
                {
                    result = GetDataLabel(input, temp);
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

            return result;
        }


        private Label GetDMSLabel(string input, List<string> temp)
        {
            var transactionDMS = "Admission No";
            var label = new Label();
            //var transactionNumber = Regex.Match(input, _patternConfig.Transaction).ToString();
            
            var dmslabel = Regex.Match(input, _patternConfig.DMSKeyword);
            var dmscode = Regex.Match(input, _patternConfig.DMSCode).ToString();
            var indexTrNumber = temp.ToList().FindIndex(x => Regex.IsMatch(x, transactionDMS));
            var indexDmsLabel = temp.ToList().FindIndex(x => Regex.IsMatch(x, _patternConfig.DMSKeyword));
            var transactionNumber = temp[indexTrNumber].Split(':').ToArray()[1].Trim();
            var wordList = new List<string>();

            for (int i = indexDmsLabel + 1; i < indexTrNumber; i++)
            {
                wordList.Add(temp.ToList()[i]);
            }

            wordList.Remove(dmscode);

            var documentType = string.Join(" ", wordList.ToArray());
            var documentClean = RemoveIllegalFileNameChars(documentType, "");

            

            label.AdmissionNo = transactionNumber;
            label.DocumentType = _documentTypeParser.GetDocumentTypeDMS(dmscode);
            label.DMSCode = dmscode;

            return label;
        }

        public static string RemoveIllegalFileNameChars(string input, string replacement = "")
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(input, replacement);
        }

        private Label GetPatientLabel(string input, List<string> temp)
        {
            //TODO try catch
            var label = new Label();

            try
            {
                var isTransactionNo = Regex.Match(input, _patternConfig.Transaction);
                var invoiceNo = temp.FirstOrDefault(x => Regex.IsMatch(x, _patternConfig.Invoice, _options)) ?? null;

                if (isTransactionNo.Success)
                {
                    label.AdmissionNo = isTransactionNo.Value;
                }

                if (invoiceNo != null)
                {
                    invoiceNo = invoiceNo.Split(' ').ToList().FirstOrDefault(x => Regex.IsMatch(x, _patternConfig.Invoice, _options));
                    label.InvoiceNo = invoiceNo;
                }

                label.DocumentType = _documentTypeParser.GetDocumentTypePatient(input);

                return label;
            }
            catch (NullReferenceException nullEx)
            {

                _logger.LogError(nullEx.Message);
            }

            return label;
        }

        private Label GetDataLabel(string input, List<string> temp)
        {
            //TODO try catch
            var label = new Label();

            try
            {
                var isTransactionNo = Regex.Match(input, _patternConfig.Transaction);
                var invoiceNo = temp.FirstOrDefault(x => Regex.IsMatch(x, _patternConfig.Invoice, _options)) ?? null;

                if (isTransactionNo.Success)
                {
                    label.AdmissionNo = isTransactionNo.Value;
                }

                if (invoiceNo != null)
                {
                    invoiceNo = invoiceNo.Split(' ').ToList().FirstOrDefault(x => Regex.IsMatch(x, _patternConfig.Invoice, _options));
                    label.InvoiceNo = invoiceNo;
                }

                label.DocumentType = _documentTypeParser.GetDocumentTypeNonLabel(input);

                return label;
            }
            catch (NullReferenceException nullEx)
            {

                _logger.LogError(nullEx.Message);
            }

            return label;
        }

    }
}
