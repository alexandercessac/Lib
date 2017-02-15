using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DancingCube.RequestStuff;
using System.Collections.Generic;
using System.Diagnostics;
using DancingCube.Enums;
using static System.ConsoleColor;

namespace DancingCube
{
    public class ConsoleContent
    {
        public string Words;
        public ConsoleColor Color;
    }

    internal static class Program
    {
        private static bool ChangeState;

        private const int MIN_HEIGHT = 0;
        private const int MIN_WIDTH = 0;
        private const int MAX_HEIGHT = 10;
        private const int MAX_WIDTH = 20;
        private const string Directions = "Enter directions to move (Up, Down, Left, or Right) [Enter] to continue or [Q]/[Esc] to quit";
        private const int MAX_DANCERS = 5;

        private static readonly ConsoleColor[] Colors = {
            Cyan, Green, Magenta, Blue, Red, Yellow
        };

        public static readonly ConsoleColor[] DarkColors = {
            DarkCyan, DarkGreen, DarkMagenta, DarkYellow, DarkRed, DarkBlue
        };
        private static ConsoleColor GetADarkColor() => DarkColors[ColorRnd.Next(0, DarkColors.Length - 1)];

        private static ConsoleColor GetAColor() => Colors[ColorRnd.Next(0, Colors.Length - 1)];
        private static readonly Random ColorRnd = new Random();


        private static readonly ConsoleContent Content = new ConsoleContent
        {
            Words = string.Empty,
            Color = ConsoleColor.White
        };

        //todo: make random quote interval?
        private static readonly Timer ChangeStateInterval = new Timer(x => { lock (STATE_CHANGED_LOCK) ChangeState = true; },
            null, Timeout.Infinite, Timeout.Infinite);

        private static readonly Timer CheckMovesInterval = new Timer(x => ReadMoves(), null, Timeout.Infinite, Timeout.Infinite);

        private static readonly Timer GetDancersInterval = new Timer( x => RefreshDancers(), null, Timeout.Infinite, Timeout.Infinite);

        private static readonly Timer DrawInterval = new Timer(x => Draw(), null, Timeout.Infinite, Timeout.Infinite);

        private static readonly Timer MoveDancersInterval = new Timer(x => MoveDancers(), null, Timeout.Infinite, Timeout.Infinite);


        private static readonly object STATE_CHANGED_LOCK = new object();

        public static readonly object USERS_CHANGED_LOCK = new object();

        private static Point[] Positions() => Floor.Points.Except(Dancers.Where(d => d != null).Select(d => d.Position).Concat(new[] { User.Position })).ToArray();

        public static Plane Floor = new Plane(MAX_WIDTH, MAX_HEIGHT);


        public static string[] Names = { "Ricky", "Randy", "Ryan", "Robert", "Regina" };

        public static Dancer[] Dancers = new Dancer[MAX_DANCERS];

        public static Dictionary<string, Dancer> Users = new Dictionary<string, Dancer>();

        private static Dancer User = new Dancer { Identity = Guid.NewGuid().ToString(), Position = new Point(0, 0), Color = White };

        static readonly Queue<Direction> Q = new Queue<Direction>();

        public static readonly Queue<MoveRequest> Q2 = new Queue<MoveRequest>();

        public static string GameId = Guid.NewGuid().ToString();

        private static void Main(string[] args)
        {
            //Console.CursorVisible = false;
            Console.WriteLine(Console.Title = "Welcome to the Cube Dance!");
            Console.Write("Enter name:");
            User.Name = Console.ReadLine();

            Console.WriteLine("         /-----------------------------------------\\");
            Console.WriteLine("         |            Select Game Mode              |");
            Console.WriteLine("         |  1. Host Game                            |");
            Console.WriteLine("         |  2. Join Game                            |");
            Console.WriteLine("         |  3. Single Player Game                   |");
            Console.WriteLine("         \\-----------------------------------------/");
            var mode = Console.ReadKey().Key;
            do
            {
                switch (mode)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        HostGame();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        JoinGame().Wait();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.Enter:
                    case ConsoleKey.Q:
                        Environment.Exit(0); //quit
                        return;
                    default:
                        Console.WriteLine("invalid choice");
                        mode = Console.ReadKey().Key;
                        break;
                }

            } while (!new[] { ConsoleKey.D1, ConsoleKey.NumPad1, ConsoleKey.D2, ConsoleKey.NumPad2, ConsoleKey.D3, ConsoleKey.NumPad3 }.Contains(mode));

        }

