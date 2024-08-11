namespace PDFTools.Tests
{
    using PDFTools.Models;
    using PDFTools.Services;
    using PDFTools.Services.Interfaces;
    using Xunit;

    public class SplitRangeParserTests
    {
        private ISplitRangeParser splitRangeParser;

        // Runs before every tests.
        public SplitRangeParserTests()
        {
            this.splitRangeParser = new SplitRangeParser();
        }

        [InlineData(-1 , 1, "interval must be greater than 0.")]
        [InlineData(0, 1, "interval must be greater than 0.")]
        [InlineData(1, -1, "pdfPageCount must be greater than 0.")]
        [InlineData(1, 0, "pdfPageCount must be greater than 0.")]
        [Theory]
        private void GenerateRangesFromInterval_InvalidInputs_ReturnsFailure(int interval, int pdfPageCount, string expectedErrorMessage)
        {
            // Arrange
            // Act
            Attempt<IEnumerable<SplitRange>> result = splitRangeParser.GenerateRangesFromInterval(interval, pdfPageCount);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        }

        [Fact]
        private void GenerateRangesFromInterval_HappyPath_ReturnsRanges()
        {
            // Arrange
            int interval = 5;
            int pdfPageCount = 11;

            // Act
            Attempt<IEnumerable<SplitRange>> result = splitRangeParser.GenerateRangesFromInterval(interval, pdfPageCount);

            // Assert
            Assert.True(result.Success);
            List<SplitRange> ranges = result.Data.ToList();
            Assert.Equal(3, ranges.Count());
            Assert.Equal(1, ranges[0].StartPageNumber);
            Assert.Equal(5, ranges[0].EndPageNumber);
            Assert.Equal(6, ranges[1].StartPageNumber);
            Assert.Equal(10, ranges[1].EndPageNumber);
            Assert.Equal(11, ranges[2].StartPageNumber);
        }
    }
}
