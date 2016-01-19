using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LyricDownload
{
    public static class HelperTasks
    {
        /// <summary>
        /// Using Task.Run()
        /// </summary>
        /// <param name="fileContents">string to be written to the file</param>
        /// <param name="dir">target directory</param>
        /// <param name="filename">name of .html file to write</param>
        /// <returns>full name of created file</returns>
        public static Task<string> Write(string fileContents, string dir, string filename)
        {
            //Create task to return
            return Task.Run(() =>
            {
                Directory.CreateDirectory(dir);

                var fullPath = string.Format("{0}/{1}.html", dir, filename);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                using (var sr = new StreamWriter(fs))
                {
                    //must wait on async task to prevent closing
                    //stream before sr is finished writting
                    sr.WriteAsync(fileContents).Wait();
                }
                //Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "--" + fullPath);
                return fullPath;
            });

        }

        /// <summary>
        /// Using async keyword
        /// </summary>
        /// <param name="fileContents">string to be written to the file</param>
        /// <param name="dir">target directory</param>
        /// <param name="filename">name of .html file to write</param>
        /// <returns>full name of created file</returns>
        public static async Task<string> AsyncWrite(string fileContents, string dir, string filename)
        {
            Directory.CreateDirectory(dir);

            var fullPath = string.Format("{0}/{1}.html", dir, filename);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            using (var sr = new StreamWriter(fs))
                await sr.WriteAsync(fileContents);
            return fullPath;
        }

        public static string SignThread(string stringToSign)
        {
            return string.Format("{0}--[{1}]", stringToSign, Thread.CurrentThread.ManagedThreadId);

        }
    }
}