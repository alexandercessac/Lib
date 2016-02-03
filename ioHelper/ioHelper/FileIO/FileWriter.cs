using System.IO;
using System.Text;

namespace ioHelper.FileIO
{
    public static class FileWriter
    {
        public static void WriteStringToFileAsBytes(string data, string fullPath)
        {
            var tmp = Encoding.UTF8.GetBytes(data);

            FileIoHelper.WriteData(fullPath, tmp);
        }

        public static void WriteStringToFileWithStreamWritter(string data, string fullPath)
        {
            using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
                sw.Write(data);
        }
    }
}