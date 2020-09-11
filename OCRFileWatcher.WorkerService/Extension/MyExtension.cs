using Newtonsoft.Json;
using OCRFileWatcher.WorkerService.Model;
using System.Collections.Generic;

namespace OCRFileWatcher.WorkerService.Extension
{
    public static class MyExtension
    {
        public static string JsonExtension(this DocumentMetadata documentMetadata)
        {
            string metadataJson = JsonConvert.SerializeObject(documentMetadata, Formatting.Indented,
                                            new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore,
                                            });

            return metadataJson;
        }

        public static string JsonExtension(this FinalMetadata finalMetadata)
        {
            string metadataJson = JsonConvert.SerializeObject(finalMetadata, Formatting.Indented,
                                            new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore,
                                            });

            return metadataJson;
        }

        public static Dictionary<string, List<int>> ToDictionary(this IEnumerable<DocumentType> documentTypes)
        {
            Dictionary<string, List<int>> output = new Dictionary<string, List<int>>();
            string currentType = string.Empty;

            foreach (var documentType in documentTypes)
            {

                if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType)
                {
                    currentType = documentType.Type;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<int>());
                    }
                }


                output[currentType].Add(documentType.Page);
            }

            return output;
        }

        public static Dictionary<string, List<DocumentType>> ToDictionaryX(this IEnumerable<DocumentType> documentTypes)
        {
            var output = new Dictionary<string, List<DocumentType>>();
            string currentType = string.Empty;
            string currentAdmission = string.Empty;

            foreach (var documentType in documentTypes)
            {

                if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType && documentType.AdmissionNo == currentAdmission)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }
                else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }
                else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type == currentType && documentType.AdmissionNo != currentAdmission)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;

                    output.Add(currentType, new List<DocumentType>());
                }


                output[currentType].Add(documentType);
            }

            return output;
        }



        public static Dictionary<string, List<DocumentType>> ToDictionaryAll(this IEnumerable<DocumentType> documentTypes)
        {
            var output = new Dictionary<string, List<DocumentType>>();
            string currentType = string.Empty;
            string currentAdmission = string.Empty;
            int currentPage = 0;

            foreach (var documentType in documentTypes)
            {

                if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType && documentType.AdmissionNo == currentAdmission)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;
                    currentPage = documentType.Page;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }
                else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type != currentType)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;
                    currentPage = documentType.Page;

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }
                else if (documentType.Type != "OTHER DOCUMENT" && documentType.Type == currentType && documentType.AdmissionNo != currentAdmission)
                {
                    currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                    currentAdmission = documentType.AdmissionNo;
                    currentPage = documentType.Page;

                    output.Add(currentType, new List<DocumentType>());
                }
                else if (documentType.Type == "OTHER DOCUMENT" && (currentType == "OTHER DOCUMENT" || currentType != "OTHER DOCUMENT") && documentType.AdmissionNo == null)
                {
                    if (documentType.AdmissionNo != null)
                    {
                        currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                        currentAdmission = documentType.AdmissionNo;
                        currentPage = documentType.Page;
                    }

                    if (string.IsNullOrEmpty(currentType))
                    {
                        currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                        currentAdmission = documentType.AdmissionNo;
                        currentPage = documentType.Page;
                    }

                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }

                }
                else if (documentType.Type == "OTHER DOCUMENT" && documentType.AdmissionNo != currentAdmission && documentType.Page > currentPage)
                {
                    if (documentType.AdmissionNo != null)
                    {
                        currentType = string.Join('_', documentType.AdmissionNo, documentType.Type);
                        currentAdmission = documentType.AdmissionNo;
                        currentPage = documentType.Page;
                    }


                    if (!output.ContainsKey(currentType))
                    {
                        output.Add(currentType, new List<DocumentType>());
                    }
                }


                output[currentType].Add(documentType);
            }

            return output;
        }

    }
}