        private static async Task JoinGame()
        {
            Dancers = null;

            do
            {
                Console.WriteLine("Enter Id of game to join: ");
                GameId = Console.ReadLine();

                if (GameId?.ToUpper() == "Q") return;

                Dancers = await RequestClient.GetGame(GameId);
                if(Dancers == null)
                    Console.WriteLine($"No dancers found for gameId: {GameId}");
            } while (Dancers == null);
            //Found a game

            Console.CursorVisible = false;

            var response = await User.JoinGame(GameId);

            if (response == null)
                Debugger.Launch();

            User = response;
            User.Position = new Point(-1, -1); // HACK

            GetDancersInterval.Change(100, 100);
            DrawInterval.Change(100, 100); //start the fun
            while (GetDirection())
            {
                if (Q.Count > 0)
                {   Dancers = await new MoveRequest
                    {
                        Direction = Q.Dequeue(),
                        Identity = User.Identity
                    }.SubmitMove(GameId);
                    lock (STATE_CHANGED_LOCK)
                        ChangeState = true;
                }

            }

        }

        private static void RefreshDancers()
        {
            GetDancersInterval.Change(Timeout.Infinite, Timeout.Infinite);

            RequestClient.GetGame(GameId).ContinueWith(getGameTask =>
            {
                Dancers = getGameTask.Result ?? Dancers;
                lock (STATE_CHANGED_LOCK) ChangeState = true;
                GetDancersInterval.Change(1000, 1000);
            });
        }

        private static void HostGame()
        {
            Console.WriteLine($"Waiting for dancers to join game Id: {GameId}");
            WaitForDancers().Wait();

            Console.Clear();
            Console.WriteLine("Time to dance!");
            Console.CursorVisible = false;
            FillDancers();

            DrawInterval.Change(100, 100); //start the fun
            //ChangeStateInterval.Change(200, 200);
            MoveDancersInterval.Change(2000, 2000);
            CheckMovesInterval.Change(100, 100);

            var cts = new CancellationTokenSource();

            var hostingThings = Task.WhenAll(RequestHandler.HostGameState(GameId, cts.Token), RequestHandler.AwaitUserMovements(cts.Token));

            while (GetDirection())
                if (Q.Count > 0)
                    Move(Q.Dequeue());

            cts.Cancel();

            hostingThings.Wait(15000);
        }

        private static void ReadMoves()
        {
            CheckMovesInterval.Change(Timeout.Infinite, Timeout.Infinite);
            while (Q2.Count > 0)
                TryMoveUser(Q2.Dequeue());

            CheckMovesInterval.Change(100, 100);
        }

        public static bool TryMoveUser(MoveRequest req)
        {
            if (!Users.ContainsKey(req.Identity)) return false;
            lock (USERS_CHANGED_LOCK)
            {
                var p = Users[req.Identity].Position;
                switch (req.Direction)
                {
                    case Direction.Right:
                        p = new Point(p.X + 1, p.Y);
                        break;
                    case Direction.Left:
                        p = new Point(p.X - 1, p.Y);
                        break;
                    case Direction.Down:
                        p = new Point(p.X, p.Y - 1);
                        break;
                    case Direction.Up:
                        p = new Point(p.X, p.Y + 1);
                        break;
                    default:
                        Debugger.Launch(); //How did this happen?!?!
                        break;
                }
                if (!Positions().Contains(p)) //how to notify external end user of collision?
                    return false;

                Users[req.Identity].Position = p;

                lock (STATE_CHANGED_LOCK) ChangeState = true;
                return true;
            }
        }


        private static async Task WaitForDancers()
        {
            Console.WriteLine();

            var cts = new CancellationTokenSource();
            lock (USERS_CHANGED_LOCK)
                Users.Add(User.Identity, User);

            var waitForDancersToJoin = Users.AwaitAdditionalDancers(MAX_DANCERS - 1, cts.Token);

            var waitCount = 0;
            while (!waitForDancersToJoin.IsCompleted && !cts.IsCancellationRequested)
            {
                Console.Write(".");
                waitCount++;
                await Task.Delay(1000);

                if (waitCount <= 10) continue;

                Console.WriteLine();
                Console.Write("No dancers have joined. Keep waiting? Y/N: ");
                if (new[] { 'N', 'n' }.Contains(Console.ReadKey().KeyChar)) cts.Cancel();
                waitCount = 0;
                Console.WriteLine();
            }


            var dancers = Users.Values.ToArray();

            //TODO: how to represent Dancers, Users, and User??
            for (int i = 0; i < dancers.Length && i <= MAX_DANCERS; i++)
                Dancers[i] = dancers[i];
        }

        private static void FillDancers()
        {
            for (var i = 0; i < Dancers.Length; i++)
            {
                Dancers[i] = Dancers[i] ?? new Dancer
                {
                    Name = Names[i],
                    Position = GetRandomUnoccupiedFreeSpace(),
                    Color = DarkColors[i]
                };
            }
        }

        public static Point GetRandomUnoccupiedFreeSpace()
        {
            var spaces = Positions();

            return spaces[new Random().Next(0, spaces.Length - 1)];
        }

