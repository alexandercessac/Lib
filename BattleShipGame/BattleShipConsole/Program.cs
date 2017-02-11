using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using BattleShipGame;
using BattleShipGame.Identity;
using BattleShipGame.Ships;
using static BattleShipConsole.UiHelper;

namespace BattleShipConsole
{
    internal static class Program
    {
        public enum Direction { North, East, South, West }

        private static void Main(string[] args)
        {
            Console.Clear();
            Msg("Please select playmode");
            Msg("           ########################################");
            Msg("           #          [1] VS CPU                  #");
            Msg("           #          [2] HOST MULTIPLAYER        #");
            Msg("           #          [3] JOIN MULTIPLAYER        #");
            Msg("           ########################################");
            switch (GetInput<uint>("Select mode [1-3]", "[1-3]"))
            {
                case 1:
                    Console.Clear();
                    PlayerVsCpu();
                    break;
                case 2:
                    Console.Clear();
                    HostMultiplayer();
                    //TODO:
                    break;
                case 3:
                    Console.Clear();
                    JoinMultiplayer();
                    //TODO:
                    break;
                    
            }
            
        }

        private static void JoinMultiplayer()
        {
            Console.Clear();
            Console.Write("Enter Ip address of game to join: ");
            Console.ReadLine();


            throw new NotImplementedException();
        }

        private static void HostMultiplayer()
        {

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 1738);

            //TODO:
            //https://msdn.microsoft.com/en-us/library/system.net.httplistener(v=vs.110).aspx
            //tcp calls are firewalled :( try the above HttpListener example?
            var tmp = new HttpListener();
            tmp.Prefixes.Add("http://localhost/BattleShip");

            var server = new TcpListener(localEndPoint);

            try
            {
                server.Start();
                Info("Waiting for players...");

                var pendingClient = server.AcceptTcpClientAsync();

                var waitCount = 0;
                while (!pendingClient.IsCompleted)
                {
                    Console.Clear();
                    // doesnt work
                    Info($"Waiting for players{Enumerable.Repeat(".", waitCount)}");
                    waitCount++;

                    if (waitCount != 10) continue;

                    if (GetInput("No one is joining. Keep waiting?", "y|Y|n|N").ToUpper() == "Y")
                        return;
                    else
                        waitCount = 0;
                }

                Msg("Connected!");


                

                

            }
            catch (Exception e)
            {
                
            }
            finally { server.Stop();}
        }

        private static void PlayerVsCpu()
        {
            Console.Title = "BattleShip!";

            var player = new Player {Name = GetInput("Enter player name", ".+")};
            var cpu = new Player {Name = "CPU"};

            var options = new GameConfig
            {
                Players = new[]
                {
                    player,
                    cpu
                }
            }
                .WithMapHeight(10)
                .WithMapWidth(10);

            var gameState = new Game(options);

            //Init
            Draw(player.Map, cpu.Map, true);

            var shipCountPerPlayer = GetInput<uint>("Enter number of ships for this game", "10|[1-9]");

            for (var i = 0; i < shipCountPerPlayer; i++)
                SetPlayerAndCpuShip(player, cpu);

            while (player.Map.HasActiveShips || cpu.Map.HasActiveShips)
            {
                EventQueue.Enqueue(Console.Clear);
                EventQueue.Enqueue(() => Draw(player.Map, cpu.Map, true));

                if (!cpu.Map.Fire(player, GetCoordinate()))
                    EventQueue.Enqueue(() => Info($"{player.Map.Captain.Name} miss"));

                //Cpu fire
                var availableShots = player.Map.Tiles.Where(t => t.Value.Status == TileStatus.OpenOcean || t.Value.Status == TileStatus.Ship).ToArray();
                var coordinate = availableShots[new Random().Next(0, availableShots.Length - 1)].Key;

                if (!player.Map.Fire(cpu, coordinate))
                    EventQueue.Enqueue(() => Info($"{cpu.Map.Captain.Name} miss"));

                DoEvents();
            }
        }

        private static void SetPlayerAndCpuShip(Player player, Player cpu)
        {

            var playerShip = player.GetShip();

            //Ensure ship can be set at this location on the map
            while (!player.Map.SetShip(playerShip))
            {
                Error("Could not set ship at location");
                playerShip = player.GetShip();
            }

            //Set CPU ship with the same name and size
            var shipSet = cpu.Map.SetShip(cpu.GetCpuShip(playerShip.Name, playerShip.Size));
            //Ensure ship can be set at this location on the map
            while (!shipSet)
                shipSet = cpu.Map.SetShip(cpu.GetCpuShip(playerShip.Name, playerShip.Size));

            //draw updated map
            Console.Clear();
            Draw(player.Map, cpu.Map, true);
        }


        private static Ship GetCpuShip(this Player cpu, string name, uint length)
        {
            var ship = new Ship(GetCpuShipCoordinates(length, cpu.Map), name);

            SetEnemyShipEvents(cpu, ship);
            return ship;
        }

        private static Ship GetShip(this Player player)
        {
            var ship = new Ship(player.Map.GetUserShipCoordinates(), GetInput("Enter name of Ship", ".+"));

            SetShipEvents(player, ship);
            return ship;
        }

        private static void SetEnemyShipEvents(Player player, Ship ship)
        {
            //TODO: other events?
            ship.OnSinking += attacker =>
            {
                EventQueue.Enqueue(() => Msg($"{attacker.Name} sunk {player.Name}'s {ship.Name}!"));

                if (player.Map != null) player.Map.ActiveShips--;

                if (player.Map?.ActiveShips == 0)
                    EventQueue.Enqueue(Win);

            };
            ship.OnHit += (attacker, location) => EventQueue.Enqueue(() => Msg($"{attacker.Name} hit {player.Name}'s ship!"));
        }

        private static void SetShipEvents(Player player, Ship ship)
        {
            //TODO: other events?
            ship.OnSinking += attacker =>
            {
                EventQueue.Enqueue(() => Msg($"{attacker.Name} sunk {player.Name}'s {ship.Name}!"));

                if (player.Map != null) player.Map.ActiveShips--;

                if (player.Map?.ActiveShips == 0)
                    EventQueue.Enqueue(Lose);

            };
            ship.OnHit += (attacker, location) => EventQueue.Enqueue(() => Msg($"{attacker.Name} hit {player.Name}'s ship!"));
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
