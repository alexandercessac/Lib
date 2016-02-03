using System.Linq;
using AsyncFileManager;
namespace AsyncFileManagerTest
{
    class Program
    {
        static void Main()
        {
            const string i = @"C:\Code\mycode\Lib\ioHelper\ioHelperTests\bin\Debug\New folder";
            const string o = @"C:\Code\mycode\Lib\ioHelper\ioHelperTests\bin\Debug\New folder\output";

            var helper = new FileClient(i,o);

           //DoRead(helper);

            DoWrite(helper);

        }

        private static void DoWrite(FileClient helper)
        {


            var tmp = "";

            for (var i = int.MinValue; i < int.MaxValue; i++)
                tmp += "0";

            var myContent = new FileClientContents {FileName = "bigFile.txt", FileContents = tmp};

            helper.WriteData.Add(myContent);

            helper.WriteAll();
        }

        private static void DoRead(FileClient helper)
        {
            helper.ReadAll();

            if (!helper.ReadingFiles)
            {
                var tmp = helper.ReadData;
            }

        }
    }
}
