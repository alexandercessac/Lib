using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SpaceQuest
{

    public interface IThing
    {
        ConsoleColor Color { get; }
        char Shape { get; }
        bool HasMass { get; }

        void Draw(Action<char> output);
    }
    public class Thing : IThing
    {
        public virtual ConsoleColor Color => ConsoleColor.Black;
        public virtual char Shape => ' ';
        public virtual bool HasMass => false;

        public virtual void Draw(Action<char> output) => output(Shape);
    }

    public class Star : IThing
    {
        public char Shape => '.';
        public bool HasMass => true;
        public ConsoleColor Color => ConsoleColor.Yellow;
        public virtual void Draw(Action<char> output) => output(Shape);
    }

    public class Program
    {
        public static Dictionary<ConsoleKey, System.Action> Commands = new Dictionary<ConsoleKey, System.Action>
        {
            { ConsoleKey.UpArrow, ()=> Direction.Up.Move()},
            { ConsoleKey.DownArrow, ()=> Direction.Down.Move()},
            { ConsoleKey.LeftArrow, ()=> Direction.Left.Move()},
            { ConsoleKey.RightArrow, ()=> Direction.Right.Move()},
        };


        public enum Direction { Up, Down, Left, Right };

        public enum Action
        {
            Move,
            Shoot
        }

        private static void Main(string[] args)
        {
            InitMap();

            Console.CancelKeyPress += (sender, eventArgs) => Environment.Exit(0);

            ConsoleKey key;
            while (!(key = Console.ReadKey().Key).Equals(ConsoleKey.Q))
            {
                key.HandleKey();
                State.GetArea().Draw();
            }


        }

        private static void InitMap() => Map.Init(Enumerable.Repeat(new Star(), 1000).ToArray());
    }

    public static class Map
    {
        private static readonly object MapLock = new object();
        private const int MaxWidth = 500;
        private const int MaxHeight = 500;
        public static IThing[,] Things = new IThing[MaxWidth, MaxHeight];

        public static void Draw(this Area area) => Draw(area.Min, area.Max);
        public static void Draw() => Draw(new Location(0, 0), new Location(MaxWidth, MaxHeight));
        public static void Draw(Location min, Location max)
        {
            lock (MapLock)
            {
                var loc = State.UserLocation;
                var sb = new StringBuilder();
                for (var y = min.Y; y < max.Y; y++)
                {
                    if (y < 0 || y > MaxHeight) continue;
                    for (var x = min.X; x < max.X; x++)
                    {
                        if (x < 0 || x > MaxWidth) continue;
                        Things[x, y].Draw(c => sb.Append(c));
                    }
                    sb.Append(Environment.NewLine);
                }
                sb.Append(Environment.NewLine);
                var format = sb.ToString();
                Console.Clear();
                Console.Write(format);
            }
        }

        public static void Init(IThing[] things)
        {
            var rem = 0;
            var set = things.Length;
            var interval = Math.DivRem(MaxHeight * MaxWidth, things.Length, out rem);

            var i = 0;
            for (var y = 0; y < MaxHeight; y++)
                for (var x = 0; x < MaxWidth; x++)
                {
                    if (++i == interval)
                    {
                        Things[x, y] = things[--set];
                        i = 0;
                    }
                    else
                        Things[x, y] = new Thing();
                }

            var tmp = things.Where(x => x is Star).ToArray();
        }
    }

    public class User : IThing
    {
        public Program.Direction Direction;
        public ConsoleColor Color => ConsoleColor.DarkGreen;

        public char Shape
        {
            get
            {
                switch (Direction)
                {
                    case Program.Direction.Up:
                        return '^';
                    case Program.Direction.Down:
                        return 'v';
                    case Program.Direction.Left:
                        return '<';
                    case Program.Direction.Right:
                        return '>';
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool HasMass => true;
        public void Draw(Action<char> output) => output(Shape);
    }

    public static class State
    {
        private const int WindowSize = 20;
        public static User User = new User();
        public static Area GetArea() => new Area(UserLocation, WindowSize);
        private static readonly object LocationLock = new object();

        public static Location UserLocation = new Location(0,0);

        public static void Move(this Program.Direction direction)
        {
            var tmp = UserLocation;
            lock (LocationLock)
            {
                User.Direction = direction;
                UserLocation = UserLocation.Move(direction);
                if (Map.Things[UserLocation.X, UserLocation.Y].HasMass)
                    throw new Exception("Ya done crashed boi!");
                else
                {
                    Map.Things[UserLocation.X, UserLocation.Y] = User;
                    Map.Things[tmp.X, tmp.Y] = new Thing();
                }
            }

            
        }
    }

    public static class Helpers
    {
        public static Location Move(this Location location, Program.Direction direction)
        {
            switch (direction)
            {
                case Program.Direction.Up:
                    return new Location(location.X, location.Y + 1);
                case Program.Direction.Down:
                    return new Location(location.X, location.Y - 1);
                case Program.Direction.Left:
                    return new Location(location.X - 1, location.Y);
                case Program.Direction.Right:
                    return new Location(location.X + 1, location.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static void HandleKey(this ConsoleKey key)
        {
            if (Program.Commands.ContainsKey(key))
                Program.Commands[key]?.Invoke();
        }
    }

    public struct Area
    {
        public readonly Location Min;
        public readonly Location Max;

        public Area(Location center, int size)
        {
            Max = new Location(center.X + size, center.Y + size);
            Min = new Location(center.X - size, center.Y - size);
        }

        public Area(Location min, Location max)
        {
            Max = max;
            Min = min;
        }
    }

    public struct Location
    {
        public readonly int X;
        public readonly int Y;

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
