﻿namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PDFTools.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for pdf splitters.
    /// </summary>
    public abstract class PdfSplitterBase
    {
        protected PdfDocument CreateNewPdfDocumentFromRange(PdfDocument inputPdf, SplitRange range, string inputPdfName)
        {
            PdfDocument outputPdf = new PdfDocument();

            try
            {
                outputPdf.Version = inputPdf.Version;
                outputPdf.Info.Title = CreateOutputPdfName(inputPdfName, range);
                outputPdf.Info.CreationDate = DateTime.UtcNow;

                for (int pageNumber = range.StartPageNumber; pageNumber <= range.EndPageNumber; pageNumber++)
                {
                    outputPdf.AddPage(inputPdf.Pages[pageNumber - 1]); //PDFSharp uses zero based indexing for pages.
                }

                return outputPdf;
            }
            catch (Exception ex)
            {
                outputPdf.Dispose();
                throw new Exception($"Failed to create a new PDF Document from range - ${ex.Message}");
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
    }
}
