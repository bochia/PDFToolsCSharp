namespace PDFTools.Services
{
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    public class SplitRangeParser : ISplitRangeParser
    {
        private const char Comma = ',';
        private const char Hyphen = '-';

        //TODO: Need to unit test this method.
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

        //TODO: Quick glance this method is too long. Probably needs to be refactored. Make unit tests for it first before doing that.
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
                    SplitRange splitRange;

                    //check if range is single number or has a start and end number.
                    if (range.Contains(Hyphen))
                    {
                        string[] startAndEndNumbers = range.Split(Hyphen);

                        Attempt<int> startPageNumberAttempt = ConvertStringToInt(startAndEndNumbers[0]);

                        if (!startPageNumberAttempt.Success)
                        {
                            return new Attempt<IEnumerable<SplitRange>>()
                            {
                                ErrorMessage = startPageNumberAttempt.ErrorMessage
                            };
                        }

                        Attempt<int> endPageNumberAttempt = ConvertStringToInt(startAndEndNumbers[1]);

                        if (!endPageNumberAttempt.Success)
                        {
                            return new Attempt<IEnumerable<SplitRange>>()
                            {
                                ErrorMessage = endPageNumberAttempt.ErrorMessage
                            };
                        }

                        splitRange = new SplitRange(startPageNumberAttempt.Data,
                                                      endPageNumberAttempt.Data);
                    }
                    else
                    {
                        string rangeWithSingleNumber = range;
                        Attempt<int> startPageNumberAttempt = ConvertStringToInt(rangeWithSingleNumber);

                        if (!startPageNumberAttempt.Success)
                        {
                            return new Attempt<IEnumerable<SplitRange>>()
                            {
                                ErrorMessage = startPageNumberAttempt.ErrorMessage
                            };
                        }

                        splitRange = new SplitRange(startPageNumberAttempt.Data,
                                                    startPageNumberAttempt.Data);

                    }

                    rangesList.Add(splitRange);
                }
            }
            catch (Exception ex)
            {
                return new Attempt<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"Failed to parse ranges from string - {ex.Message}"
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

        private Attempt<int> ConvertStringToInt(string stringNumber)
        {
            int number;

            if (!int.TryParse(stringNumber, out number))
            {
                return new Attempt<int>()
                {
                    ErrorMessage = $"Failed to parse ranges - {stringNumber} couldn't be converted to an integer."
                };
            }

            return new Attempt<int>()
            {
                Success = true,
                Data = number
            };
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
