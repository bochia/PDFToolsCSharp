namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    public class PdfMergeService : IPdfMergeService
    {
        private const string PdfFileExtension = ".pdf";

        /// <inheritdoc />
        public ServiceResponse<string> MergePdfs(IEnumerable<string> inputPdfPaths, string outputFolderPath)
        {
            if (inputPdfPaths == null || !inputPdfPaths.Any())
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"{nameof(inputPdfPaths)} cannot be null or empty."
                };
            }

            if (string.IsNullOrWhiteSpace(outputFolderPath))
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"{nameof(outputFolderPath)} cannot be null or whitespace."
                };
            }

            if (!Directory.Exists(outputFolderPath))
            {
                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"Directory doesn't exist at specified location: {outputFolderPath}."
                };
            }

            foreach (string inputPdfPath in inputPdfPaths)
            {
                if (!File.Exists(inputPdfPath))
                {
                    return new ServiceResponse<string>()
                    {
                        ErrorMessage = $"Couldn't find file at {inputPdfPath}"
                    };
                }

                if (!inputPdfPath.EndsWith(PdfFileExtension))
                {
                    return new ServiceResponse<string>()
                    {
                        ErrorMessage = $"Couldn't find file at {inputPdfPath}"
                    };
                }
            }

            try
            {
                using (PdfDocument outputPdf = new PdfDocument())
                {
                    outputPdf.Info.Title = "Merge_Result";

                    foreach (string inputPdfPath in inputPdfPaths)
                    {

                        using (PdfDocument inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import))
                        {
                            if (inputPdf == null)
                            {
                                return new ServiceResponse<string>()
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

                    string outputPdfPath = $"{outputFolderPath}{outputPdf.Info.Title}{PdfFileExtension}";
                    outputPdf.Save(outputPdfPath);

                    return new ServiceResponse<string>()
                    {
                        Success = true,
                        Data = outputPdfPath,
                    };
                }
            }
            catch (Exception ex)
            {

                return new ServiceResponse<string>()
                {
                    ErrorMessage = $"Failed to merge PDFs - {ex.Message}"
                };
            }

        }
    }
}
