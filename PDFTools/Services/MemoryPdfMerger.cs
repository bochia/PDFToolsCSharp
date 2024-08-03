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
        private const string ResultFileName = "Merge_Result"; //TODO: Make merge result add the datetime to the name.

        /// <inheritdoc />
        public ServiceResponse<Stream> Merge(IEnumerable<Stream> inputPdfStreams)
        {
            if (inputPdfStreams == null || !inputPdfStreams.Any())
            {
                return new ServiceResponse<Stream>()
                {
                    ErrorMessage = $"{nameof(inputPdfStreams)} cannot be null or empty."
                };
            }

            //check if any of the inputPdfStreams are empty
            foreach (Stream inputPdfStream in inputPdfStreams)
            {
                if (inputPdfStream.Length == 0)
                {
                    return new ServiceResponse<Stream>()
                    {
                        ErrorMessage = $"{nameof(inputPdfStream)} was empty."
                    };
                }
            }

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
                                return new ServiceResponse<Stream>()
                                {
                                    ErrorMessage = $"Couldn't open PDF using input PDF stream"
                                };
                            }

                            foreach (PdfPage page in inputPdf.Pages)
                            {
                                outputPdf.AddPage(page);
                            }
                        }
                    }

                    MemoryStream outputPdfStream = new MemoryStream();
                    outputPdf.Save(outputPdfStream, false); //TODO: should I close the stream here?

                    return new ServiceResponse<Stream>()
                    {
                        Success = true,
                        Data = outputPdfStream,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Stream>()
                {
                    ErrorMessage = $"Failed to merge PDFs - {ex.Message}" //TODO: Need to make this more DRY. Search for other place it is used.
                };
            }
        }
    }
}
