namespace PDFTools.Services.Interfaces
{
    using PDFTools.Models;

    internal interface ISplitRangeValidator
    {
        ServiceResponse ValidateSplitRanges(string ranges);
    }
}
