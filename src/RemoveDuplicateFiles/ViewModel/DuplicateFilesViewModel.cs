using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveDuplicateFiles.ViewModel
{
    public class DuplicateFilesViewModel : IDuplicateFilesViewModel
    {
        public string SelectedFolderPath { get; set; }
        public Dictionary<string, List<string>> SameFileDictionary { get; set; }
        public Dictionary<string, List<string>> SearchFolderForDuplicates()
        {
            if (string.IsNullOrEmpty(SelectedFolderPath) || !Directory.Exists(SelectedFolderPath)) return default;
            Dictionary<long, List<string>> filesize_dictionary = new Dictionary<long, List<string>>();
            List<string> all_files = new List<string>();
            Dictionary<string, List<string>> matches = new Dictionary<string, List<string>>();

            all_files = Directory.EnumerateFiles(SelectedFolderPath, "*.*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            }).ToList();

            foreach (string filename in all_files)
            {
                var fi = new FileInfo(filename);
                long size = fi.Length;
                if (!filesize_dictionary.ContainsKey(size))
                    filesize_dictionary.Add(size, new List<string>() { filename});
                else
                    filesize_dictionary[size].Add(filename);
            }
            foreach (var item in filesize_dictionary.Where(kvp => kvp.Value.Count==1).ToList())
            {
                filesize_dictionary.Remove(item.Key);
            }
            foreach (var key in filesize_dictionary.Keys)
            {
                var same_size_files = filesize_dictionary[key];

                while (same_size_files.Any())
                {
                    var filename = same_size_files.First();
                    List<string> duplicates = FindMatchingFiles(filename, same_size_files);
                    if (duplicates.Any())
                        matches.TryAdd(filename, duplicates);
                
                    same_size_files.RemoveAll(name => duplicates.Contains(name) || string.Equals(name, filename));
                }
            }


            SameFileDictionary = matches;
            return matches;

        }
        public bool RemoveAllDuplicates()
        {
            if (SameFileDictionary == default) return true;
            bool success = true;
            try
            {
                SameFileDictionary.ToList().ForEach(kvp => kvp.Value.ForEach(f => File.Delete(f)));
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }


        private List<string> FindMatchingFiles(string filename, List<string> filenames)
        {
            List<string> matches = new List<string>();
            FileInfo fi1 = new FileInfo(filename);
            foreach (string fileb in filenames) if (!string.Equals(fileb, filename))
                {
                    FileInfo fi2 = new FileInfo(fileb);
                    if (AreFilesEqual(fi1, fi2))
                        matches.Add(fileb);
                }
            return matches;
        }
        public bool AreFilesEqual(FileInfo fi1, FileInfo fi2)
        {
            if (fi1.Length != fi2.Length) return false;
            if (fi1.Length < 2000000000) return AreFileContentsEqual(fi1, fi2);
            foreach (var block in ReadChunks(fi1.FullName))
            {
                foreach (var blockb in ReadChunks(fi2.FullName))
                {
                    if (!block.SequenceEqual(blockb)) return false;
                   
                }
            }
            return true;
        }
        public bool AreFileContentsEqual(FileInfo fi1, FileInfo fi2) => /*fi1.Length == fi2.Length &&*/
            ( fi1.Length == 0 || File.ReadAllBytes(fi1.FullName).SequenceEqual(File.ReadAllBytes(fi2.FullName)));



        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/54eb346f-f979-49fb-aa2d-44dddad066bd/how-to-read-file-in-chunks-with-for-loop?forum=netfxbcl

        public IEnumerable<byte[]> ReadChunks(string path)
        {

            // FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var FS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long FSBytes = FS.Length;

                int ChunkSize = 2 << 8;

                byte[] B = new byte[ChunkSize];

                int Pos;

                for (Pos = 0; Pos < (FSBytes - ChunkSize); Pos += ChunkSize)

                {

                    FS.Read(B, 0, ChunkSize);

                    //  Write(B);
                    yield return B;
                }
                B = new byte[FSBytes - Pos];

                FS.Read(B, 0, (int)(FSBytes - Pos));
                yield return B;
            }

        }
    }

}


/* public bool CompareFiles(string first_filename, string second_filename)
       {

           FileInfo first_file_info = new FileInfo(first_filename);
           FileInfo second_file_info = new FileInfo(second_filename);
           if (first_file_info.Exists
           if (!second_file_info.Exists) return false;
           if (first_file_info.Length != second_file_info.Length) return false;

           {
               // Get file size  
               long size = fi.Length;
           }
       }
       public static bool AreFileContentsEqual(String path1, String path2) =>
           File.ReadAllBytes(path1).SequenceEqual(File.ReadAllBytes(path2));*/