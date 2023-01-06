namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface IPdfSplitterService
    {
        /// <summary>
        /// Returns path to zip file where resulting PDFs from split are stored.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        ServiceResponse<string> SplitByInterval(string inputPdfPath, int interval);

        /// <summary>
        /// Returns path to zip file where resulting PDFs from split are stored.
        /// </summary>
        /// <param name="inputPdfPath"></param>
        /// <param name="ranges"></param>
        /// <returns></returns>
        ServiceResponse<string> SplitByRanges(string inputPdfPath, string ranges);
    }
}
