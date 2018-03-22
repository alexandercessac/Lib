using System;

namespace msg
{
    public static class Log
    {
        private static readonly string Dir = $"{Program.workingDir}\\log";
        private static readonly string ErrorFile = $"{Dir}\\Errors";
        
        static Log()
        {
            if(!System.IO.Directory.Exists(Dir))
                System.IO.Directory.CreateDirectory(Dir);
        }

        public static void Error(string msg)
        {
            System.IO.File.AppendAllText(ErrorFile, $"{Environment.NewLine}[{System.DateTime.Now}] {msg}");
        }
    }
}