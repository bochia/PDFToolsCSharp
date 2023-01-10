namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IPdfSplitService
    {
        /// <summary>
        /// Splits PDF according to provided interval using file storage.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="interval"></param>
        /// <returns>
        /// Returns paths to output PDFs.
        /// </returns>
        ServiceResponse<IEnumerable<string>> SplitByInterval(string inputPdfPath, int interval, string outputFolderPath);

        /// <summary>
        /// Splits PDF according to provided ranges using file storage.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="ranges"></param>
        /// <returns>
        /// Returns paths to output PDFs.
        /// </returns>
        ServiceResponse<IEnumerable<string>> SplitByRanges(string inputPdfPath, string outputFolderPathstring, string ranges);
    }
}
