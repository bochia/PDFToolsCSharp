# PDFTools.SplitMerge Nuget Package
On this page you will find real code examples of how to use the PDFTools.SplitMerge nuget package.

## Split By Ranges (Local Storage)
### Code
```csharp
ISplitRangeParser splitRangeParser = new SplitRangeParser();
IStoragePdfSplitter pdfSplitter = new FileStoragePdfSplitter(splitRangeParser);
Attempt<IEnumerable<string>> splitAttempt1 = pdfSplitter.SplitByRanges(inputPdfPath, outputFolderPath, "1,3-5,8-16");

if (!splitAttempt1.Success)
{
    Console.WriteLine(splitAttempt1.ErrorMessage);
}
```

### Output
![image](https://github.com/user-attachments/assets/3b4e0435-ec5c-4a06-aab4-d87aa0b3bb85)

## Split By Ranges (In Memory)
### Code
```csharp
ISplitRangeParser splitRangeParser = new SplitRangeParser();
IMemoryPdfSplitter pdfSplitter = new MemoryPdfSplitter(splitRangeParser);
Attempt<IEnumerable<Stream>> splitAttempt = pdfSplitter.SplitByRanges(inputPdfStream, "1,3-5,8-16");

if (!splitAttempt.Success)
{
    Console.WriteLine(splitAttempt.ErrorMessage);
}
```

## Split By Interval (Local Storage)
### Code
```csharp
ISplitRangeParser splitRangeParser = new SplitRangeParser();
IStoragePdfSplitter pdfSplitter = new FileStoragePdfSplitter(splitRangeParser);

Attempt<IEnumerable<string>> splitAttempt = pdfSplitter.SplitByInterval(inputPdfPath, 15, outputFolderPath);

if (!splitAttempt.Success)
{
    Console.WriteLine(splitAttempt.ErrorMessage);
}
```

### Output
![image](https://github.com/user-attachments/assets/70a58081-ff4e-4cc2-a297-902ab57ad5ee)


## Split By Interval (In Memory)
### Code
```csharp
ISplitRangeParser splitRangeParser = new SplitRangeParser();
IMemoryPdfSplitter pdfSplitter = new MemoryPdfSplitter(splitRangeParser);
Attempt<IEnumerable<Stream>> splitAttempt = pdfSplitter.SplitByInterval(inputPdfStream, 5);

if (!splitAttempt.Success)
{
    Console.WriteLine(splitAttempt.ErrorMessage);
}
```

## Merge (Local Storage)
### Code
```csharp
List<string> pdfPaths = new List<string>();

IStoragePdfMerger pdfMerger = new FileStoragePdfMerger();
Attempt<string> mergeAttempt = pdfMerger.Merge(pdfPaths, outputFolderPath);

if (!mergeAttempt.Success)
{
    Console.WriteLine(mergeAttempt.ErrorMessage);
}
```

### Output
![image](https://github.com/user-attachments/assets/184774ce-4700-4dc0-977c-dde359cfc890)


## Merge (In Memory)
### Code
```csharp
List<Stream> pdfStreams = new List<Stream>();

IMemoryPdfMerger pdfMerger = new MemoryPdfMerger();
Attempt<Stream> mergeAttempt = pdfMerger.Merge(pdfStreams);

if (!mergeAttempt.Success)
{
    Console.WriteLine(mergeAttempt.ErrorMessage);
}
```

