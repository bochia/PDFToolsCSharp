namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IPdfMergeService
    {
        /// <summary>
        /// Merges PDFs in order they were passed using file storage.
        /// </summary>
        /// <returns>
        /// Returns path to resulting PDF.
        /// </returns>
        ServiceResponse<string> MergePdfs(IEnumerable<string> inputPdfPaths, string outputFolderPath);
    }
}
