using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ioHelper;
using ioHelper.FileIO;

namespace AsyncFileManager
{
    public class FileClient
    {
        public bool ReadingFiles { get; private set; }
        public bool WritingFiles { get; private set; }

        public DirectoryInfo InputDir { get; set; }
        public DirectoryInfo OutpuDir { get; set; }

        public List<FileClientContents> ReadData { get; private set; }
        public List<FileClientContents> WriteData { get; private set; }


        public string SearchPattern = "*.*";

        public FileClient(DirectoryInfo inputDir, DirectoryInfo outputDir)
        {
            InputDir = inputDir;
            OutpuDir = outputDir;

            ReadData = new List<FileClientContents>();
        }

        public FileClient(string inputDir, string outputDir)
        {
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            InputDir = new DirectoryInfo(inputDir);
            OutpuDir = new DirectoryInfo(outputDir);

            ReadData = new List<FileClientContents>();
        }

        public void ReadAll()
        {
            ReadingFiles = true;
            ReadData.Clear();

            var results = InputDir.GetFiles(SearchPattern).Select(ReadFile);

            Task.WaitAll(results.ToArray());

            ReadingFiles = false;

        }

        public void WriteAll()
        {
            if (WriteData != new List<FileClientContents>() && WriteData != null && !WritingFiles)
            {
                WritingFiles = true;

                var results = WriteData.Select(WriteFile);

                Task.WaitAll(results.ToArray());

                WritingFiles = false;
            }
        }

        public Task ReadAllAsync()
        {
            return Task.Run(() => ReadAll());
        }

        private Task WriteFile(FileClientContents myFile)
        {
            return Task.Run(() => FileWriter.WriteStringToFileWithStreamWritter(myFile.FileContents, myFile.FileName));
        }

        private Task ReadFile(FileSystemInfo myFile)
        {
            return Task.Run(() => ReadData.Add(new FileClientContents
            {
                FileName = myFile.Name,
                FileContents = FileReader.ReadFileToString(myFile.FullName)
            }));
        }
    }

    public class FileClientContents
    {
        public string FileName { get; set; }
        public string FileContents { get; set; }

    }
}
