namespace PDFTools.Services
{
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;

    public class SplitRangeValidator : ISplitRangeValidator
    {
        /// <inheritdoc />
        public ServiceResponse ValidateSplitRanges(string ranges)
        {
            throw new NotImplementedException();
            //TODO:
            //maybe before string passed you should remove all whitespaces from the ranges.
            //Needs to validate that string is not null, empty, or whitespace.
            //Needs to validate string only has expected characters.
            //validate that each range starts with a number smaller than the end number in the range. 
            //validate that a range has larger numbers than the last range. 
        }
    }
}
