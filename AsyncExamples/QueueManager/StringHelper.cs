using System;
using System.Collections.Generic;

namespace QueueManager
{
    public static class StringHelper
    {
        public static void WriteListToConsole(List<string> vals)
        {
            var val = "";
            if (vals.Count <= 0) return;
            vals.ForEach(x => val += $"{x}, ");
            Console.WriteLine(val.Trim().Trim(','));
        }
    }
}
