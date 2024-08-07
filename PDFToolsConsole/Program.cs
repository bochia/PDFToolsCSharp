
using PDFTools.Models;
using PDFTools.Services;
using PDFTools.Services.Interfaces;
using System.Diagnostics;

//TODO: Create a readme file that has examples of how to use the code.
//TODO: Create a PDF macro editor that can do stuff like SplitThenMerge etc..
//TODO: Need to make sure all of my guard clauses are good.
//TODO: Add unit tests using xUnit and NSubstitute.
//TODO: Maybe add a feature to add a prefix to output files.
//TODO: Go through all methods and make sure proper try catches are used.
//TODO: Implement a service that will zip files together. Make a hard disk one and a memory version.
//TODO: Add unit tests using XUnit and NSubstitute.
//TODO: Create a name generating service. Add a feature where the user can write their own prefix, suffix, or entire name for the storage splitters.
//TODO: Move ToDos to a readme file in the library project.

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
    IStoragePdfSplitter pdfSplitter = new FileStoragePdfSplitter(splitRangeParser);
    Attempt<IEnumerable<string>> splitAttempt1 = pdfSplitter.SplitByRanges(inputPdfPath, outputFolderPathForStorageFolder, ranges);
    Attempt<IEnumerable<string>> splitAttempt2 = pdfSplitter.SplitByInterval(inputPdfPath, 15, outputFolderPathForStorageFolder);

    if (!splitAttempt1.Success)
    {
        Console.WriteLine(splitAttempt1.ErrorMessage);
    }

    if (!splitAttempt2.Success)
    {
        Console.WriteLine(splitAttempt2.ErrorMessage);
    }

    IStoragePdfMerger pdfMerger = new FileStoragePdfMerger();
    Attempt<string> mergeAttempt = pdfMerger.Merge(splitAttempt1.Data, outputFolderPathForStorageFolder);

    if (!mergeAttempt.Success)
    {
        Console.WriteLine(mergeAttempt.ErrorMessage);
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
    Attempt<IEnumerable<Stream>> splitAttempt = pdfSplitter.SplitByRanges(inputPdfStream, ranges);

    IMemoryPdfMerger pdfMerger = new MemoryPdfMerger();
    Attempt<Stream> inMemoryMergeAttempt = pdfMerger.Merge(splitAttempt.Data);

    // Open a FileStream to write to the file
    using (FileStream fs = new FileStream($"{outputFolderPath}InMemoryMergeResult_FromSplitByRange_{ranges}.pdf", FileMode.Create, FileAccess.Write))
    {
        // Copy data from MemoryStream to FileStream
        inMemoryMergeAttempt.Data.CopyTo(fs);
        Console.WriteLine("Stream data written to file successfully.");
    }
}

static void InMemorySplitByInternal(FileStream inputPdfStream, string outputFolderPath, ISplitRangeParser splitRangeParser)
{
    int internval = 5;

    IMemoryPdfSplitter pdfSplitter = new MemoryPdfSplitter(splitRangeParser);
    Attempt<IEnumerable<Stream>> splitAttempt = pdfSplitter.SplitByInterval(inputPdfStream, internval);


    int index = 0;
    foreach (Stream pdfStream in splitAttempt.Data)
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