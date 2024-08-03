namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PDFTools.Models;
    using PDFTools.Services.Interfaces;
    using System.Collections.Generic;

    public class PdfMerger : IPdfMerger
    {
        private const string PdfFileExtension = ".pdf";
        private const string ResultFileName = "Merge_Result"; //TODO: Make merge result add the datetime to the name.

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
                    outputPdf.Info.Title = ResultFileName;

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

                    string outputPdfPath = $"{outputFolderPath}{ResultFileName}.pdf";
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

        /// <inheritdoc />
        public ServiceResponse<Stream> MergePdfsInMemory(IEnumerable<Stream> inputPdfStreams)
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