        private static void MoveDancers()
        {
            MoveDancersInterval.Change(Timeout.Infinite, Timeout.Infinite);
            foreach (var t in Dancers.Where(d => d.Identity == null))
                t.SafeMove();
            //Dancers[index].Position = Dancers[index].Position.SafeMove();
            //Console.ForegroundColor = RandomConsoleColor();
            lock (STATE_CHANGED_LOCK)
                ChangeState = true;
            MoveDancersInterval.Change(1700, 1700);
        }

        public static void SafeMove(this ITangible thing)
        {

            var emptySpaces = Positions();
            var rnd = new Random();

            for (var i = 1; i < 5; i++)
            {
                var point = thing.Position;

                switch (rnd.Next(1, 5))
                {
                    case 1:
                        point = new Point(point.X + 1, point.Y);
                        break;
                    case 2:
                        point = new Point(point.X - 1, point.Y);
                        break;
                    case 3:
                        point = new Point(point.X, point.Y - 1);
                        break;
                    case 4:
                        point = new Point(point.X, point.Y + 1);
                        break;
                    default:
                        System.Diagnostics.Debugger.Launch();//How did this happen?!?!
                        break;
                }
                if (emptySpaces.Contains(point))
                    thing.Position = point;
            }
        }

        private static void Draw()
        {
            lock (STATE_CHANGED_LOCK)
                if (!ChangeState) return;

            DrawInterval.Change(Timeout.Infinite, Timeout.Infinite);

            Console.Clear();


            Console.ForegroundColor = White;
            Console.WriteLine(Directions);
            var color = Console.ForegroundColor = GetAColor();

            lock (USERS_CHANGED_LOCK)
            {
                for (var y = MAX_HEIGHT; y >= MIN_HEIGHT; y--)
                {
                    for (var x = MIN_WIDTH; x <= MAX_WIDTH; x++)
                    {
                        var point = new Point(x, y);
                        if (point.Equals(User.Position))
                        {
                            Console.ForegroundColor = User.Color;
                            Console.Write("■");
                            Console.ForegroundColor = color;
                        }
                        else
                        {
                            var dancer = Dancers.FirstOrDefault(d => point.Equals(d.Position));
                            if (dancer != null)
                            {
                                Console.ForegroundColor = dancer.Color;
                                Console.Write("■");
                                Console.ForegroundColor = color;
                            }
                            else
                                Console.Write("-");
                        }
                    }
                    Console.WriteLine();
                }
            }


            if (!string.IsNullOrWhiteSpace(Content.Words))
            {
                Console.ForegroundColor = Content.Color;
                Console.WriteLine(Content.Words);
                Console.ForegroundColor = color;
            }

            lock (STATE_CHANGED_LOCK)
                ChangeState = false;

            DrawInterval.Change(10, 100);
        }


        private static ConsoleColor RandomConsoleColor()
        {
            var randomConsoleColor = (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), new Random().Next(0, 11));

            while (randomConsoleColor == Black)
                randomConsoleColor = (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), new Random().Next(0, 11));

            return randomConsoleColor;
        }

        private static void Move(Direction dir)
        {
            var position = User.Position;
            switch (dir)
            {
                case Direction.Up:
                    if (position.Y == MAX_HEIGHT) position.Y = MIN_HEIGHT;
                    else position.Y++;
                    break;
                case Direction.Down:
                    if (position.Y == MIN_HEIGHT) position.Y = MAX_HEIGHT;
                    else position.Y--;
                    break;
                case Direction.Left:
                    if (position.X == MIN_WIDTH) position.X = MAX_WIDTH;
                    else position.X--;
                    break;
                case Direction.Right:
                    if (position.X == MAX_WIDTH) position.X = MIN_WIDTH;
                    else position.X++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
            User.Position = position;

            var dancer = Dancers.FirstOrDefault(d => d.Identity != User.Identity && Equals(d.Position, User.Position));
            if (dancer != null)
            {
                Content.Words = $"{dancer?.Name}: DON'T DANCE ON ME!!!";
                Content.Color = dancer.Color;
            }
            lock (STATE_CHANGED_LOCK)
                ChangeState = true;
        }

        public static bool GetDirection()
        {
            while (true)
            {
                var input = Console.ReadKey(true).Key;
                Content.Words = string.Empty;
                switch (input)
                {
                    case ConsoleKey.Enter:
                        return true;
                    case ConsoleKey.UpArrow:
                        Q.Enqueue(Direction.Up);
                        return true;
                    case ConsoleKey.DownArrow:
                        Q.Enqueue(Direction.Down);
                        return true;
                    case ConsoleKey.LeftArrow:
                        Q.Enqueue(Direction.Left);
                        return true;
                    case ConsoleKey.RightArrow:
                        Q.Enqueue(Direction.Right);
                        return true;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        return false;
                    default:
                        Content.Words = "Invalid direction";
                        break;
                }
            }
        }
    }
}
