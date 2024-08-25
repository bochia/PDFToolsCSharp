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
        public Attempt<IEnumerable<string>> SplitByInterval(string inputPdfPath, int interval, string outputFolderPath)
        {
            if (string.IsNullOrWhiteSpace(inputPdfPath))
            {
                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(inputPdfPath)} cannot be null or whitespace."
                };
            }

            try
            {
                Attempt<PdfDocument> pdfAttempt = OpenPdf(inputPdfPath);
                if (!pdfAttempt.Success || pdfAttempt.Data == null)
                {
                    return new Attempt<IEnumerable<string>>()
                    {
                        ErrorMessage = pdfAttempt.ErrorMessage
                    };
                }

                using (PdfDocument openedPdf = pdfAttempt.Data)
                {
                    Attempt<IEnumerable<SplitRange>> rangesAttempt = splitRangeParser.GenerateRangesFromInterval(interval, pdfAttempt.Data.PageCount);
                    if (!rangesAttempt.Success)
                    {
                        return new Attempt<IEnumerable<string>>()
                        {
                            ErrorMessage = rangesAttempt.ErrorMessage
                        };
                    }

                    if (rangesAttempt.Data == null)
                    {
                        return new Attempt<IEnumerable<string>>()
                        {
                            ErrorMessage = $"{nameof(rangesAttempt)}.Data cannot be null."
                        };
                    }

                    return SplitByRanges(openedPdf, outputFolderPath, rangesAttempt.Data);
                }
            }
            catch (Exception ex)
            {

                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"Failed to split PDF by interval - {ex.Message}"
                };
            }
        }

        /// <inheritdoc />
        public Attempt<IEnumerable<string>> SplitByRanges(string inputPdfPath, string outputFolderPath, string ranges)
        {
            if (string.IsNullOrWhiteSpace(inputPdfPath))
            {
                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(inputPdfPath)} cannot be null or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(outputFolderPath))
            {
                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(outputFolderPath)} cannot be null or whitespace."
                };
            }

            if (!Directory.Exists(outputFolderPath))
            {
                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"Directory doesn't exist at specified location: {outputFolderPath}."
                };
            }

            if (string.IsNullOrWhiteSpace(ranges))
            {
                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(ranges)} cannt be null or whitespace."
                };
            }

            try
            {
                Attempt<PdfDocument> pdfAttempt = OpenPdf(inputPdfPath);
                if (!pdfAttempt.Success || pdfAttempt.Data == null)
                {
                    return new Attempt<IEnumerable<string>>()
                    {
                        ErrorMessage = pdfAttempt.ErrorMessage
                    };
                }

                using (PdfDocument openedPdf = pdfAttempt.Data)
                {
                    Attempt<IEnumerable<SplitRange>> parseRangesAttempt = splitRangeParser.ParseRangesFromString(ranges);

                    if (!parseRangesAttempt.Success || parseRangesAttempt.Data == null)
                    {
                        return new Attempt<IEnumerable<string>>()
                        {
                            ErrorMessage = parseRangesAttempt.ErrorMessage
                        };
                    }

                    return SplitByRanges(openedPdf, outputFolderPath, parseRangesAttempt.Data);
                }
            }
            catch (Exception ex)
            {

                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"Failed to split PDF by ranges - {ex.Message}"
                };
            }
        }

        private Attempt<IEnumerable<string>> SplitByRanges(PdfDocument inputPdf, string outputFolderPath, IEnumerable<SplitRange> ranges)
        {
            if (inputPdf == null)
            {
                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"{nameof(inputPdf)} cannot be null."
                };
            }

            if (ranges == null || !ranges.Any())
            {
                return new Attempt<IEnumerable<string>>()
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
                    using (PdfDocument outputPdf = CreateNewPdfDocumentFromRange(inputPdf, range, inputPdfName))
                    {

                        string outputPdfPath = $"{outputFolderPath}{outputPdf.Info.Title}{PdfFileExtension}";
                        outputPdf.Save(outputPdfPath);

                        outputPdfPaths.Add(outputPdfPath);
                    }
                }

                return new Attempt<IEnumerable<string>>()
                {
                    Success = true,
                    Data = outputPdfPaths
                };
            }
            catch (Exception ex)
            {
                return new Attempt<IEnumerable<string>>()
                {
                    ErrorMessage = $"Failed to split PDF by ranges - {ex.Message}"
                };
            }
        }

        private Attempt<PdfDocument> OpenPdf(string inputPdfPath)
        {
            PdfDocument inputPdf = null;

            try
            {
                inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import);

                if (inputPdf == null)
                {
                    return new Attempt<PdfDocument>()
                    {
                        ErrorMessage = "Opened PDF was null."
                    };
                }

                return new Attempt<PdfDocument>()
                {
                    Success = true,
                    Data = inputPdf
                };
            }
            catch (Exception ex)
            {
                if (inputPdf != null)
                {
                    inputPdf.Dispose();
                }

                return new Attempt<PdfDocument>()
                {
                    ErrorMessage = $"Failed to open PDF at location {inputPdfPath} - {ex.Message}"
                };
            }
        }
    }
}
