
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
//TODO: Add a feature where the user can write their own prefix, suffix, or entire name for the storage splitters.

string inputPdfPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\TestPdf_Max20Pages.pdf";
string outputFolderPath = @"C:\source\repos\PDFToolsCSharp\PDFFiles\OutputFiles";

string ranges = "1,3-5,8-16";
TestMethodsThatWriteToDisk(inputPdfPath, outputFolderPath, ranges);
TestInMemoryMethods(inputPdfPath, outputFolderPath, ranges);


static void TestMethodsThatWriteToDisk(string inputPdfPath, string outputFolderPath, string ranges)
{
    string outputFolderPathForStorageFolder = $"{outputFolderPath}\\Storage\\";

    // before starting to create output files clean the folder so you can start fresh.
    DeleteAllOutputFiles(outputFolderPathForStorageFolder);

    ISplitRangeParser splitRangeParser = new SplitRangeParser();
    IStoragePdfSplitter pdfSplitter = new StoragePdfSplitter(splitRangeParser);
    ServiceResponse<IEnumerable<string>> splitResponse1 = pdfSplitter.SplitByRanges(inputPdfPath, outputFolderPathForStorageFolder, ranges);
    ServiceResponse<IEnumerable<string>> splitResponse2 = pdfSplitter.SplitByInterval(inputPdfPath, 15, outputFolderPathForStorageFolder);

    if (!splitResponse1.Success)
    {
        Console.WriteLine(splitResponse1.ErrorMessage);
    }

    if (!splitResponse2.Success)
    {
        Console.WriteLine(splitResponse2.ErrorMessage);
    }

    IStoragePdfMerger pdfMerger = new StoragePdfMerger();
    ServiceResponse<string> mergeResponse = pdfMerger.Merge(splitResponse1.Data, outputFolderPathForStorageFolder);

    if (!mergeResponse.Success)
    {
        Console.WriteLine(mergeResponse.ErrorMessage);
    }

    // open up file explorer so you can look at the results.
    Process.Start("explorer.exe", outputFolderPathForStorageFolder);
}

static void TestInMemoryMethods(string inputPdfPath, string outputFolderPath, string ranges)
{
    string outputFolderPathForInMemory = $"{outputFolderPath}\\InMemory\\";

    // before starting to create output files clean the folder so you can start fresh.
    DeleteAllOutputFiles(outputFolderPathForInMemory);

    ISplitRangeParser splitRangeParser = new SplitRangeParser();

    using (FileStream inputPdfStream = new FileStream(inputPdfPath, FileMode.Open, FileAccess.Read))
    {
        InMemorySplitByRangesAndThenMerge(inputPdfStream, outputFolderPathForInMemory, splitRangeParser, ranges);
        InMemorySplitByInternal(inputPdfStream, outputFolderPathForInMemory, splitRangeParser);
    }

    // open up file explorer so you can look at the results.
    Process.Start("explorer.exe", outputFolderPathForInMemory);
}

static void InMemorySplitByRangesAndThenMerge(FileStream inputPdfStream, string outputFolderPath, ISplitRangeParser splitRangeParser, string ranges)
{
    IMemoryPdfSplitter pdfSplitter = new MemoryPdfSplitter(splitRangeParser);
    ServiceResponse<IEnumerable<Stream>> splitResponse = pdfSplitter.SplitByRanges(inputPdfStream, ranges);

    IMemoryPdfMerger pdfMerger = new MemoryPdfMerger();
    ServiceResponse<Stream> inMemoryMergeResponse = pdfMerger.Merge(splitResponse.Data);

    // Open a FileStream to write to the file
    using (FileStream fs = new FileStream($"{outputFolderPath}InMemoryMergeResult_FromSplitByRange_{ranges}.pdf", FileMode.Create, FileAccess.Write))
    {
        // Copy data from MemoryStream to FileStream
        inMemoryMergeResponse.Data.CopyTo(fs);
        Console.WriteLine("Stream data written to file successfully.");
    }
}

static void InMemorySplitByInternal(FileStream inputPdfStream, string outputFolderPath, ISplitRangeParser splitRangeParser)
{
    int internval = 5;

    IMemoryPdfSplitter pdfSplitter = new MemoryPdfSplitter(splitRangeParser);
    ServiceResponse<IEnumerable<Stream>> splitResponse = pdfSplitter.SplitByInterval(inputPdfStream, internval);


    int index = 0;
    foreach (Stream pdfStream in splitResponse.Data)
    {
        // Open a FileStream to write to the file to output folder
        using (FileStream fs = new FileStream($"{outputFolderPath}InMemorySplitByInternvalResult_{index}.pdf", FileMode.Create, FileAccess.Write))
        {
            // Copy data from MemoryStream to FileStream
            pdfStream.CopyTo(fs);
            Console.WriteLine("Stream data written to file successfully.");
        }

        index++;
    }
}


static void DeleteAllOutputFiles(string outputFolderPath)
{
    System.IO.DirectoryInfo di = new DirectoryInfo(outputFolderPath);

    foreach (FileInfo file in di.GetFiles())
    {
        file.Delete();
    }
}