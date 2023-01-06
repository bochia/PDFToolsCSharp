namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    public interface ISplitRangeParser
    {
        /// <summary>
        /// Tries to parse ranges from strings.
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        ServiceResponse<IEnumerable<SplitRange>> ParseRangesFromString(string ranges);
    }
}
