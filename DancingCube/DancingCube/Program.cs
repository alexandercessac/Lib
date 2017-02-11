using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using static System.ConsoleColor;

namespace DancingCube
{
    public class Dancer : ITangible
    {
        public string Name;
        public ConsoleColor Color;
        public Point Position { get; set; }
    }

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

        private static readonly ConsoleColor[] Colors = {
            Cyan, Green, Magenta, Blue, Red, Yellow
        };

        private static readonly ConsoleColor[] DarkColors = {
            DarkCyan, DarkGreen, DarkMagenta, DarkYellow, DarkRed, DarkBlue
        };
        private static ConsoleColor GetADarkColor() => DarkColors[ColorRnd.Next(0, DarkColors.Length - 1)];

        private static ConsoleColor GetAColor() => Colors[ColorRnd.Next(0, Colors.Length - 1)];
        private static readonly Random ColorRnd = new Random();


        private static ConsoleContent _content = new ConsoleContent
        {
            Words = string.Empty,
            Color = ConsoleColor.White
        } ;

        //todo: make random quote interval?
        private static readonly Timer ChangeStateInterval = new Timer(x => { lock (STATE_CHANGED_LOCK) ChangeState = true; },
            null, Timeout.Infinite, Timeout.Infinite);

        private static readonly Timer DrawInterval = new Timer(x => Draw(), null, Timeout.Infinite, Timeout.Infinite);

        private static readonly Timer MoveDancersInterval = new Timer(x => MoveDancers(), null, Timeout.Infinite, Timeout.Infinite);


        private static readonly object STATE_CHANGED_LOCK = new object();
        private static Point[] Positions() => Floor.Points.Except(Dancers.Where(d => d != null).Select(d => d.Position).Concat(new[] { Cube.Position })).ToArray();
        public static Plane Floor = new Plane(MAX_WIDTH, MAX_HEIGHT);

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public static string[] Names = { "Ricky", "Randy", "Ryan", "Rue", "Regina" };
        public static Dancer[] Dancers;

        private static readonly Dancer Cube = new Dancer
        {
            Position = new Point(0, 0)
        };

        static Queue<Direction> Q = new Queue<Direction>();

        static void Main(string[] args)
        {
            Console.WriteLine(Console.Title = "Welcome to the Cube Dance!");

            Dancers = new Dancer[5];
            var rnd = new Random();
            for (var i = 0; i < Names.Length; i++)
            {
                Dancers[i] = new Dancer
                {
                    Name = Names[i],
                    Position = GetRandomUnoccupiedFreeSpace(),
                    Color = DarkColors[i]
                };
            }

            DrawInterval.Change(100, 100);//start the fun
            //ChangeStateInterval.Change(200, 200);
            MoveDancersInterval.Change(2000, 2000);
            while (true)
            {
                GetDirection();
                if (Q.Count > 0) Move(Q.Dequeue());
            }

        }

        public static Point GetRandomUnoccupiedFreeSpace()
        {
            var spaces = Positions();

            return spaces[new Random().Next(0, spaces.Length - 1)];
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
                        point = new Point(point.X, point.Y -1);
                        break;
                    case 4:
                        point = new Point(point.X, point.Y+1);
                        break;
                    default:
                        System.Diagnostics.Debugger.Launch();//How did this happen?!?!
                        break;
                }
                if (emptySpaces.Contains(point))
                    thing.Position = point;
            }
        }

        private static void MoveDancers()
        {
            MoveDancersInterval.Change(Timeout.Infinite, Timeout.Infinite);
            for (var index = 0; index < Dancers.Length; index++)
                Dancers[index].SafeMove();
            //Dancers[index].Position = Dancers[index].Position.SafeMove();
            //Console.ForegroundColor = RandomConsoleColor();
            lock (STATE_CHANGED_LOCK)
                ChangeState = true;
            MoveDancersInterval.Change(1700, 1700);
        }

        private static void Draw()
        {

            //if (myState.Color == Console.ForegroundColor && Equals(myState.Location, _location)) return;
            lock (STATE_CHANGED_LOCK)
                if (!ChangeState) return;

            DrawInterval.Change(Timeout.Infinite, Timeout.Infinite);

            Console.Clear();


            Console.ForegroundColor = White;
            Console.WriteLine(Directions);
            var color = Console.ForegroundColor = GetAColor();

            for (var y = MAX_HEIGHT; y >= MIN_HEIGHT; y--)
            {
                for (var x = MIN_WIDTH; x <= MAX_WIDTH; x++)
                {
                    var point = new Point(x, y);
                    if (point.Equals(Cube.Position))
                    {
                        Console.ForegroundColor = White;
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

            if (!string.IsNullOrWhiteSpace(_content.Words))
            {
                Console.ForegroundColor = _content.Color;
                Console.WriteLine(_content.Words);
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
            var position = Cube.Position;
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
            Cube.Position = position;

            var dancer = Dancers.FirstOrDefault(d => Equals(d.Position, Cube.Position));
            if (dancer != null)
            {
                _content.Words = $"{dancer?.Name}: DO NOT DANCE ON ME!!!";
                _content.Color = dancer.Color;
            }
            lock (STATE_CHANGED_LOCK)
                ChangeState = true;
        }

        public static void GetDirection()
        {
            while (true)
            {
                var input = Console.ReadKey(true).Key;
                _content.Words = string.Empty;
                switch (input)
                {
                    case ConsoleKey.Enter:
                        return;
                    case ConsoleKey.UpArrow:
                        Q.Enqueue(Direction.Up);
                        return;
                    case ConsoleKey.DownArrow:
                        Q.Enqueue(Direction.Down);
                        return;
                    case ConsoleKey.LeftArrow:
                        Q.Enqueue(Direction.Left);
                        return;
                    case ConsoleKey.RightArrow:
                        Q.Enqueue(Direction.Right);
                        return;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        return;
                    default:
                        _content.Words = "Invalid direction";
                        break;
                }
            }
        }

        public static void GetDirections()
        {
            while (true)
            {
                var input = Console.ReadKey(true).Key;
                switch (input)
                {
                    case ConsoleKey.Enter: return;
                    case ConsoleKey.UpArrow:
                        Q.Enqueue(Direction.Up);
                        return;
                    case ConsoleKey.DownArrow:
                        Q.Enqueue(Direction.Down);
                        return;
                    case ConsoleKey.LeftArrow:
                        Q.Enqueue(Direction.Left);
                        return;
                    case ConsoleKey.RightArrow:
                        Q.Enqueue(Direction.Right);
                        return;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid direction");
                        break;
                }
            }
        }
    }
}
