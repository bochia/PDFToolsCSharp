
using PDFTools.Models;
using PDFTools.Services;
using PDFTools.Services.Interfaces;

string inputPdfPath = @"C:\Users\John\Documents\TestPdf.pdf";
string outputFolderPath = @"C:\Users\John\Documents\";

ISplitRangeParser splitRangeParser = new SplitRangeParser();
IPdfSplitService pdfSplitter = new PdfSplitService(splitRangeParser);
ServiceResponse<IEnumerable<string>> splitResponse1 = pdfSplitter.SplitByRanges(inputPdfPath, outputFolderPath, "1,3-5,8-16");
ServiceResponse<IEnumerable<string>> splitResponse2 = pdfSplitter.SplitByInterval(inputPdfPath, 15, outputFolderPath);

if (!splitResponse1.Success)
{
    Console.WriteLine(splitResponse1.ErrorMessage);
}

if (!splitResponse2.Success)
{
    Console.WriteLine(splitResponse2.ErrorMessage);
}

if (splitResponse1.Data != null)
{
    IPdfMergeService pdfMerger = new PdfMergeService();
    ServiceResponse<string> mergeResponse = pdfMerger.MergePdfs(splitResponse1.Data, @"C:\Users\John\Documents\");

    if (!mergeResponse.Success)
    {
        Console.WriteLine(mergeResponse.ErrorMessage);
    }
}

//TODO: Create a PDF macro editor that can do stuff like SplitThenMerge etc..