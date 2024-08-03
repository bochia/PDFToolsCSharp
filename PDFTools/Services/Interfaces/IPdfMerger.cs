namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IPdfMerger
    {
        /// <summary>
        /// Merges PDFs in order they were passed using file storage.
        /// </summary>
        /// <returns>
        /// Returns path to resulting PDF.
        /// </returns>
        ServiceResponse<string> MergePdfs(IEnumerable<string> inputPdfPaths, string outputFolderPath);

        /// <summary>
        /// Merges PDFs in order they were passed, merged in memory.
        /// </summary>
        /// <returns>
        /// Returns Steam of PDF.
        /// </returns>
        ServiceResponse<Stream> MergePdfsInMemory(IEnumerable<Stream> inputPdfStreams);
    }
}
