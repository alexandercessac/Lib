using System;
using System.CodeDom;
using System.Collections.Generic;
using BattleShipGame;
using BattleShipGame.Ships;
using static BattleShipConsole.UiHelper;

namespace BattleShipConsole
{
    internal class Program
    {
        private static readonly string RN = Environment.NewLine;
        public static Map MyMap { get; private set; }
        public enum Direction { North, East, South, West }

        public static int RemainingShips = 0;

        private static void Main(string[] args)
        {
            Console.Title = "BattleShip!";

            //Init map
            MyMap = new Map(10, 10);
            MyMap.Draw(showLegend: true);

            MyMap.SetShip(GetShip());
            RemainingShips++;

            MyMap.SetShip(GetShip());
            RemainingShips++;

            Console.Clear();
            MyMap.Draw();

            while (RemainingShips != 0)
                Fire(GetCoordinate());
        }

        private static void Fire(Coordinate coord)
        {
            EventQueue.Enqueue(Console.Clear);
            EventQueue.Enqueue(MyMap.Draw);

            if (!MyMap.Fire(coord))
                EventQueue.Enqueue(() => Info("Miss"));

            while (EventQueue.Count > 0)
                EventQueue.Dequeue().Invoke();

        }

        private static Ship GetShip()
        {
            var ship = new Ship(GetShipCoordinates(), GetInput("Enter name of first Ship", ".+"));

            //TODO: other events?
            ship.OnSinking += () =>
            {
                EventQueue.Enqueue(() => Info($"{ship.Name} has been sunk!"));
                RemainingShips--;
                if (RemainingShips == 0)
                {
                    EventQueue.Enqueue(Win);
                }
            };
            ship.OnHit += location => EventQueue.Enqueue(() =>  Info("Hit!"));
            return ship;
        }

        private static void Win()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Congradulations you have sunk all the battleships and have been promoted to Lord of the Kings Navy!");
            Console.ResetColor();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static Coordinate[] GetShipCoordinates()
        {
            do
            {
                Info("Setting coordinates of new ship");
                var bow = GetShipCoordinate();
                var shipSize = GetInput<uint>("Enter length of ship [1 - 10]", "10|[1-9]");
                var direction = GetDirectin();

                Coordinate[] shipCoords;
                if (TryPopulateCoordinates(direction, bow, shipSize, out shipCoords))
                    return shipCoords;

                Error($"Ship of size {shipSize} will not fit on the map at location {bow.X}, {bow.Y} facing {direction}");

            } while (true);

        }

        private static Direction GetDirectin()
        {
            var input = GetInput("Enter direction of ship (North, South, East, or West)",
                "^(n(orth)?|e(ast)?|s(outh)?|w(est)?)$").ToUpper();

            switch (input)
            {
                case "N":
                case "NORTH":
                    return Direction.North;
                case "S":
                case "SOUTH":
                    return Direction.South;
                case "E":
                case "EAST":
                    return Direction.East;
                case "W":
                case "WEST":
                    return Direction.West;
                default:
                    throw new Exception($"{input} is not a valid {nameof(Direction)}");
            }


        }

        private static bool TryPopulateCoordinates(Direction direction, Coordinate bow, uint shipSize, out Coordinate[] shipCoords)
        {
            shipCoords = new Coordinate[shipSize];
            switch (direction)
            {
                case Direction.North:
                    if (bow.Y - shipSize < 0)
                    {
                        break;
                    }
                    shipCoords[0] = bow;
                    for (var i = 1; i < shipCoords.Length; i++)
                    {
                        shipCoords[i] = new Coordinate
                        {
                            X = bow.X,
                            Y = bow.Y - i
                        };
                    }
                    {
                        return true;
                    }
                case Direction.East:
                    if (bow.X + shipSize > MyMap.BoardWidth)
                    {
                        break;
                    }
                    shipCoords[0] = bow;
                    for (var i = 1; i < shipCoords.Length; i++)
                    {
                        shipCoords[i] = new Coordinate
                        {
                            X = bow.X + i,
                            Y = bow.Y
                        };
                    }
                    {
                        return true;
                    }
                case Direction.South:
                    if (bow.Y + shipSize > MyMap.BoardHeight)
                    {
                        break;
                    }
                    shipCoords[0] = bow;
                    for (var i = 1; i < shipCoords.Length; i++)
                    {
                        shipCoords[i] = new Coordinate
                        {
                            X = bow.X,
                            Y = bow.Y + i
                        };
                    }
                    {
                        return true;
                    }
                    ;
                case Direction.West:
                    if (bow.X - shipSize < 0)
                    {
                        break;
                    }
                    shipCoords[0] = bow;
                    for (var i = 1; i < shipCoords.Length; i++)
                    {
                        shipCoords[i] = new Coordinate
                        {
                            X = bow.X - i,
                            Y = bow.Y
                        };
                    }
                    {
                        return true;
                    }
                default:
                    {
                        shipCoords = null;
                        return true;
                    }
            }
            return false;
        }

        private static Coordinate GetShipCoordinate()
        {
            Coordinate bow;
            do
            {
                bow = GetCoordinate();

                if (!MyMap.OpenOcean(bow))
                    Error("Tile unavailable!");
                else
                    break;

            } while (true);
            return bow;
        }

        private static Coordinate GetCoordinate() => new Coordinate
        {
            X = int.Parse(GetInput("Enter X coordinate", "10|[0-9]")),
            Y = int.Parse(GetInput("Enter Y coordinate", "10|[0-9]"))
        };
    }
}
