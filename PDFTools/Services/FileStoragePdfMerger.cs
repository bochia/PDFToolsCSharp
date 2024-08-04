namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    public class FileStoragePdfMerger : IStoragePdfMerger
    {
        private const string PdfFileExtension = ".pdf";
        private const string ResultFileName = "MergeResult"; //TODO: Make merge result add the datetime to the name.

        /// <inheritdoc />
        public Attempt<string> Merge(IEnumerable<string> inputPdfPaths, string outputFolderPath)
        {
            if (inputPdfPaths == null || !inputPdfPaths.Any())
            {
                return new Attempt<string>()
                {
                    ErrorMessage = $"{nameof(inputPdfPaths)} cannot be null or empty."
                };
            }

            if (string.IsNullOrWhiteSpace(outputFolderPath))
            {
                return new Attempt<string>()
                {
                    ErrorMessage = $"{nameof(outputFolderPath)} cannot be null or whitespace."
                };
            }

            if (!Directory.Exists(outputFolderPath))
            {
                return new Attempt<string>()
                {
                    ErrorMessage = $"Directory doesn't exist at specified location: {outputFolderPath}."
                };
            }

            foreach (string inputPdfPath in inputPdfPaths)
            {
                if (!File.Exists(inputPdfPath))
                {
                    return new Attempt<string>()
                    {
                        ErrorMessage = $"Couldn't find file at {inputPdfPath}"
                    };
                }

                if (!inputPdfPath.EndsWith(PdfFileExtension))
                {
                    return new Attempt<string>()
                    {
                        ErrorMessage = $"Couldn't find file at {inputPdfPath}"
                    };
                }
            }

            try
            {
                using (PdfDocument outputPdf = new PdfDocument())
                {
                    outputPdf.Info.Title = ResultFileName;

                    foreach (string inputPdfPath in inputPdfPaths)
                    {

                        using (PdfDocument inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import))
                        {
                            if (inputPdf == null)
                            {
                                return new Attempt<string>()
                                {
                                    ErrorMessage = $"{inputPdfPath} couldn't be opened." //TODO: Only should return name of pdf not path.
                                };
                            }

                            foreach (PdfPage page in inputPdf.Pages)
                            {
                                outputPdf.AddPage(page);
                            }
                        }
                    }

                    string outputPdfPath = $"{outputFolderPath}{ResultFileName}.pdf";
                    outputPdf.Save(outputPdfPath);

                    return new Attempt<string>()
                    {
                        Success = true,
                        Data = outputPdfPath,
                    };
                }
            }
            catch (Exception ex)
            {

                return new Attempt<string>()
                {
                    ErrorMessage = $"Failed to merge PDFs - {ex.Message}"
                };
            }

        }
    }
}
