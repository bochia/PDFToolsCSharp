namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IMemoryPdfMerger
    {
        /// <summary>
        /// Merges PDFs in order they were passed, merged in memory.
        /// </summary>
        /// <returns>
        /// Returns Steam of PDF.
        /// </returns>
        Attempt<Stream> Merge(IEnumerable<Stream> inputPdfStreams);
    }
}
