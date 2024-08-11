namespace PDFTools.Services
{
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System;
    using System.Collections.Generic;

    public class SplitRangeParser : ISplitRangeParser
    {
        private const char Comma = ',';
        private const char Hyphen = '-';


        //TODO: Invalid inputs should throw errors. Atleast that's what the seniors said.
        /// <inheritdoc />
        public Attempt<IEnumerable<SplitRange>> GenerateRangesFromInterval(int interval, int pdfPageCount)
        {
            if (interval <= 0)
            {
                return new Attempt<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"{nameof(interval)} must be greater than 0."
                };
            }

            if (pdfPageCount <= 0)
            {
                return new Attempt<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"{nameof(pdfPageCount)} must be greater than 0."
                };
            }

            // create ranges
            List<SplitRange> ranges = new List<SplitRange>();

            int startPageNumber = 1;
            int endPageNumber = interval;
            while (endPageNumber <= pdfPageCount)
            {
                ranges.Add(new SplitRange(startPageNumber, endPageNumber));
                startPageNumber = endPageNumber + 1;
                endPageNumber = startPageNumber + interval - 1;
            }

            if (startPageNumber < pdfPageCount && endPageNumber > pdfPageCount)
            {
                ranges.Add(new SplitRange(startPageNumber, pdfPageCount));
            }

            return new Attempt<IEnumerable<SplitRange>>() 
            {
                Success = true,
                Data = ranges,
            };
        }

        /// <inheritdoc />
        public Attempt<IEnumerable<SplitRange>> ParseRangesFromString(string ranges)
        {
            const string inputName = nameof(ranges);

            if (string.IsNullOrWhiteSpace(ranges))
            {
                return new Attempt<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"{inputName} cannt be null or whitespace."
                };
            }

            List<SplitRange> rangesList = new List<SplitRange>();

            try
            {
                ranges = RemoveIrrelevantCharacters(ranges);

                Attempt validCharactersAttempt = CheckOnlyHasAllowedCharacters(ranges);

                if (!validCharactersAttempt.Success)
                {
                    return new Attempt<IEnumerable<SplitRange>>()
                    {
                        ErrorMessage = validCharactersAttempt.ErrorMessage
                    };
                }

                string[] rangesSplit = ranges.Split(Comma);

                foreach (string range in rangesSplit)
                {
                    SplitRange splitRange = CreateSplitRangeFromStringRange(range);
                    rangesList.Add(splitRange);
                }
            }
            catch (Exception ex)
            {
                return new Attempt<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"Failed to parse ranges from string: {ranges} - {ex.Message}"
                };
            }

            return new Attempt<IEnumerable<SplitRange>>()
            {
                Success = true,
                Data = rangesList
            };

        }

        /// <summary>
        /// Checks that input only containts numbers, hyphens, and commas.
        /// Example valid range 1-5,9-25,60-90.
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        private Attempt CheckOnlyHasAllowedCharacters(string ranges)
        {
            foreach (char character in ranges)
            {
                if (!char.IsDigit(character) && character != Hyphen && character != Comma)
                {
                    return new Attempt()
                    {
                        ErrorMessage = $"The character '{character}' is not permitted in a valid range input.",
                    };
                }
            }

            return new Attempt()
            {
                Success = true,
            };
        }

        private int ConvertStringToInt(string stringNumber)
        {
            int number;

            if (!int.TryParse(stringNumber, out number))
            {
                throw new Exception($"Failed to parse ranges - {stringNumber} couldn't be converted to an integer.");
            }

            return number;
        }

        private SplitRange CreateSplitRangeFromStringRange(string range)
        {
            SplitRange splitRange;

            //check if range is single number or has a start and end number.
            if (range.Contains(Hyphen))
            {
                splitRange = CreateSplitRangeFromFullRangeString(range);
            }
            else
            {
                splitRange = CreateSplitRangeFromSingleNumberString(range);
            }

            return splitRange;
        }

        private SplitRange CreateSplitRangeFromFullRangeString(string range)
        {
            string[] startAndEndNumbers = range.Split(Hyphen);
            string startNumberString = startAndEndNumbers[0];
            string endNumberString = startAndEndNumbers[1];

            int startPageNumber = ConvertStringToInt(startNumberString);

            int endPageNumber = ConvertStringToInt(endNumberString);

            SplitRange splitRange = new SplitRange(startPageNumber, endPageNumber);

            return splitRange;
        }

        private SplitRange CreateSplitRangeFromSingleNumberString(string singleNumberRange)
        {
            int startPageNumber = ConvertStringToInt(singleNumberRange);
            SplitRange splitRange = new SplitRange(startPageNumber, startPageNumber);

            return splitRange;
        }

        /// <summary>
        /// Clean the input string of any obviously irrelevant characters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveIrrelevantCharacters(string ranges)
        {
            return ranges.Replace(" ", string.Empty);
        }
    }
}
