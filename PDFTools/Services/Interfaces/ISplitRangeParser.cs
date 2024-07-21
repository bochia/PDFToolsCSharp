namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface ISplitRangeParser
    {
        ServiceResponse<IEnumerable<SplitRange>> GenerateRangesFromInterval(int interval, int pdfPageCount);

        /// <summary>
        /// Tries to parse ranges from strings.
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        ServiceResponse<IEnumerable<SplitRange>> ParseRangesFromString(string ranges);
    }
}
