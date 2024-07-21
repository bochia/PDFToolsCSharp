
using PDFTools.Models;
using PDFTools.Services;
using PDFTools.Services.Interfaces;
using System.Diagnostics;

//TODO: Can turn this into a nuget package, then use that in your website project. Make the console program internal only. Dont allow to be seen in nuget package.
//TODO: Create a PDF macro editor that can do stuff like SplitThenMerge etc..
//TODO: Need to make sure all of my guard clauses are good.
//TODO: Add unit tests using xUnit and NSubstitute.
//TODO: Maybe add a feature to add a prefix to output files.

string inputPdfPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\TestPdf_Max20Pages.pdf";
string outputFolderPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\OutputFiles\";

//TestMethodsThatWriteToDisk(inputPdfPath, outputFolderPath);
TestInMemoryMethods(inputPdfPath, outputFolderPath);


static void TestMethodsThatWriteToDisk(string inputPdfPath, string outputFolderPath)
{
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

    IPdfMergeService pdfMerger = new PdfMergeService();
    ServiceResponse<string> mergeResponse = pdfMerger.MergePdfs(splitResponse1.Data, outputFolderPath);

    if (!mergeResponse.Success)
    {
        Console.WriteLine(mergeResponse.ErrorMessage);
    }

    // open up file explorer so you can look at the results.
    Process.Start("explorer.exe", outputFolderPath);
}

static void TestInMemoryMethods(string inputPdfPath, string outputFolderPath)
{
    // before starting to create output files clean the folder so you can start fresh.
    DeleteAllOutputFiles(outputFolderPath);

    ISplitRangeParser splitRangeParser = new SplitRangeParser();
    IPdfSplitService pdfSplitter = new PdfSplitService(splitRangeParser);
    ServiceResponse<IEnumerable<string>> splitResponse1 = pdfSplitter.SplitByRanges(inputPdfPath, outputFolderPath, "1,3-5,8-16");

    if (!splitResponse1.Success)
    {
        Console.WriteLine(splitResponse1.ErrorMessage);
    }

    List<Stream> pdfStreams = new List<Stream>();

    foreach (string pdfPath in splitResponse1.Data)
    {
        FileStream inMemoryPdf = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
        pdfStreams.Add(inMemoryPdf);
    }

    IPdfMergeService pdfMerger = new PdfMergeService();
    ServiceResponse<Stream> inMemoryMergeResponse = pdfMerger.MergePdfsInMemory(pdfStreams);

    // Open a FileStream to write to the file
    using (FileStream fs = new FileStream($"{outputFolderPath}InMemoryMergeResult.pdf", FileMode.Create, FileAccess.Write))
    {
        // Copy data from MemoryStream to FileStream
        inMemoryMergeResponse.Data.CopyTo(fs);
        Console.WriteLine("Stream data written to file successfully.");
    }

    // open up file explorer so you can look at the results.
    Process.Start("explorer.exe", outputFolderPath);
}


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