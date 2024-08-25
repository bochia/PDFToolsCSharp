namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    public class MemoryPdfMerger : IMemoryPdfMerger
    {
        private const string PdfFileExtension = ".pdf";
        private const string ResultFileName = "MergeResult";

        /// <inheritdoc />
        public Attempt<Stream> Merge(IEnumerable<Stream> inputPdfStreams)
        {
            if (inputPdfStreams == null || !inputPdfStreams.Any())
            {
                return new Attempt<Stream>()
                {
                    ErrorMessage = $"{nameof(inputPdfStreams)} cannot be null or empty."
                };
            }

            //check if any of the inputPdfStreams are empty
            foreach (Stream inputPdfStream in inputPdfStreams)
            {
                if (inputPdfStream.Length == 0)
                {
                    return new Attempt<Stream>()
                    {
                        ErrorMessage = $"{nameof(inputPdfStream)} was empty."
                    };
                }
            }

            MemoryStream outputPdfStream = new MemoryStream();

            try
            {
                using (PdfDocument outputPdf = new PdfDocument())
                {
                    outputPdf.Info.Title = ResultFileName;

                    foreach (Stream inputPdfStream in inputPdfStreams)
                    {

                        using (PdfDocument inputPdf = PdfReader.Open(inputPdfStream, PdfDocumentOpenMode.Import))
                        {
                            if (inputPdf == null)
                            {
                                return new Attempt<Stream>()
                                {
                                    ErrorMessage = $"Couldn't open PDF using input PDF stream."
                                };
                            }

                            foreach (PdfPage page in inputPdf.Pages)
                            {
                                outputPdf.AddPage(page);
                            }
                        }
                    }

                    outputPdf.Save(outputPdfStream, false); // Leave the stream open - it's up to the caller to close it.

                    return new Attempt<Stream>()
                    {
                        Success = true,
                        Data = outputPdfStream,
                    };
                }
            }
            catch (Exception ex)
            {
                outputPdfStream.Dispose();
                return new Attempt<Stream>()
                {
                    ErrorMessage = $"Failed to merge PDFs - {ex.Message}"
                };
            }
        }
    }
}
