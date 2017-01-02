using System;
using System.Linq;
using BattleShipGame;
using BattleShipGame.Ships;
using static BattleShipConsole.UiHelper;

namespace BattleShipConsole
{
    internal static class Program
    {
        public static Map MyMap { get; private set; }
        public enum Direction { North, East, South, West }

        public static int RemainingShips = 0;

        private static void RunOnePlayerGame()
        {
            //Init map
            MyMap = new Map(10, 10);
            MyMap.Draw(showLegend: true);

            MyMap.SetShip(GetShip(MyMap));
            RemainingShips++;

            MyMap.SetShip(GetShip(MyMap));
            RemainingShips++;

            Console.Clear();
            MyMap.Draw();

            while (RemainingShips != 0)
                MyMap.Fire(GetCoordinate());
        }

        private static void Main(string[] args)
        {
            Console.Title = "BattleShip!";

            var options = new GameConfig
            {
                MapWidth = 10,
                MapHeight = 10,
                Players = 2 //User & CPU
            };

            var gameState = new Game(options);

            var myMap = gameState.Maps[0];
            var oppenentMap = gameState.Maps[1];

            //Init
            Draw(myMap, oppenentMap, true);


            var newShip1 = GetShip(myMap);
            myMap.SetShip(newShip1);
            oppenentMap.SetShip(GetCpuShip(newShip1.Name, newShip1.Size, oppenentMap));

            Console.Clear();
            Draw(myMap, oppenentMap, true);

            var newShip2 = GetShip(myMap);
            myMap.SetShip(newShip2);
            oppenentMap.SetShip(GetCpuShip(newShip2.Name, newShip2.Size, oppenentMap));

            Console.Clear();
            Draw(myMap, oppenentMap, true);

            while (myMap.ActiveShips != 0 || oppenentMap.ActiveShips != 0)
            {

                EventQueue.Enqueue(Console.Clear);
                EventQueue.Enqueue(() => Draw(myMap, oppenentMap, true));


                if (!oppenentMap.Fire(GetCoordinate()))
                    EventQueue.Enqueue(() => Info("You miss"));

                //Cpu fire
                var availableShots = myMap.Tiles.Where(t => t.Value.Status == TileStatus.OpenOcean).ToArray();
                var coordinate = availableShots[new Random().Next(0, availableShots.Length - 1)].Key;

                if (!myMap.Fire(coordinate))
                    EventQueue.Enqueue(() => Info("Opponent miss"));


                while (EventQueue.Count > 0)
                    EventQueue.Dequeue().Invoke();
            }
        }



        private static void Fire(this Map map, Coordinate coord)
        {


            if (!map.Fire(coord))
                EventQueue.Enqueue(() => Info("Miss"));


        }

        private static Ship GetCpuShip(string name, uint length, Map map)
        {
            var ship = new Ship(GetCpuShipCoordinates(length, map), name);

            SetCpuShipEvents(map, ship);
            return ship;
        }

        private static Ship GetShip(Map map = null)
        {
            var ship = new Ship(map.GetUserShipCoordinates(), GetInput("Enter name of first Ship", ".+"));

            SetShipEvents(map, ship);
            return ship;
        }

        private static void SetCpuShipEvents(Map map, Ship ship)
        {
            //TODO: other events?
            ship.OnSinking += () =>
            {
                EventQueue.Enqueue(() => Msg($"Opponent's {ship.Name} has been sunk!"));

                if (map != null) map.ActiveShips--;

                if (map?.ActiveShips == 0)
                    EventQueue.Enqueue(Win);

            };
            ship.OnHit += location => EventQueue.Enqueue(() => Msg("You hit an opponent's ship!"));
        }

        private static void SetShipEvents(Map map, Ship ship)
        {
            //TODO: other events?
            ship.OnSinking += () =>
            {
                EventQueue.Enqueue(() => Error($"Your {ship.Name} has been sunk!"));

                if (map != null) map.ActiveShips--;

                if (map?.ActiveShips == 0)
                    EventQueue.Enqueue(Lose);

            };
            ship.OnHit += location => EventQueue.Enqueue(() => Error("Opponent hit a ship!"));
        }

        private static void Win()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Congratulations you have sunk all the battleships and have been promoted to Lord of the Kings Navy!");
            Console.ResetColor();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void Lose()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("The last resistence against the rebels has failed. All youre base are belong to us...");
            Console.ResetColor();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static Coordinate[] GetCpuShipCoordinates(uint length, Map myMap)
        {
            do
            {
                //Info("Setting coordinates of new ship");

                var bow = new Coordinate(new Random().Next(0, 10), new Random().Next(0, 10));

                var direction = (Direction)new Random().Next(0, 4);

                Coordinate[] shipCoords;
                if (TryPopulateCoordinates(direction, bow, length, out shipCoords, myMap))
                    return shipCoords;

                //Error($"Ship of size {shipSize} will not fit on the map at location {bow.X}, {bow.Y} facing {direction}");

            } while (true);

        }

        private static Coordinate[] GetUserShipCoordinates(this Map map)
        {
            do
            {
                Info("Setting coordinates of new ship");
                var bow = GetShipCoordinate(map);
                var shipSize = GetInput<uint>("Enter length of ship [1 - 10]", "10|[1-9]");
                var direction = GetDirectin();

                Coordinate[] shipCoords;
                if (TryPopulateCoordinates(direction, bow, shipSize, out shipCoords, map))
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

        private static bool TryPopulateCoordinates(Direction direction, Coordinate bow, uint shipSize, out Coordinate[] shipCoords, Map map)
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
                    if (bow.X + shipSize > map.BoardWidth)
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
                    if (bow.Y + shipSize > map.BoardHeight)
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

        private static Coordinate GetShipCoordinate(Map map)
        {
            Coordinate bow;
            do
            {
                bow = GetCoordinate();

                if (!map.OpenOcean(bow))
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
