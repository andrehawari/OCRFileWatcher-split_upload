namespace OCRFileWatcher.WorkerService.Processing
{
    public interface IDocumentTypeParser
    {
        string GetDocumentTypePatient(string input);
        string GetDocumentTypeNonLabel(string input);

        string GetDocumentTypeDMS(string input);
    }
}