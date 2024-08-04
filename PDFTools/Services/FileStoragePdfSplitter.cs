namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// PDF splitter that uses file storage in order to split PDFs.
    /// </summary>
    public class FileStoragePdfSplitter : PdfSplitterBase, IStoragePdfSplitter
    {
        private const string PdfFileExtension = ".pdf";
        private readonly ISplitRangeParser splitRangeParser;

        public FileStoragePdfSplitter(ISplitRangeParser splitRangeParser)
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

            ServiceResponse<IEnumerable<SplitRange>> rangesResponse = splitRangeParser.GenerateRangesFromInterval(interval, pdfResponse.Data.PageCount);
            if (!rangesResponse.Success)
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = rangesResponse.ErrorMessage
                };
            }

            if (rangesResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(rangesResponse)}.Data cannot be null."
                };
            }

            return SplitByRanges(pdfResponse.Data, outputFolderPath, rangesResponse.Data);
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
                string inputPdfName = Path.GetFileNameWithoutExtension(inputPdf.FullPath);

                foreach (SplitRange range in ranges)
                {
                    PdfDocument outputPdf = CreateNewPdfDocumentFromRange(inputPdf, range, inputPdfName);

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
    }
}
