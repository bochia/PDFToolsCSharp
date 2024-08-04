namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IStoragePdfMerger
    {
        /// <summary>
        /// Merges PDFs in order they were passed using file storage.
        /// </summary>
        /// <returns>
        /// Returns path to resulting PDF.
        /// </returns>
        Attempt<string> Merge(IEnumerable<string> inputPdfPaths, string outputFolderPath);
    }
}
