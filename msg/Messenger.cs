using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace msg
{
    public static class Messenger
    {
        private static readonly string Dir = $"{Program.workingDir}\\messages";
        private static readonly string Unread = $"{Program.workingDir}\\messages\\Unread";
        private static readonly string Read = $"{Program.workingDir}\\messages\\Read";

        static Messenger()
        {
            if (!System.IO.Directory.Exists(Dir)) System.IO.Directory.CreateDirectory(Dir);
            if (!System.IO.Directory.Exists(Unread)) System.IO.Directory.CreateDirectory(Unread);
            if (!System.IO.Directory.Exists(Read)) System.IO.Directory.CreateDirectory(Read);
        }


        public static IEnumerable<Message> ReadMessages()
        {
            foreach (var f in Directory.EnumerateFiles(Unread))
            {
                var msg = new FileInfo(f).GetMessage();
                yield return msg;
                msg.MarkRead();
            }

        }

        public static void MarkRead(this Message msg)
        {
            var fileInfo = new FileInfo(msg.Path);
            var fromPath = Path.Combine(Read, msg.From);

            if (!Directory.Exists(fromPath))
                Directory.CreateDirectory(fromPath);

            fileInfo.MoveTo(Path.Combine(fromPath, msg.Title).GetUniquePath());
        }

        public static void Save(this Message msg)
        {

            var fullPath = Path.Combine(Unread, msg.Title);

            var fi = new FileInfo(fullPath.GetUniquePath());
            
            using (var sw = fi.CreateText())
            {
                sw.WriteLine($"From: {msg.From}");
                sw.Write(msg.Body);
            }
            msg.Path = fi.FullName;

        }

        private static Message GetMessage(this FileInfo fi)
        {
            var msg = new Message
            {
                Path = fi.FullName,
                Title = fi.Name
            };
            using (var fs = fi.OpenRead())
            using (var sr = new StreamReader(fs))
            {
                msg.From = Regex.Replace(sr.ReadLine() ?? "", "From: ", "");

                msg.Body = sr.ReadToEnd();
            }

            return msg;

        }

        private static string GetUniquePath(this string path)
        {
            var i = 1;
            var unique = path;
            while (File.Exists(unique))
                unique = $"{path}_{i}";

            return unique;
        }

    }
}
