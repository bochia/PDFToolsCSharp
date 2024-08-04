namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IStoragePdfSplitter
    {
        /// <summary>
        /// Splits PDF according to provided interval using file storage.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="interval"></param>
        /// <returns>
        /// Returns output PDFs as Streams.
        /// </returns>
        Attempt<IEnumerable<string>> SplitByInterval(string inputPdfPath, int interval, string outputFolderPath);

        /// <summary>
        /// Splits PDF according to provided ranges using file storage.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="ranges"></param>
        /// <returns>
        /// Returns paths to output PDFs.
        /// </returns>
        Attempt<IEnumerable<string>> SplitByRanges(string inputPdfPath, string outputFolderPathstring, string ranges);
    }
}
