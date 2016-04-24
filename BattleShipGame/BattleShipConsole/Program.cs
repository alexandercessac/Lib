using System;
using System.Data.SqlTypes;
using System.Drawing;
using System.Text.RegularExpressions;
using BattleShipGame;
using BattleShipGame.Ships;

namespace BattleShipConsole
{
    internal class Program
    {
        public static Map MyMap { get; private set; }

        enum Direction { North, East, South, West }

        private static void Main(string[] args)
        {
            //Init map
            MyMap = new Map(10, 10);




            var ship = new Ship(GetShipCoordinates(), GetInput("Enter name of first Ship", ".+"));

            ship.OnSinking += () => Console.WriteLine($"{ship.Name} has been sunk!");

            MyMap.SetShip(ship);

            var result = MyMap.Fire(new Coordinate(0, 0)) ? "Hit" : "Miss";

            Console.WriteLine($"{result}!");

            Console.WriteLine("Press any key to fire again");
            Console.ReadKey();

            result = MyMap.Fire(new Coordinate(0, 1)) ? "Hit" : "Miss";

            Console.WriteLine($"{result}!");

            Console.WriteLine("Press any key to end");
            Console.ReadKey();
        }

        private static Coordinate[] GetShipCoordinates()
        {
            do
            {

                var shipSize = uint.Parse(GetInput("Enter length of ship [1 - 10]", "10|[1-9]"));
                
                var tmp = GetCoordinate();

                if (tmp == null)
                {
                    //handle user quit    
                    throw new Exception();
                }

                var bow = tmp.Value;

                var direction = (Direction)Enum.Parse(typeof(Direction),
                    GetInput("Enter direction of ship (North, South, East, or West)",
                        "^(n(orth)?|e(ast)?|s(outh)?|w(est)?)$"));


                Coordinate[] shipCoords;
                if (TryPopulateCoordinates(direction, bow, shipSize,out shipCoords)) return shipCoords;
                Msg($"Ship of size {shipSize} will not fit on the map at location {bow.X}, {bow.Y} facing {direction}");
            } while (true);

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
                    else
                    {
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
                    }
                case Direction.East:
                    if (bow.X + shipSize > MyMap.BoardWidth)
                    {
                        break;
                    }
                    else
                    {
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
                    }
                case Direction.South:
                    if (bow.Y + shipSize > MyMap.BoardHeight)
                    {
                        break;
                    }
                    else
                    {
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
                    }
                    ;
                case Direction.West:
                    if (bow.X - shipSize < 0)
                    {
                        break;
                    }
                    else
                    {
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
                    }
                default:
                {
                    shipCoords = null;
                    return true;
                }
            }
            return false;
        }

        private static Coordinate? GetCoordinate()
        {
            Coordinate bow;
            do
            {
                bow = new Coordinate
                {
                    X = int.Parse(GetInput("Enter X coordinate of ship", "10|[1-9]")),
                    Y = int.Parse(GetInput("Enter Y coordinate of ship", "10|[1-9]"))
                };
                if (MyMap.IsOpenOcean(bow))
                {
                    Msg("Tile unavilable!");
                    Msg(Environment.NewLine);
                }
                else
                {
                    break;
                }
            } while (true);
            return bow;
        }


        private static string GetInput(string msg, string regex)
        {
            Match input;
            string rawInput;

            do
            {
                Msg(msg);

                rawInput = Console.ReadLine()?.Replace(Environment.NewLine, "");

                input = Regex.Match(rawInput, regex);
            } while (!input.Success && rawInput != "q");
            return input.Value;
        }

        private static void Msg(string msg)
        {
            Console.Write($"[{DateTime.Now}] {msg}: ");
        }
    }
}
