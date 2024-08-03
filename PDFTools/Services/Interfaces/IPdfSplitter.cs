namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IPdfSplitter
    {
        /// <summary>
        /// Splits PDF according to provided interval using file storage.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="interval"></param>
        /// <returns>
        /// Returns output PDFs as Streams.
        /// </returns>
        ServiceResponse<IEnumerable<string>> SplitByInterval(string inputPdfPath, int interval, string outputFolderPath);

        /// <summary>
        /// Splits PDF according to provided interval and occurs entirely in memory.
        /// </summary>
        /// <param name="inputPdfStream"></param>
        /// <param name="interval"></param>
        /// <returns>
        /// Returns paths to output PDFs.
        /// </returns>
        ServiceResponse<IEnumerable<Stream>> SplitByIntervalInMemory(Stream inputPdfStream, int interval);

        /// <summary>
        /// Splits PDF according to provided ranges using file storage.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="ranges"></param>
        /// <returns>
        /// Returns paths to output PDFs.
        /// </returns>
        ServiceResponse<IEnumerable<string>> SplitByRanges(string inputPdfPath, string outputFolderPathstring, string ranges);

        /// <summary>
        /// Splits PDF according to provided ranges and occurs entirely in memory.
        /// </summary>
        /// <param name="inputPdfStream"></param>
        /// <param name="ranges"></param>
        /// <returns>
        /// Returns output PDFs as Streams.
        /// </returns>
        ServiceResponse<IEnumerable<Stream>> SplitByRangesInMemory(Stream inputPdfStream, string ranges);
    }
}
