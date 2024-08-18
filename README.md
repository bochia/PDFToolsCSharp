# PDFTools.SplitMerge Nuget Package
The page is dedicated to showing some how the PDFTools.SplitMerge nuget package can be used. We'll go over some real code examples and outputs.

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


## Merge PDFs
### In Memory Example
### Local Storage Example
