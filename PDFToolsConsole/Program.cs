
using PDFTools.Models;
using PDFTools.Services;
using PDFTools.Services.Interfaces;

string inputPdfPath = @"C:\Users\John\Downloads\TestPdf.pdf";
string outputFolderPath = @"C:\Users\John\Downloads\";

ISplitRangeParser splitRangeParser = new SplitRangeParser();
IPdfSplitService pdfSplitter = new PdfSplitService(splitRangeParser);
//ServiceResponse<string> splitResponse = pdfSplitter.SplitByRanges(inputPdfPath, outputFolderPath, "1,3-5,8-16");

ServiceResponse<string> splitResponse = pdfSplitter.SplitByInterval(inputPdfPath, 15, outputFolderPath);

if (!splitResponse.Success)
{
    Console.WriteLine(splitResponse.ErrorMessage);
}
