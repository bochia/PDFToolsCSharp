namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    public class PdfSplitService : IPdfSplitService
    {
        private const string PdfFileExtension = ".pdf";
        private readonly ISplitRangeParser splitRangeParser;

        public PdfSplitService(ISplitRangeParser splitRangeParser)
        {
            this.splitRangeParser = splitRangeParser;
        }

        /// <inheritdoc />
        public ServiceResponse<IEnumerable<string>> SplitByInterval(string inputPdfPath, int interval, string outputFolderPath)
        {
            if (string.IsNullOrWhiteSpace(inputPdfPath))
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(inputPdfPath)} cannot be null or whitespace."
                };
            }

            ServiceResponse<PdfDocument> pdfResponse = OpenPdf(inputPdfPath);
            if (!pdfResponse.Success || pdfResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = pdfResponse.ErrorMessage
                };
            }

            IEnumerable<SplitRange> ranges = GenerateRangesFromInterval(interval, pdfResponse.Data.PageCount);

            return SplitByRanges(pdfResponse.Data, outputFolderPath, ranges);
        }

        /// <inheritdoc />
        public ServiceResponse<IEnumerable<Stream>> SplitByIntervalInMemory(Stream inputPdfStream, int interval)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<SplitRange> GenerateRangesFromInterval(int interval, int pdfPageCount)
        {
            // create ranges
            List<SplitRange> ranges = new List<SplitRange>();

            int startPageNumber = 1;
            int endPageNumber = interval;
            while (endPageNumber < pdfPageCount)
            {
                ranges.Add(new SplitRange(startPageNumber, endPageNumber));
                startPageNumber = endPageNumber + 1;
                endPageNumber = startPageNumber + interval - 1;
            }

            if (startPageNumber < pdfPageCount && endPageNumber > pdfPageCount)
            {
                ranges.Add(new SplitRange(startPageNumber, pdfPageCount));
            }

            return ranges;
        }

        /// <inheritdoc />
        public ServiceResponse<IEnumerable<string>> SplitByRanges(string inputPdfPath, string outputFolderPath, string ranges)
        {
            if (string.IsNullOrWhiteSpace(inputPdfPath))
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(inputPdfPath)} cannot be null or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(outputFolderPath))
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(outputFolderPath)} cannot be null or whitespace."
                };
            }

            if (!Directory.Exists(outputFolderPath))
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"Directory doesn't exist at specified location: {outputFolderPath}."
                };
            }

            if (string.IsNullOrWhiteSpace(ranges))
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(ranges)} cannt be null or whitespace."
                };
            }

            ServiceResponse<PdfDocument> pdfResponse = OpenPdf(inputPdfPath);
            if (!pdfResponse.Success || pdfResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = pdfResponse.ErrorMessage
                };
            }

            ServiceResponse<IEnumerable<SplitRange>> parseRangesResponse = splitRangeParser.ParseRangesFromString(ranges);

            if (!parseRangesResponse.Success || parseRangesResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = parseRangesResponse.ErrorMessage
                };
            }

            return SplitByRanges(pdfResponse.Data, outputFolderPath, parseRangesResponse.Data);

        }

        private ServiceResponse<IEnumerable<string>> SplitByRanges(PdfDocument inputPdf, string outputFolderPath, IEnumerable<SplitRange> ranges)
        {
            if (inputPdf == null)
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(inputPdf)} cannot be null."
                };
            }

            if (ranges == null || !ranges.Any())
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(ranges)} cannot be null or empty."
                };
            }

            List<string> outputPdfPaths = new List<string>();

            try
            {
                string pdfName = Path.GetFileNameWithoutExtension(inputPdf.FullPath);

                foreach (SplitRange range in ranges)
                {
                    PdfDocument outputPdf = new PdfDocument();
                    outputPdf.Version = inputPdf.Version;
                    outputPdf.Info.Title = CreateOutputPdfName(pdfName, range);
                    outputPdf.Info.CreationDate = DateTime.UtcNow;

                    for (int pageNumber = range.StartPageNumber; pageNumber <= range.EndPageNumber; pageNumber++)
                    {
                        outputPdf.AddPage(inputPdf.Pages[pageNumber - 1]); //PDFSharp uses zero based indexing for pages.
                    }

                    string outputPdfPath = $"{outputFolderPath}{outputPdf.Info.Title}{PdfFileExtension}";
                    outputPdf.Save(outputPdfPath);

                    outputPdfPaths.Add(outputPdfPath);
                }
            }
            catch (Exception ex)
            {

                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"Failed to split PDF - {ex.Message}"
                };
            }


            return new ServiceResponse<IEnumerable<string>>()
            {
                Success = true,
                Data = outputPdfPaths
            };
        }

        /// <inheritdoc />
        public ServiceResponse<IEnumerable<Stream>> SplitByRangesInMemory(Stream inputPdfStream, string ranges)
        {
            throw new NotImplementedException();
        }

        private string CreateOutputPdfName(string inputPdfName, SplitRange range)
        {
            string outputPdfName = $"{inputPdfName}_Page_{range.StartPageNumber}";

            if (!range.EndPageNumber.HasValue)
            {
                return outputPdfName;
            }

            return $"{outputPdfName}_to_{range.EndPageNumber}";
        }

        private ServiceResponse<PdfDocument> OpenPdf(string inputPdfPath)
        {

            try
            {
                PdfDocument inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import); //TODO: how do we stop this from memory leaking?

                if (inputPdf == null)
                {
                    return new ServiceResponse<PdfDocument>()
                    {
                        ErrorMessage = "Opened PDF was null."
                    };
                }

                return new ServiceResponse<PdfDocument>()
                {
                    Success = true,
                    Data = inputPdf
                };
            }
            catch (Exception ex)
            {
                //TODO: Add logging that includes pdf path here.
                return new ServiceResponse<PdfDocument>()
                {
                    ErrorMessage = $"Failed to open PDF - {ex.Message}"
                };
            }
        }

        private string ZipOutputPdfsTogether()
        {
            throw new NotImplementedException();
        }
    }
}
