using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioHelper.FileIO
{
    public static class FileReader
    {
        //public static Task<string> ReadFileToStringAsync(string fullPath)
        //{
            
        //}

        public static string ReadFileToString(string fullPath)
        {
            var bytes = FileIoHelper.ReadBytes(fullPath);

            //Convert to string
            string retVal;
            if (!TryGetString(bytes, out retVal))
                retVal = string.Format("{0} {1}", fullPath, retVal);
            return retVal;
        }

        public static IEnumerable<string> ReadFileToEnumerable(string fullPath)
        {
            return new FileIoHelper().ReadList(fullPath);
        }


        private static bool TryGetString(byte[] data, out string retVal)
        {


            if (data == null)
            {
                retVal = "No data found.";
                return false;
            }

            try
            {
                var memoryStream = new MemoryStream(data);
                retVal = new StreamReader(memoryStream, Encoding.UTF8).ReadToEnd();
                //retVal = Encoding.UTF8.GetString(data);
            }
            catch
            {
                retVal = "Invalid encoding";
                return false;
            }

            return true;
        }

        //public static List<string> ReadFileToStringList(string fullPath)
        //{
        //    return ReadFileToString(fullPath).Split(new[] { '\r', '\n' }).ToList();
        //}


    }
}
