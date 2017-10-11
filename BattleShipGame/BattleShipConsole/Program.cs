using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BattleShipGame;
using BattleShipGame.Identity;
using BattleShipGame.Network;
using BattleShipGame.Ships;
using Newtonsoft.Json;
using static BattleShipConsole.UiHelper;

namespace BattleShipConsole
{
    internal static class Program
    {
        public enum Direction { North, East, South, West }

        private static void Main(string[] args)
        {

            ////It works
            //var test = new Map
            //{
            //    TileDictionary = new TileDictionary
            //{
            //    { new Coordinate(0, 0), new Tile { Status = TileStatus.Sunk } },
            //    { new Coordinate(0, 1), new Tile { Status = TileStatus.Sunk } }
            //}
            //};
            //var teststring = JsonConvert.SerializeObject(test);

            //var testdict = JsonConvert.DeserializeObject<Map>(teststring);

            //var tmp = JsonConvert.SerializeObject(new Coordinate(20, 20));
            //var testcoord = JsonConvert.DeserializeObject<Coordinate>(tmp);

            //var tmp2 = JsonConvert.SerializeObject(new Ship(new[] {new Coordinate(0,1), new Coordinate(0, 0) }, "name"));
            //var testship = JsonConvert.DeserializeObject<Ship>(tmp2);


            Console.Title = "BattleShip!";

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
                    JoinMultiplayer().Wait();
                    //TODO:
                    break;

            }

        }

        private static async Task JoinMultiplayer()
        {
            Console.Clear();

            var player = new Player { Name = GetInput("Enter player name", ".+") };

            player.Map = new Map(player, 10, 10);

            //create map
            //var opponent = new Player();
            //var options = new GameConfig { Players = new[] { player, opponent } }.WithMapHeight(10).WithMapWidth(10);
            //new Game(options);

            for (var i = 0; i < 3; i++)
            {
                SetPlayerShip(player);
                Console.Clear();
                player.Map.Draw(true);
            }

            Console.Write("Enter Ip address of game to join: ");
            var ip = Console.ReadLine();

            Client.Msg += Info;

            Game game = null;

            while (game == null)
                game = await player.JoinGame(ip);

            var oppenentMap = game.Players[0].Map;


            foreach (var ship in oppenentMap.Fleet)
                SetEnemyShipEvents(game.Players[0], ship);

            Draw(player.Map, oppenentMap, true);

            while (player.Map.HasActiveShips || oppenentMap.HasActiveShips)
            {
                EventQueue.Enqueue(Console.Clear);
                EventQueue.Enqueue(() => Draw(player.Map, oppenentMap, true));

                var coordinate = GetCoordinate();

                var shot = new Shot { Coordinate = coordinate, PlayerName = player.Name };
                var fireingShot = shot.Fire();

                if (!oppenentMap.Fire(shot))
                    EventQueue.Enqueue(() => Info($"{player.Name} miss"));

                var fireResult = await fireingShot;

                if (fireResult != null)
                {
                    oppenentMap.ActiveShips = fireResult.Players[0].Map.ActiveShips;

                    foreach (var result in oppenentMap.TileDictionary)
                        result.Value.Status = fireResult.Players[0].Map.TileDictionary[result.Key].Status;

                    player.Map.TileDictionary = fireResult.Players[1].Map.TileDictionary;
                }


                DoEvents();
            }

            throw new NotImplementedException();
        }

