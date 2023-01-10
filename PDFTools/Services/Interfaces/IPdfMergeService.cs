namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    internal interface IPdfMergeService
    {
        /// <summary>
        /// Merges PDFs in order they were passed. 
        /// </summary>
        /// <returns></returns>
        ServiceResponse<string> MergePdfs(IEnumerable<string> inputPdfPaths);
    }
}
