using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ioHelper.FileIO
{
    public class FileIoHelper
    {
        public Encoding EncodingFormat;

        public FileIoHelper(Encoding encoding)
        {
            EncodingFormat = encoding;
        }

        public FileIoHelper()
            : this(Encoding.UTF8)
        {
        }

        public IEnumerable<string> ReadList(string filename)
        {
            return ReadListfunc(filename, EncodingFormat);
        }

        //Get FileStream with options
        public static readonly Func<string, FileMode, FileAccess, FileStream> Fs = (fullFilePath, fileModeOption, fileAccessOption) => new FileStream(fullFilePath, fileModeOption, fileAccessOption);

        //Overloads for default Reader/Writer options
        public static readonly Func<string, FileStream> FsWriter = fullFilePath => Fs(fullFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        public static readonly Func<string, FileStream> FsReader = fullFilePath => Fs(fullFilePath, FileMode.Open, FileAccess.Read);


        public static readonly Action<FileStream, MemoryStream> CopyToMemStreamAsync = (fs, ms) => { using (fs) fs.CopyToAsync(ms); };
        public static readonly Action<FileStream, MemoryStream> CopyToMemStream = (fs, ms) => { using (fs) fs.CopyTo(ms); };
        public static readonly Action<MemoryStream, FileStream> CopyToFileStream = (ms, fs) => { using (ms) ms.CopyTo(fs); };

        public static readonly Func<StreamReader, string> ReadFromStream = (sr) => sr.ReadLine();

        public static readonly Func<StreamReader, IEnumerable<string>> ReadLinesFromStream = (sr) =>
        {
            string myLine; var retVal = new List<string>();
            while ((myLine = sr.ReadLine()) != null)
                retVal.Add(myLine);
            return retVal;

        };


        public static readonly Action<MemoryStream, FileStream> WriteToFileStream = (ms, fs) =>
        {
            int len;
            var buffer = new byte[128];
            while ((len = ms.Read(buffer, 0, buffer.Length)) > 0)
                fs.Write(buffer, 0, len);

        };

        public static Action<string, byte[]> WriteData = (fullPath, data) =>
        {

            using (var ms = new MemoryStream(data))
            {


                using (var fs = Fs(fullPath, FileMode.Truncate, FileAccess.ReadWrite))
                {
                    //fs.Write(data, 0, data.Length);
                    WriteToFileStream(ms, fs);
                }
            }


        };

        

        public static readonly Func<string, Encoding, IEnumerable<string>> ReadListfunc =
            delegate(string filePath, Encoding encoding)
            {
                var retVal = new List<string>();

                using (var fs = FsReader(filePath))
                using (var sr = new StreamReader(fs, encoding))
                    retVal.AddRange(ReadLinesFromStream(sr));
                return retVal;
            };

        //public static Task<byte[]> ReadBytesAsync(string fullPath)
        //{

        //}

        public static readonly Func<string, byte[]> ReadBytes = fullPath =>
        {
            byte[] retVal;

            using (var ms = new MemoryStream())
            {

                CopyToMemStream(Fs(fullPath, FileMode.Open, FileAccess.Read), ms);
                //CopyToMemStream(FsReader(fullPath), ms);

                retVal = ms.ToArray();
            }

            return retVal;
        };




    }
}