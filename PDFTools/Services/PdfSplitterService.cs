namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    public class PdfSplitterService : IPdfSplitterService
    {
        private readonly ISplitRangeParser splitRangeParser;

        public PdfSplitterService(ISplitRangeParser splitRangeParser)
        {
            this.splitRangeParser = splitRangeParser;
        }

        /// <inheritdoc />
        public ServiceResponse<string> SplitByInterval(string inputPdfPath, int interval)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ServiceResponse<string> SplitByRanges(string inputPdfPath, string ranges)
        {
            if (string.IsNullOrWhiteSpace(inputPdfPath))
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"{nameof(inputPdfPath)} cannot be null or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(ranges))
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"{nameof(ranges)} cannt be null or whitespace."
                };
            }

            PdfDocument inputPdf;
            try
            {
                inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import); //TODO: how do we stop this from memory leaking?

                if (inputPdf == null)
                {
                    return new ServiceResponse<string>()
                    {
                        ErrorMessage = "Opened PDF was null."
                    };
                }
            }
            catch (Exception ex)
            {
                //TODO: Add logging that includes pdf path here.
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"Failed to open PDF - {ex.Message}"
                };
            }

            ServiceResponse<IEnumerable<SplitRange>> parseRangesResponse = splitRangeParser.ParseRangesFromString(ranges);

            if (!parseRangesResponse.Success || parseRangesResponse.Data == null)
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = parseRangesResponse.ErrorMessage
                };
            }

            return SplitByRanges(inputPdf, parseRangesResponse.Data);

        }


        private ServiceResponse<string> SplitByRanges(PdfDocument inputPdf, IEnumerable<SplitRange> ranges)
        {
            if (inputPdf == null)
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"{nameof(inputPdf)} cannot be null."
                };
            }

            if (ranges == null || !ranges.Any())
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"{nameof(ranges)} cannot be null or empty."
                };
            }

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

                    outputPdf.Save(@$"C:\Users\John\Downloads\{outputPdf.Info.Title}.pdf"); //TODO: need to change this path.
                }
            }
            catch (Exception ex)
            {

                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"Failed to split PDF - {ex.Message}"
                };
            }


            return new ServiceResponse<string>()
            {
                Success = true,
                Data = "ochia"
            };
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

        private string ZipOutputPdfsTogether()
        {
            throw new NotImplementedException();
        }
    }
}
