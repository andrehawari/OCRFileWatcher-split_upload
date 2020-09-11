using ExcelDataReader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OCRFileWatcher.WorkerService.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace OCRFileWatcher.WorkerService.Processing
{
    public class DocumentTypeParser : IDocumentTypeParser
    {
        private readonly ExcelConfig _excelConfig;
        private readonly ILogger<DocumentTypeParser> _logger;

        public DocumentTypeParser(
            IOptions<ExcelConfig> excelConfig,
            ILogger<DocumentTypeParser> logger
            )
        {
            _excelConfig = excelConfig.Value;
            _logger = logger;
        }

        private DataTable Load(string sheetName)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = File.Open(_excelConfig.DocumentConfig, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var headers = new List<string>();

                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {

                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true,

                                ReadHeaderRow = rowReader => {
                                    for (var i = 0; i < rowReader.FieldCount; i++)

                                        headers.Add(Convert.ToString(rowReader.GetValue(i)));
                                },

                                FilterColumn = (columnReader, columnIndex) =>
                                    headers.IndexOf("string") != columnIndex
                            }
                        });

                        var docTypeTable = result.Tables[sheetName];

                        return docTypeTable;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
            
        }

        public string GetDocumentTypePatient(string input)
        {
            var dt = Load("Keywords");

            foreach (DataRow row in dt.Rows)
            {
                var res = row[1];
                var str = res.ToString().Trim().Split(',').ToArray();
                int totalKeyword = str.Length;

                var validationList = new List<bool>();
                foreach (var keyword in str)
                {
                    validationList.Add(input.Contains(keyword.Trim(), StringComparison.InvariantCultureIgnoreCase));
                }

                bool validate = validationList.All(v => v);

                if (!validate)
                {
                    continue;
                }

                return row[0].ToString().Trim();
            }

            return "OTHER-DOCUMENT";

        }

        public string GetDocumentTypeNonLabel(string input)
        {
            var dt = Load("Keywords");

            foreach (DataRow row in dt.Rows)
            {
                var res = row[1];
                var str = res.ToString().Trim().Split(',').ToArray();
                int totalKeyword = str.Length;

                var validationList = new List<bool>();
                foreach (var keyword in str)
                {
                    validationList.Add(input.Contains(keyword.Trim(), StringComparison.InvariantCultureIgnoreCase));
                }

                bool validate = validationList.All(v => v);

                if (!validate)
                {
                    continue;
                }

                return row[0].ToString().Trim();
            }

            return "OTHER DOCUMENT";

        }


        public string GetDocumentTypeDMS(string input)
        {
            var dt = Load("DMS");

            foreach (DataRow row in dt.Rows)
            {
                var res = row[0];

                if (input == res.ToString())
                {
                    return row[1].ToString().Trim();
                }
            }

            return null;
        }
    }
}
