namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IPdfMergeService
    {
        /// <summary>
        /// Merges PDFs in order they were passed. 
        /// </summary>
        /// <returns>
        /// Returns path to resulting PDF.
        /// </returns>
        ServiceResponse<string> MergePdfs(IEnumerable<string> inputPdfPaths, string outputFolderPath);
    }
}
