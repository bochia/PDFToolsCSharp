namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IMemoryPdfSplitter
    {
        /// <summary>
        /// Splits PDF according to provided interval and occurs entirely in memory.
        /// </summary>
        /// <param name="inputPdfStream"></param>
        /// <param name="interval"></param>
        /// <returns>
        /// Returns paths to output PDFs.
        /// </returns>
        Attempt<IEnumerable<Stream>> SplitByInterval(Stream inputPdfStream, int interval);

        /// <summary>
        /// Splits PDF according to provided ranges and occurs entirely in memory.
        /// </summary>
        /// <param name="inputPdfStream"></param>
        /// <param name="ranges"></param>
        /// <returns>
        /// Returns output PDFs as Streams.
        /// </returns>
        Attempt<IEnumerable<Stream>> SplitByRanges(Stream inputPdfStream, string ranges);
    }
}
