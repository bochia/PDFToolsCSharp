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
        public ServiceResponse<IEnumerable<SplitRange>> GenerateRangesFromInterval(int interval, int pdfPageCount)
        {
            if (interval <= 0)
            {
                return new ServiceResponse<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"{nameof(interval)} must be greater than 0."
                };
            }

            if (pdfPageCount <= 0)
            {
                return new ServiceResponse<IEnumerable<SplitRange>>()
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

            return new ServiceResponse<IEnumerable<SplitRange>>() 
            {
                Success = true,
                Data = ranges,
            };
        }

        //TODO: Quick glance this method is too long. Probably needs to be refactored. Make unit tests for it first before doing that.
        /// <inheritdoc />
        public ServiceResponse<IEnumerable<SplitRange>> ParseRangesFromString(string ranges)
        {
            const string inputName = nameof(ranges);

            if (string.IsNullOrWhiteSpace(ranges))
            {
                return new ServiceResponse<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"{inputName} cannt be null or whitespace."
                };
            }

            List<SplitRange> rangesList = new List<SplitRange>();

            try
            {
                ranges = RemoveIrrelevantCharacters(ranges);

                ServiceResponse validCharactersResponse = CheckOnlyHasAllowedCharacters(ranges);

                if (!validCharactersResponse.Success)
                {
                    return new ServiceResponse<IEnumerable<SplitRange>>()
                    {
                        ErrorMessage = validCharactersResponse.ErrorMessage
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

                        ServiceResponse<int> startPageNumberResponse = ConvertStringToInt(startAndEndNumbers[0]);

                        if (!startPageNumberResponse.Success)
                        {
                            return new ServiceResponse<IEnumerable<SplitRange>>()
                            {
                                ErrorMessage = startPageNumberResponse.ErrorMessage
                            };
                        }

                        ServiceResponse<int> endPageNumberResponse = ConvertStringToInt(startAndEndNumbers[1]);

                        if (!endPageNumberResponse.Success)
                        {
                            return new ServiceResponse<IEnumerable<SplitRange>>()
                            {
                                ErrorMessage = endPageNumberResponse.ErrorMessage
                            };
                        }

                        splitRange = new SplitRange(startPageNumberResponse.Data,
                                                      endPageNumberResponse.Data);
                    }
                    else
                    {
                        string rangeWithSingleNumber = range;
                        ServiceResponse<int> startPageNumberResponse = ConvertStringToInt(rangeWithSingleNumber);

                        if (!startPageNumberResponse.Success)
                        {
                            return new ServiceResponse<IEnumerable<SplitRange>>()
                            {
                                ErrorMessage = startPageNumberResponse.ErrorMessage
                            };
                        }

                        splitRange = new SplitRange(startPageNumberResponse.Data,
                                                    startPageNumberResponse.Data);

                    }

                    rangesList.Add(splitRange);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<SplitRange>>()
                {
                    ErrorMessage = $"Failed to parse ranges from string - {ex.Message}"
                };
            }

            return new ServiceResponse<IEnumerable<SplitRange>>()
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
        private ServiceResponse CheckOnlyHasAllowedCharacters(string ranges)
        {
            foreach (char character in ranges)
            {
                if (!char.IsDigit(character) && character != Hyphen && character != Comma)
                {
                    return new ServiceResponse()
                    {
                        ErrorMessage = $"The character '{character}' is not permitted in a valid range input.",
                    };
                }
            }

            return new ServiceResponse()
            {
                Success = true,
            };
        }

        private ServiceResponse<int> ConvertStringToInt(string stringNumber)
        {
            int number;

            if (!int.TryParse(stringNumber, out number))
            {
                return new ServiceResponse<int>()
                {
                    ErrorMessage = $"Failed to parse ranges - {stringNumber} couldn't be converted to an integer."
                };
            }

            return new ServiceResponse<int>()
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
