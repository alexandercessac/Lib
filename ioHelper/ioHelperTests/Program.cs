using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ioHelper.FileIO;

namespace ioHelperTests
{
    class Program
    {
        const string StringToWrite = "lolololol\r\nlawlawlawlawlawl\r\ntrololol";

        static void Main(string[] args)
        {


            Dowork("tmpfile4.txt", Encoding.UTF8);
           
            //FileIoHelper.WriteData("tmpfile2.txt", GetTestData());

            //var dataToWrite = Encoding.UTF8.GetString(GetTestData());


            //TestWrite(dataToWrite);

            //            var readList = FileIoHelper.ReadList("unicodeHex.txt");



            //FileWriter.WriteStringToFile(GetAllPrintableChars(), "tmpFile4.txt");//works
            //FileWriter.WriteStringToFileWithStreamWritter(GetAllPrintableChars(), "tmpFile5.txt");//works better



            //debug;eval
            //var contents = TestRead("tmpFile5.txt");
            //var contentsList = TestReadList();
            //var easyContents = TestReadList();
        }

        private static void Dowork(string fileName, Encoding myEncoding)
        {
            var filemode = File.Exists(fileName) ? FileMode.Truncate : FileMode.CreateNew;

            using (var fs = new FileStream(fileName, filemode))
            using (var sw = new StreamWriter(fs, myEncoding))
            {
                var tmp = GetAllPrintableChars();
                sw.Write(tmp);
            }


            var result = "";

            var tmpbytes = FileIoHelper.ReadBytes(fileName);

            using (var fs = new FileStream(fileName, FileMode.Open))
            using (var sr = new StreamReader(fs, myEncoding))
                result = sr.ReadToEnd();

        }

        private static string GetAllPrintableChars()
        {
            var printableChars = new List<char>();
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                if (printableChars.Count == 63730)
                {
                    
                }

                char c = Convert.ToChar(i);

                if (c == 'ﰚ')
                {
                    
                }
                if (!char.IsControl(c))
                {
                    printableChars.Add(c);
                }
            }
            return new string(printableChars.ToArray());
        }

        private static string ConvertListOfHexToChar(IEnumerable<string> data)
        {
            var retString = "";
            foreach (var dataItem in data)
            {
                var x = Convert.ToInt32(dataItem, 16);

                if (x >= 0xD800 && x <= 0xDFFF) continue;

                retString += Char.ConvertFromUtf32(x);
            }
            return retString;

        }

        private static byte[] GetTestData()
        {
            return FileIoHelper.ReadBytes("unicodeHex.txt"); ;
        }

        //private static object TestReadList()
        //{
        //    return FileReader.ReadFileToEnumerable("tmpFile.txt");
        //}

        private static void TestWrite(string data = StringToWrite)
        {


            FileWriter.WriteStringToFileWithStreamWritter(StringToWrite, "tmpFile.txt");
        }

        private static string TestRead(string file)
        {
            return FileReader.ReadFileToString(file);
        }
    }
}
