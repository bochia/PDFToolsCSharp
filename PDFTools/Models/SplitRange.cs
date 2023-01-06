namespace PDFTools.Models
{
    public class SplitRange
    {
        public int StartPageNumber { get; }
        public int? EndPageNumber { get; }

        public SplitRange(int startPageNumber, int endPageNumber = 0)
        {
            if (endPageNumber < startPageNumber)
            {
                throw new ArgumentException("Invalid range - EndPageNumber must be >= StartPageNumber.");
            }

            if (startPageNumber <= 0)
            {
                startPageNumber = 1;
            }

            StartPageNumber = startPageNumber;

            if (endPageNumber != 0)
            {
                EndPageNumber = endPageNumber;
            }
            else
            {
                EndPageNumber = startPageNumber;
            }
        }

        public override string ToString()
        {
            if (EndPageNumber != StartPageNumber)
            {
                return $"{StartPageNumber}-{EndPageNumber}";
            }

            return StartPageNumber.ToString();
        }
    }
}
