namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// PDF splitter that can split PDFs entirely in memory without using file storage.
    /// </summary>
    public class MemoryPdfSplitter : PdfSplitterBase, IMemoryPdfSplitter
    {
        private const string PdfFileExtension = ".pdf";
        private readonly ISplitRangeParser splitRangeParser;

        public MemoryPdfSplitter(ISplitRangeParser splitRangeParser)
        {
            this.splitRangeParser = splitRangeParser;
        }

        /// <inheritdoc />
        public Attempt<IEnumerable<Stream>> SplitByInterval(Stream inputPdfStream, int interval)
        {
            if (inputPdfStream == null)
            {
                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(inputPdfStream)} cannot be null."
                };
            }

            try
            {
                using (PdfDocument openedPdf = PdfReader.Open(inputPdfStream, PdfDocumentOpenMode.Import))
                {
                    Attempt<IEnumerable<SplitRange>> rangesAttempt = splitRangeParser.GenerateRangesFromInterval(interval, openedPdf.PageCount);
                    if (!rangesAttempt.Success)
                    {
                        return new Attempt<IEnumerable<Stream>>()
                        {
                            ErrorMessage = rangesAttempt.ErrorMessage
                        };
                    }

                    if (rangesAttempt.Data == null)
                    {
                        return new Attempt<IEnumerable<Stream>>()
                        {
                            ErrorMessage = $"{nameof(rangesAttempt)}.Data cannot be null."
                        };
                    }

                    return SplitByRanges(openedPdf, rangesAttempt.Data);
                }
            }
            catch (Exception ex)
            {
                return new Attempt<IEnumerable<Stream>>()
                {
                    ErrorMessage = $"Failed to split by interval - {ex.Message}"
                };
            }
        }

        /// <inheritdoc />
        public Attempt<IEnumerable<Stream>> SplitByRanges(Stream inputPdfStream, string ranges)
        {
            if (inputPdfStream == null)
            {
                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(inputPdfStream)} cannot be null."
                };
            }

            if (inputPdfStream.Length == 0)
            {
                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(inputPdfStream)} was empty."
                };
            }

            if (string.IsNullOrWhiteSpace(ranges))
            {
                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(ranges)} cannt be null or whitespace."
                };
            }

            try
            {
                using (PdfDocument openedPdf = PdfReader.Open(inputPdfStream, PdfDocumentOpenMode.Import))
                {
                    Attempt<IEnumerable<SplitRange>> parseRangesAttempt = splitRangeParser.ParseRangesFromString(ranges);

                    if (!parseRangesAttempt.Success || parseRangesAttempt.Data == null)
                    {
                        return new Attempt<IEnumerable<Stream>>
                        {
                            ErrorMessage = parseRangesAttempt.ErrorMessage
                        };
                    }

                    return SplitByRanges(openedPdf, parseRangesAttempt.Data);
                }
            }
            catch (Exception ex)
            {
                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = $"Failed to split the PDF by ranges - {ex.Message}"
                };
            }
        }

        private Attempt<IEnumerable<Stream>> SplitByRanges(PdfDocument inputPdf, IEnumerable<SplitRange> ranges)
        {
            if (inputPdf == null)
            {
                return new Attempt<IEnumerable<Stream>>()
                {
                    ErrorMessage = $"{nameof(inputPdf)} cannot be null."
                };
            }

            if (ranges == null || !ranges.Any())
            {
                return new Attempt<IEnumerable<Stream>>()
                {
                    ErrorMessage = $"{nameof(ranges)} cannot be null or empty."
                };
            }

            PdfDocument outputPdf = null;
            MemoryStream outputPdfStream = null;
            List<Stream> outputPdfSteams = new List<Stream>();

            try
            {
                string inputPdfName = inputPdf.Info.Title;

                foreach (SplitRange range in ranges)
                {
                    outputPdf = CreateNewPdfDocumentFromRange(inputPdf, range, inputPdfName);
                
                    outputPdfStream = new MemoryStream();
                    outputPdf.Save(outputPdfStream, false); // Leave the stream open - it's up to the caller to close it.
                    outputPdfSteams.Add(outputPdfStream);
                }

                return new Attempt<IEnumerable<Stream>>()
                {
                    Success = true,
                    Data = outputPdfSteams
                };
            }
            catch (Exception ex)
            {
                // make sure all disposable objects get disposed
                DisposeOf(outputPdf);
                DisposeOf(outputPdfStream);

                foreach (MemoryStream stream in outputPdfSteams)
                {
                    DisposeOf(stream);
                }

                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = $"Failed to split PDFs - {ex.Message}"
                };
            }
        }

        private void DisposeOf(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
