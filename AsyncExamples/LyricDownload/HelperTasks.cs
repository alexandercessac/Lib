using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LyricDownload
{
    public static class HelperTasks
    {
        /// <summary>
        /// Using TaskFactory.StartNew()
        /// </summary>
        /// <param name="fileContents"></param>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Task<string> Write(string fileContents, string dir, string filename)
        {
            //Create task to return
            return new TaskFactory<string>().StartNew(() =>
            {
                Directory.CreateDirectory(dir);

                var fullPath = dir + "/" + filename + ".html";
                
                using (var fs = new FileStream(fullPath, FileMode.Create))
                using (var sr = new StreamWriter(fs))
                {
                    var writeTask = sr.WriteAsync(fileContents);
                    writeTask.Wait();
                }
                Console.WriteLine(Thread.CurrentThread.Name + "--" + fullPath );
                return fullPath;
            });

        }

        /// <summary>
        /// Using async keyword
        /// </summary>
        /// <param name="fileContents"></param>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<string> AsyncWrite(string fileContents, string dir, string filename)
        {
            Directory.CreateDirectory(dir);

            var fullPath = dir + "/" + filename + ".html";

            using (var fs = new FileStream(fullPath, FileMode.Create))
            using (var sr = new StreamWriter(fs))
                await sr.WriteAsync(fileContents);

            return fullPath;
        }
    }
}