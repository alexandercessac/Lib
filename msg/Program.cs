using System;

namespace msg
{
    public class Program
    {
        public static string workingDir = $"{AppDomain.CurrentDomain.BaseDirectory}\\msgData";

        static void Main(string[] args)
        {
            Listener.Start();
            Console.WriteLine(Listener.MyUri);
            do
            {
                Console.WriteLine("#############################################");
                Console.WriteLine("Test [L]og");
                Console.WriteLine("[S]end Message  (Comming soon!)");
                Console.WriteLine("[R]ead Messages (Comming soon)");
                Console.WriteLine("E[x]it");
                Console.WriteLine("#############################################");
                Console.Write("What to do?: ");

                var input = Console.ReadLine();

                switch(input.Trim().ToLower())
                {
                    case "l":
                        Log.Error("Woops!");
                        break;
                    case "x":
                        Listener.Stop();
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                        break;
                    case "w":
                        Console.Write("Enter title for new message: ");
                        var t = Console.ReadLine();
                        Console.Write("Enter body of new message: ");
                        var b = Console.ReadLine();
                        new Message
                        {
                            From = "Me",
                            Title = t,
                            Body = b
                        }.Save();
                        break;
                    case "r":
                        foreach (var msg in Messenger.ReadMessages())
                        {
                            Console.WriteLine($"##### {msg.Title}#####");
                            Console.WriteLine($"From: {msg.From}");
                            Console.WriteLine(msg.Body);

                            //todo: handle reply
                            Console.ReadLine();
                        }
                        break;
                    case "s":
                        Console.WriteLine("Comming soon!");
                        break;
                    default:
                        Console.WriteLine("Invalid input!");
                        break;
                }
            } while(true);

        }
    }
}
