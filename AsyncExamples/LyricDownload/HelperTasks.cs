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

                var fullPath = string.Format("{0}/{1}.html", dir, filename);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                using (var sr = new StreamWriter(fs))
                {
                    var writeTask = sr.WriteAsync(fileContents);
                    
                    //Pretend this takes awhile
                    Task.WaitAll(writeTask, Task.Delay(25000));
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

            var fullPath = string.Format("{0}/{1}.html", dir, filename);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            using (var sr = new StreamWriter(fs))
                await sr.WriteAsync(fileContents);

            await Task.Delay(25000);
            return fullPath;
        }
    }
}