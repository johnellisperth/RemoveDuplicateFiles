using System.Collections.Generic;
using System.IO;

namespace RemoveDuplicateFiles.ViewModel
{
    public interface IDuplicateFilesViewModel
    {
        string SelectedFolderPath { get; set; }
        Dictionary<string, List<string>> SameFileDictionary { get; set; }
        bool AreFileContentsEqual(FileInfo fi1, FileInfo fi2);
        Dictionary<string, List<string>> SearchFolderForDuplicates();

        bool RemoveAllDuplicates();
    }
}