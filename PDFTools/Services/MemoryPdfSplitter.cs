﻿namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System;
    using System.Collections.Generic;

    public class MemoryPdfSplitter : IMemoryPdfSplitter
    {
        private const string PdfFileExtension = ".pdf";
        private readonly ISplitRangeParser splitRangeParser;

        public MemoryPdfSplitter(ISplitRangeParser splitRangeParser)
        {
            this.splitRangeParser = splitRangeParser;
        }

        //TODO: Need to test this method functionaly.
        /// <inheritdoc />
        public ServiceResponse<IEnumerable<Stream>> SplitByInterval(Stream inputPdfStream, int interval)
        {
            if (inputPdfStream == null)
            {
                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(inputPdfStream)} cannot be null."
                };
            }

            ServiceResponse<PdfDocument> pdfResponse = OpenPdf(inputPdfStream);
            if (!pdfResponse.Success || pdfResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<Stream>>()
                {
                    ErrorMessage = pdfResponse.ErrorMessage
                };
            }

            ServiceResponse<IEnumerable<SplitRange>> rangesResponse = splitRangeParser.GenerateRangesFromInterval(interval, pdfResponse.Data.PageCount);
            if (!rangesResponse.Success)
            {
                return new ServiceResponse<IEnumerable<Stream>>()
                {
                    ErrorMessage = rangesResponse.ErrorMessage
                };
            }

            if (rangesResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<Stream>>()
                {
                    ErrorMessage = $"{nameof(rangesResponse)}.Data cannot be null."
                };
            }

            return SplitByRanges(pdfResponse.Data, rangesResponse.Data);
        }

        /// <inheritdoc />
        public ServiceResponse<IEnumerable<Stream>> SplitByRanges(Stream inputPdfStream, string ranges)
        {
            if (inputPdfStream == null)
            {
                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(inputPdfStream)} cannot be null."
                };
            }

            if (inputPdfStream.Length == 0)
            {
                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(inputPdfStream)} was empty."
                };
            }

            if (string.IsNullOrWhiteSpace(ranges))
            {
                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = $"{nameof(ranges)} cannt be null or whitespace."
                };
            }

            ServiceResponse<PdfDocument> pdfResponse = OpenPdf(inputPdfStream);
            if (!pdfResponse.Success || pdfResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = pdfResponse.ErrorMessage
                };
            }

            ServiceResponse<IEnumerable<SplitRange>> parseRangesResponse = splitRangeParser.ParseRangesFromString(ranges);

            if (!parseRangesResponse.Success || parseRangesResponse.Data == null)
            {
                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = parseRangesResponse.ErrorMessage
                };
            }

            return SplitByRanges(pdfResponse.Data, parseRangesResponse.Data);
        }

        private ServiceResponse<IEnumerable<Stream>> SplitByRanges(PdfDocument inputPdf, IEnumerable<SplitRange> ranges)
        {
            if (inputPdf == null)
            {
                return new ServiceResponse<IEnumerable<Stream>>()
                {
                    ErrorMessage = $"{nameof(inputPdf)} cannot be null."
                };
            }

            if (ranges == null || !ranges.Any())
            {
                return new ServiceResponse<IEnumerable<Stream>>()
                {
                    ErrorMessage = $"{nameof(ranges)} cannot be null or empty."
                };
            }

            List<Stream> outputPdfSteams = new List<Stream>();

            try
            {
                // TODO: need to confirm this is the correct way to get the name.
                string pdfName = inputPdf.Info.Title;

                foreach (SplitRange range in ranges)
                {
                    // TODO: this can also go in the pdf meta data service.
                    PdfDocument outputPdf = new PdfDocument();
                    outputPdf.Version = inputPdf.Version;
                    outputPdf.Info.Title = CreateOutputPdfName(pdfName, range);

                    // TODO: can this go in a common service? Maybe a base class.
                    for (int pageNumber = range.StartPageNumber; pageNumber <= range.EndPageNumber; pageNumber++)
                    {
                        outputPdf.AddPage(inputPdf.Pages[pageNumber - 1]); //PDFSharp uses zero based indexing for pages.
                    }
                
                    MemoryStream outputPdfStream = new MemoryStream();
                    outputPdf.Save(outputPdfStream, false); // Leave the stream open - it's up to the caller to close it.
                    outputPdfSteams.Add(outputPdfStream);
                }

                return new ServiceResponse<IEnumerable<Stream>>()
                {
                    Success = true,
                    Data = outputPdfSteams
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = $"Failed to split PDFs - {ex.Message}"
                };
            }
        }

        // TODO: create a service to do this logic. Maybe call it pdf meta data operations or something like that.
        private string CreateOutputPdfName(string inputPdfName, SplitRange range)
        {
            string outputPdfName = $"{inputPdfName}_Page_{range.StartPageNumber}";

            if (!range.EndPageNumber.HasValue)
            {
                return outputPdfName;
            }

            return $"{outputPdfName}_to_{range.EndPageNumber}";
        }

        private ServiceResponse<PdfDocument> OpenPdf(Stream inputPdfStream)
        {
            try
            {
                PdfDocument inputPdf = PdfReader.Open(inputPdfStream, PdfDocumentOpenMode.Import);

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
