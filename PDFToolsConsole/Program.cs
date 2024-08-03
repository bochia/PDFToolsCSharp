
using PDFTools.Models;
using PDFTools.Services;
using PDFTools.Services.Interfaces;
using System.Diagnostics;

//TODO: Can turn this into a nuget package, then use that in your website project. Make the console program internal only. Dont allow to be seen in nuget package.
//TODO: Create a PDF macro editor that can do stuff like SplitThenMerge etc..
//TODO: Need to make sure all of my guard clauses are good.
//TODO: Add unit tests using xUnit and NSubstitute.
//TODO: Maybe add a feature to add a prefix to output files.
//TODO: Go through all methods and make sure proper try catches are used.
//TODO: Implement a service that will zip files together. Make a hard disk one and a memory version.
//TODO: Add unit tests using XUnit and NSubstitute.

string inputPdfPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\TestPdf_Max20Pages.pdf";
string outputFolderPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\OutputFiles\";

string ranges = "1,3-5,8-16";
//TestMethodsThatWriteToDisk(inputPdfPath, outputFolderPath, ranges);
TestInMemoryMethods(inputPdfPath, outputFolderPath, ranges);


static void TestMethodsThatWriteToDisk(string inputPdfPath, string outputFolderPath, string ranges)
{
    // before starting to create output files clean the folder so you can start fresh.
    DeleteAllOutputFiles(outputFolderPath);

    ISplitRangeParser splitRangeParser = new SplitRangeParser();
    IHardDiskPdfSplitter pdfSplitter = new HardDiskPdfSplitter(splitRangeParser);
    ServiceResponse<IEnumerable<string>> splitResponse1 = pdfSplitter.SplitByRanges(inputPdfPath, outputFolderPath, ranges);
    ServiceResponse<IEnumerable<string>> splitResponse2 = pdfSplitter.SplitByInterval(inputPdfPath, 15, outputFolderPath);

    if (!splitResponse1.Success)
    {
        Console.WriteLine(splitResponse1.ErrorMessage);
    }

    if (!splitResponse2.Success)
    {
        Console.WriteLine(splitResponse2.ErrorMessage);
    }

    IHardDiskPdfMerger pdfMerger = new HardDiskPdfMerger();
    ServiceResponse<string> mergeResponse = pdfMerger.Merge(splitResponse1.Data, outputFolderPath);

    if (!mergeResponse.Success)
    {
        Console.WriteLine(mergeResponse.ErrorMessage);
    }

    // open up file explorer so you can look at the results.
    Process.Start("explorer.exe", outputFolderPath);
}

static void TestInMemoryMethods(string inputPdfPath, string outputFolderPath, string ranges)
{
    // before starting to create output files clean the folder so you can start fresh.
    DeleteAllOutputFiles(outputFolderPath);

    ISplitRangeParser splitRangeParser = new SplitRangeParser();

    using (FileStream inputPdfStream = new FileStream(inputPdfPath, FileMode.Open, FileAccess.Read))
    {
        IMemoryPdfSplitter pdfSplitter = new MemoryPdfSplitter(splitRangeParser);
        ServiceResponse<IEnumerable<Stream>> splitResponse = pdfSplitter.SplitByRanges(inputPdfStream, ranges);

        IMemoryPdfMerger pdfMerger = new MemoryPdfMerger();
        ServiceResponse<Stream> inMemoryMergeResponse = pdfMerger.Merge(splitResponse.Data);

        // Open a FileStream to write to the file
        using (FileStream fs = new FileStream($"{outputFolderPath}InMemoryMergeResult.pdf", FileMode.Create, FileAccess.Write))
        {
            // Copy data from MemoryStream to FileStream
            inMemoryMergeResponse.Data.CopyTo(fs);
            Console.WriteLine("Stream data written to file successfully.");
        }
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