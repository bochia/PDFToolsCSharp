namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    //TODO: Change this into 2 classes. An in memory and a hard disk class.
    // All the like named methods are going to get confusing and maybe will accidently use a hard disk method with a in memory method.
    //HardDiskPdfSplitter
    //MemoryPdfSplitter.
    public class MemoryPdfSplitter : IMemoryPdfSplitter
    {
        private const string PdfFileExtension = ".pdf";
        private readonly ISplitRangeParser splitRangeParser;

        public MemoryPdfSplitter(ISplitRangeParser splitRangeParser)
        {
            this.splitRangeParser = splitRangeParser;
        }

        /// <inheritdoc />
        public ServiceResponse<IEnumerable<Stream>> SplitByInterval(Stream inputPdfStream, int interval)
        {
            throw new NotImplementedException();
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

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {

                return new ServiceResponse<IEnumerable<Stream>>
                {
                    ErrorMessage = $"Failed to split PDFs - {ex.Message}"
                };
            }
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

        // ochia - TODO - need to implement this.
        private string ZipOutputPdfsTogether()
        {
            throw new NotImplementedException();
        }
    }
}
