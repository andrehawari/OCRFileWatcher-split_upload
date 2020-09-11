using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OCRFileWatcher.WorkerService.Config;
using OCRFileWatcher.WorkerService.Extension;
using OCRFileWatcher.WorkerService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OCRFileWatcher.WorkerService.Processing
{
    public class FileProcessor : IFileProcessor
    {
        private readonly FolderConfig _folderConfig;
        private readonly ILogger<FileProcessor> _logger;


        public FileProcessor(IOptions<FolderConfig> folderConfig, ILogger<FileProcessor> logger)
        {
            _folderConfig = folderConfig.Value;
            _logger = logger;
        }


        public async Task<string> MoveFile(string source)
        {
            var fileName = Path.GetFileNameWithoutExtension(source);
            var fileExtension = Path.GetExtension(source);
            var suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var temp = string.Join('_', fileName, suffix);
            var newFileName = temp + fileExtension;

            var dest = Path.Combine(_folderConfig.InputCopy, newFileName);

            try
            {
                using (FileStream sourceStream = File.Open(source, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(dest))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                        sourceStream.Close();
                        File.Delete(source);

                        return dest;
                    }
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine("An IOException occured during move, " + ioex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Exception occured during move, " + ex.Message);
            }
            return null;
        }


        public string RunPDFToImage(string source)
        {
            var dest = _folderConfig.PdfImages;
            //char[] separator = { Path.DirectorySeparatorChar, '.' };
            var resultFolder = Path.Combine(dest, Path.GetFileNameWithoutExtension(source));
            var tempFilename = Path.Combine(resultFolder, Path.GetFileNameWithoutExtension(source));

            if (!Directory.Exists(resultFolder))
            {
                Directory.CreateDirectory(resultFolder);
            }

            ProcessStartInfo pdfToImage = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = String.Format(@"/c pdftoppm ""{0}"" ""{1}"" -jpeg", source, tempFilename),
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            Process pdfToImg = Process.Start(pdfToImage);
            var messageFromCommandLine = pdfToImg.StandardOutput.ReadToEnd();
            (messageFromCommandLine.Any() ? new Action(() => _logger.LogWarning(messageFromCommandLine)) : () => _logger.LogInformation("Run"))();
            _logger.LogInformation("Convert done");

            var result = resultFolder;
            return result;
        }

        public List<FinalMetadata> SplitPDF(DocumentMetadata documentMetadata, string originFile)
        {
            //Parsing data buat ambil range page tiap jenis dokumen
            //var pageRange = documentMetadata.DocumentTypes.ToDictionary(); bakal dihapus

            //Test metdos
            var testexten = documentMetadata.DocumentTypes.ToDictionaryAll();

            //test
            //var testexten = documentMetadata.DocumentTypes.ToDictionaryX();



            // bikin list kosong buat nampung
            var result = new List<FinalMetadata>();

            var resultFolder = Path.Combine(_folderConfig.SplitPdf, Path.GetFileNameWithoutExtension(documentMetadata.FileName));


            if (!Directory.Exists(resultFolder))
            {
                Directory.CreateDirectory(resultFolder);
            }

            foreach (var item in testexten)
            {
                var outputName = item.Key + ".pdf";
                var outputFolder = Path.Combine(resultFolder, outputName);
                var semuaPages = item.Value.Select(x => x.Page);
                var admision = item.Value.Select(x => x.AdmissionNo).FirstOrDefault(); 
                var invoiceNo = item.Value.Select(x => x.InvoiceNo).FirstOrDefault(); 

                ProcessStartInfo pdftkSplit = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = String.Format(@"/c pdftk ""{0}"" cat {1} output ""{2}""",
                                                originFile, String.Join(' ', semuaPages), outputFolder),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

                Process pdfSplit = Process.Start(pdftkSplit);
                var messageFromCommandLine = pdfSplit.StandardOutput.ReadToEnd();
                (messageFromCommandLine.Any() ? new Action(() => _logger.LogWarning(messageFromCommandLine)) : () => _logger.LogInformation("Run"))();
                _logger.LogInformation("dokumen split done");

                //kasih return metadata. bikin disini, jenis return finalMetadata
                var finalMetadata = new FinalMetadata()
                {
                    DateInsert = documentMetadata.DateInsert,
                    FileName = outputName,
                    AdmissionNo = admision,
                    InvoiceNo = invoiceNo,
                    LocalFilePath = outputFolder,
                    DocumentType = item.Key.Split('_').ToList()[1],
                    TotalPages = item.Value.Count(),
                    Unit = documentMetadata.Unit
                };

                result.Add(finalMetadata);
            }

            return result;
        }

        public void CopyToOutput(DocumentMetadata documentMetadata, List<FinalMetadata> originFiles)
        {
            try
            {
                var outputFolder = _folderConfig.Output;

                foreach (var item in originFiles)
                {
                    var adm = item.AdmissionNo ?? "";
                    var destFolder = Path.Combine(outputFolder, "AdmissionNo", adm);
                    var filename = Path.GetFileName(item.LocalFilePath);
                    var outputFileName = string.Join("_", item.Unit, filename);
                    var outputFilePath = Path.Combine(destFolder, outputFileName);

                    if (string.IsNullOrEmpty(adm))
                    {
                        var date = DateTime.ParseExact(item.DateInsert, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        var suffix = date.ToString("yyyyMMdd_HHmmss");

                        destFolder = Path.Combine(outputFolder, "UNKOWN");
                        filename = Path.GetFileNameWithoutExtension(item.LocalFilePath);
                        outputFileName = string.Join("_", item.Unit, filename, suffix);
                        outputFilePath = Path.Combine(destFolder, outputFileName+".pdf");

                        Directory.CreateDirectory(destFolder);

                        if (!Directory.Exists(destFolder))
                        {
                            Directory.CreateDirectory(destFolder);
                        }
                    }

                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    File.Copy(item.LocalFilePath, outputFilePath, true);

                    item.LocalFilePath = outputFilePath;

                    //Save Json File
                    MetadataToOutput(item, outputFilePath);

                    //disini nanti kasih return full collection metadata
                    _logger.LogInformation(Path.Combine(destFolder, outputFilePath));

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// original method
        /// </summary>
        /// <param name="documentMetadata"></param>
        /// <param name="destinationPath"></param>
        //public void CopyToOutput(DocumentMetadata documentMetadata, List<FinalMetadata> originFiles)
        //{
        //    //TODO SPLIT FOLDER BY ADMISSION NUMBER DULU GAN,
        //    //ABIS ITU BARU BIKIN COPY FILENYA MASUK
        //    //DIDALEM TRY MUSTI DIGANTI SEMUA WKWAKKWAKWAKAW
        //    try
        //    {
        //        var allAdmissionNo = documentMetadata.DocumentTypes
        //            .Where(x => x.AdmissionNo != null)
        //            .Select(x => x.AdmissionNo).Distinct().ToList();

        //        var folderAdmission = string.Join("_", allAdmissionNo);

        //        var outputFolder = _folderConfig.Output;

        //        var destFolder = Path.Combine(outputFolder, "AdmissionNo", folderAdmission);

        //        var outputFileName = string.Join("_", documentMetadata.Unit);

        //        var outputFilePath = Path.Combine(destFolder, outputFileName);


        //        if (!allAdmissionNo.Any())
        //        {
        //            destFolder = Path.Combine(outputFolder, "UNKOWN");
        //            outputFilePath = Path.Combine(destFolder, outputFileName);
        //            Directory.CreateDirectory(destFolder);

        //            if (!Directory.Exists(destFolder))
        //            {
        //                Directory.CreateDirectory(destFolder);
        //            }
        //        }

        //        if (!Directory.Exists(destFolder))
        //        {
        //            Directory.CreateDirectory(destFolder);
        //        }

        //        foreach (var file in originFiles)
        //        {
        //            var filename = Path.GetFileName(file.LocalFilePath);
        //            outputFileName = string.Join("_", documentMetadata.Unit, filename);
        //            outputFilePath = Path.Combine(destFolder, outputFileName);
        //            File.Copy(file.LocalFilePath, outputFilePath, true);

        //            //Save Json File
        //            MetadataToOutput(documentMetadata, outputFilePath);

        //            //disini nanti kasih return full collection metadata
        //            _logger.LogInformation(Path.Combine(destFolder, outputFilePath));

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        throw ex;
        //    }
        //}

        private void MetadataToOutput(FinalMetadata finalMetadata, string destinationPath)
        {
            using (StreamWriter file = new StreamWriter(destinationPath + ".txt"))
            {
                file.Write(finalMetadata.JsonExtension());
            }
                
        }

    }
}
