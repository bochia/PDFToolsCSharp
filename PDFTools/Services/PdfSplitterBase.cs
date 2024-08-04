namespace PDFTools.Services
{
    using PdfSharpCore.Pdf;
    using PDFTools.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class PdfSplitterBase
    {
        protected PdfDocument CreateNewPdfDocumentFromRange(PdfDocument inputPdf, SplitRange range, string inputPdfName)
        {
            PdfDocument outputPdf = new PdfDocument();
            outputPdf.Version = inputPdf.Version;
            outputPdf.Info.Title = CreateOutputPdfName(inputPdfName, range);
            outputPdf.Info.CreationDate = DateTime.UtcNow;

            for (int pageNumber = range.StartPageNumber; pageNumber <= range.EndPageNumber; pageNumber++)
            {
                outputPdf.AddPage(inputPdf.Pages[pageNumber - 1]); //PDFSharp uses zero based indexing for pages.
            }

            return outputPdf;
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
    }
}
