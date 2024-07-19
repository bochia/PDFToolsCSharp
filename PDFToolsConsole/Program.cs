
using PDFTools.Models;
using PDFTools.Services;
using PDFTools.Services.Interfaces;
using System.Diagnostics;

//TODO: Create a PDF macro editor that can do stuff like SplitThenMerge etc..
//TODO: Need to make sure all of my guard clauses are good.
//TODO: Add unit tests using xUnit and NSubstitute.

string inputPdfPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\TestPdf_Max20Pages.pdf";
string outputFolderPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\OutputFiles\";

// before starting to create output files clean the folder so you can start fresh.
DeleteAllOutputFiles(outputFolderPath);

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
    ServiceResponse<string> mergeResponse = pdfMerger.MergePdfs(splitResponse1.Data, outputFolderPath);

    if (!mergeResponse.Success)
    {
        Console.WriteLine(mergeResponse.ErrorMessage);
    }
}

// open up file explorer so you can look at the results.
Process.Start("explorer.exe", outputFolderPath);


static void DeleteAllOutputFiles(string outputFolderPath)
{

    System.IO.DirectoryInfo di = new DirectoryInfo(outputFolderPath);

    foreach (FileInfo file in di.GetFiles())
    {
        file.Delete();
    }
    foreach (DirectoryInfo dir in di.GetDirectories())
    {
        dir.Delete(true);
    }
}