namespace PDFTools.Models
{
    public class Range
    {
        public int StartPageNumber { get; }
        public int? EndPageNumber { get; }

        public Range(int startPageNumber, int endPageNumber)
        {
            StartPageNumber = startPageNumber;
            EndPageNumber = endPageNumber;
        }

        public override string ToString()
        {
            if (EndPageNumber.HasValue)
            {
                return $"{StartPageNumber}-{EndPageNumber}";
            }

            return StartPageNumber.ToString();
        }
    }
}
