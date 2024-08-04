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

        //TODO: Need to test this method functionaly.
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

            Attempt<PdfDocument> pdfAttempt = OpenPdf(inputPdfStream);
            if (!pdfAttempt.Success || pdfAttempt.Data == null)
            {
                return new Attempt<IEnumerable<Stream>>()
                {
                    ErrorMessage = pdfAttempt.ErrorMessage
                };
            }

            Attempt<IEnumerable<SplitRange>> rangesAttempt = splitRangeParser.GenerateRangesFromInterval(interval, pdfAttempt.Data.PageCount);
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

            return SplitByRanges(pdfAttempt.Data, rangesAttempt.Data);
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

            Attempt<PdfDocument> pdfAttempt = OpenPdf(inputPdfStream);
            if (!pdfAttempt.Success || pdfAttempt.Data == null)
            {
                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = pdfAttempt.ErrorMessage
                };
            }

            Attempt<IEnumerable<SplitRange>> parseRangesAttempt = splitRangeParser.ParseRangesFromString(ranges);

            if (!parseRangesAttempt.Success || parseRangesAttempt.Data == null)
            {
                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = parseRangesAttempt.ErrorMessage
                };
            }

            return SplitByRanges(pdfAttempt.Data, parseRangesAttempt.Data);
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

            List<Stream> outputPdfSteams = new List<Stream>();

            try
            {
                string inputPdfName = inputPdf.Info.Title;

                foreach (SplitRange range in ranges)
                {
                    PdfDocument outputPdf = CreateNewPdfDocumentFromRange(inputPdf, range, inputPdfName);
                
                    MemoryStream outputPdfStream = new MemoryStream();
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

                return new Attempt<IEnumerable<Stream>>
                {
                    ErrorMessage = $"Failed to split PDFs - {ex.Message}"
                };
            }
        }

        private Attempt<PdfDocument> OpenPdf(Stream inputPdfStream)
        {
            try
            {
                PdfDocument inputPdf = PdfReader.Open(inputPdfStream, PdfDocumentOpenMode.Import);

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
                //TODO: Add logging that includes pdf path here.
                return new Attempt<PdfDocument>()
                {
                    ErrorMessage = $"Failed to open PDF - {ex.Message}"
                };
            }
        }
    }
}
