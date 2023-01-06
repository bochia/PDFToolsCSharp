namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    internal interface ISplitRangeValidator
    {
        /// <summary>
        /// Validates that passed string represents a valid set of ranges.
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        ServiceResponse ValidateSplitRanges(string ranges);
    }
}