        private static void HostMultiplayer()
        {

            var cts = new CancellationTokenSource();

            var player = new Player { Name = GetInput("Enter player name", ".+") };
            var opponent = new Player();

            var options = new GameConfig
            {
                Players = new[]
                {
                    player,
                    opponent
                }
            }
                .WithMapHeight(10)
                .WithMapWidth(10);

            var gameState = new Game(options);

            Info("Waiting for players...");

            var CONNECTING = new object();

            Listener.PlayerConnected += p =>
            {
                lock (CONNECTING)
                {
                    if (!Listener.WaitForPlayers) return;
                    //Todo: handle players joining in a better way
                    Listener.WaitForPlayers = false;
                    gameState.Players[1] = opponent = p;

                    foreach (var ship in opponent.Map.Fleet)
                        SetEnemyShipEvents(opponent, ship);

                    Msg($"{opponent.Name} Connected!");
                }
            };

            var hostTask = Listener.HostGame(gameState, cts.Token);



            for (var i = 0; i < gameState.NumberOfShips; i++)
            {
                SetPlayerShip(player);
                Console.Clear();
                player.Map.Draw(true);
            }





            var waitCount = 0;
            while (Listener.WaitForPlayers)
            {
                Console.Clear();
                // doesnt work
                Info($"Waiting for players{string.Join("", Enumerable.Repeat(".", waitCount).ToArray())}");
                waitCount++;
                Thread.Sleep(2000);

                if (waitCount != 10) continue;

                if (GetInput("No one is joining. Keep waiting?", "y|Y|n|N").ToUpper() == "N")
                    return;
                else
                    waitCount = 0;
            }


            Listener.PlayerShot += s =>
            {
                if (!player.Map.Fire(s))
                    EventQueue.Enqueue(() => Info($"{opponent.Name} miss"));
            };

            //Init
            Draw(player.Map, opponent.Map, true);

            while (player.Map.HasActiveShips || opponent.Map.HasActiveShips)
            {
                EventQueue.Enqueue(Console.Clear);
                EventQueue.Enqueue(() => Draw(player.Map, opponent.Map, true));

                if (!opponent.Map.Fire(player, GetCoordinate()))
                    EventQueue.Enqueue(() => Info($"{player.Name} miss"));

                DoEvents();
            }

            cts.Cancel();
            hostTask.Wait();
        }



        private static void PlayerVsCpu()
        {


            var player = new Player { Name = GetInput("Enter player name", ".+") };
            var cpu = new Player { Name = "CPU" };

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
                    EventQueue.Enqueue(() => Info($"{player.Name} miss"));

                //Cpu fire
                var availableShots = player.Map.TileDictionary.Where(t => t.Value.Status == TileStatus.OpenOcean || t.Value.Status == TileStatus.Ship).ToArray();
                var coordinate = availableShots[new Random().Next(0, availableShots.Length - 1)].Key;

                if (!player.Map.Fire(cpu, coordinate))
                    EventQueue.Enqueue(() => Info($"{cpu.Name} miss"));

                DoEvents();
            }
        }

        private static void SetPlayerShip(Player player)
        {
            //Ensure ship can be set at this location on the map
            while (!player.Map.SetShip(player.GetShip()))
                Error("Could not set ship at location");
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
            ship.OnSinking += attackerName =>
            {
                EventQueue.Enqueue(() => Msg($"{attackerName} sunk {player.Map.CaptainName}'s {ship.Name}!"));

                if (player.Map != null) player.Map.ActiveShips--;

                if (player.Map?.ActiveShips == 0)
                    EventQueue.Enqueue(Win);

            };
            ship.OnHit += (attackerName, location) => EventQueue.Enqueue(() => Msg($"{attackerName} hit {player.Name}'s ship!"));
        }

        private static void SetShipEvents(Player player, Ship ship)
        {
            //TODO: other events?
            ship.OnSinking += attackerName =>
            {
                EventQueue.Enqueue(() => Msg($"{attackerName} sunk {player.Name}'s {ship.Name}!"));

                if (player.Map != null) player.Map.ActiveShips--;

                if (player.Map?.ActiveShips == 0)
                    EventQueue.Enqueue(Lose);

            };
            ship.OnHit += (attackerName, location) => EventQueue.Enqueue(() => Msg($"{attackerName} hit {player.Name}'s ship!"));
        }

        private static void Win()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(" Congratulations you have sunk all the battleships and have been promoted to Lord of the Kings Navy! ");
            Console.ResetColor();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void Lose()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(" The last resistence against the rebels has failed. All youre base are belong to us... ");
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
