using System;
using System.Collections.Generic;
using System.Text;
using BattleShipGame;
using System.Text.RegularExpressions;

namespace BattleShipConsole
{
    internal static class UiHelper
    {

        public static Queue<Action> EventQueue = new Queue<Action>();

        public static void Draw(this Map map) => map.Draw(false);

        public static void Draw(Map myMap, Map oppenentMap, bool showLegend = false)
        {
            if (showLegend)
                DrawLegend();
            DrawHeader();

            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0] ");
            Console.ResetColor();

            //X coord headers
            for (var x = 0; x < myMap.BoardWidth; x++)
                Console.Write($" [{x}] ");

            Console.Write("  |  ");

            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0] ");
            Console.ResetColor();
            for (var x = 0; x < oppenentMap.BoardWidth; x++)
                Console.Write($" [{x}] ");

            Console.WriteLine();
            //end X coord headers


            //What if maps are different heights?
            for (var y = 0; y < myMap.BoardHeight; y++)
            {
                WriteFiller(myMap.BoardWidth);
                Console.Write("  |  ");
                WriteFiller(oppenentMap.BoardWidth);
                Console.WriteLine();

                //draw width of first map
                Console.Write($"[{y}] ");
                for (var x = 0; x < myMap.BoardWidth; x++)
                    DrawTile(myMap.Tiles[new Coordinate(x, y)].Status);

                //separator
                Console.Write("  |  ");

                //draw width of second map
                Console.Write($"[{y}] ");
                for (var x = 0; x < oppenentMap.BoardWidth; x++)
                    DrawOpponentTile(oppenentMap.Tiles[new Coordinate(x, y)].Status);

                Console.WriteLine();
            }
        }

        public static void Draw(this Map map, bool showLegend)
        {
            if (showLegend)
                DrawLegend();
            DrawHeader();

            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0] ");
            Console.ResetColor();

            for (var x = 0; x < map.BoardWidth; x++)
                Console.Write($" [{x}] ");

            Console.WriteLine();

            for (var y = 0; y < map.BoardHeight; y++)
            {
                WriteFiller(map.BoardWidth);

                Console.WriteLine();

                Console.Write($"[{y}] ");
                for (var x = 0; x < map.BoardWidth; x++)
                    DrawTile(map.Tiles[new Coordinate(x, y)].Status);
                Console.WriteLine();
            }
        }

        private static void WriteFiller(uint length)
        {
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0]  ");
            Console.ResetColor();
            var sb = new StringBuilder();

            for (var x = 0; x < length; x++)
                sb.Append("-----");
                //Console.Write("-----");

            var line = sb.ToString();
            Console.Write(line.Substring(0, line.Length - 2));

            //Console.CursorLeft--;
            //Console.CursorLeft--;
            //Console.ForegroundColor = Console.BackgroundColor;
            Console.Write(" ");
            //Console.ResetColor();


        }

        private static void DrawHeader()
        {
            Console.Write("     ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine("############### BATTLE SHIP ###############");
            Console.ResetColor();
        }

        public static void DrawLegend()
        {
            Console.WriteLine("Legend");
            DrawTile(TileStatus.OpenOcean);
            Console.WriteLine($"    {nameof(TileStatus.OpenOcean)}");
            DrawTile(TileStatus.Ship);
            Console.WriteLine($"    {nameof(TileStatus.Ship)}");
            DrawTile(TileStatus.Hit);
            Console.WriteLine($"    {nameof(TileStatus.Hit)}");
            DrawTile(TileStatus.Miss);
            Console.WriteLine($"    {nameof(TileStatus.Miss)}");
            DrawTile(TileStatus.Sunk);
            Console.WriteLine($"    {nameof(TileStatus.Sunk)}");
        }

        private static void DrawOpponentTile(TileStatus tileStatus)
        {
            Console.Write("|");
            switch (tileStatus)
            {
                case TileStatus.OpenOcean:
                case TileStatus.Ship://Conceal opponent's un hit ship as opean ocean
                    DrawOpenOceanTile();
                    break;
                case TileStatus.Hit:
                    DrawHitTile();
                    break;
                case TileStatus.Miss:
                    DrawMissTile();
                    break;
                case TileStatus.Sunk:
                    DrawSunkTile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tileStatus), tileStatus, null);
            }
            Console.Write("|");
        }

        private static void DrawTile(TileStatus tileStatus)
        {
            Console.Write("|");
            switch (tileStatus)
            {
                case TileStatus.OpenOcean:
                    DrawOpenOceanTile();
                    break;
                case TileStatus.Ship:
                    DrawShipTile();
                    break;
                case TileStatus.Hit:
                    DrawHitTile();
                    break;
                case TileStatus.Miss:
                    DrawMissTile();
                    break;
                case TileStatus.Sunk:
                    DrawSunkTile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tileStatus), tileStatus, null);
            }
            Console.Write("|");
        }

        private static void DrawSunkTile()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" X ");
            Console.ResetColor();
        }

        private static void DrawMissTile()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" O ");
            Console.ResetColor();
        }

        private static void DrawHitTile()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" X ");
            Console.ResetColor();
        }

        private static void DrawOpenOceanTile()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("   ");
            Console.ResetColor();
        }

        private static void DrawShipTile()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" █ ");
            Console.ResetColor();
        }

        public static string GetInput(string msg, string regex)
        {
            Match input;
            string rawInput;

            do
            {
                Ask(msg);

                rawInput = Console.ReadLine()?.Replace(Environment.NewLine, "").Trim() ?? string.Empty;

                rawInput.CheckSysComands();

                input = Regex.Match(rawInput, regex);
            } while (!input.Success && rawInput != "q");

            return input.Value;
        }

        public static T GetInput<T>(string msg, string regex) => (T)Convert.ChangeType(GetInput(msg, regex), typeof(T));

        private static void CheckSysComands(this string rawInput)
        {
            if (rawInput == "q" || rawInput == "Q")
                ConfirmQuit();
        }

        private static void ConfirmQuit()
        {
            Ask("Are you sure you want to quit? [y|n]");
            var rawInput = Console.ReadLine()?.Replace(Environment.NewLine, "").Trim() ?? string.Empty;
            if (rawInput == "y" || rawInput == "Y")
                Environment.Exit(0);
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[{DateTime.Now}] ");
            Console.ResetColor();
            Console.WriteLine($"{msg}");
        }

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{DateTime.Now}] ");
            Console.ResetColor();
            Console.WriteLine($"{msg}");
        }

        public static void Ask(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{DateTime.Now}] ");
            Console.ResetColor();
            Console.Write($"{msg}: ");
        }

        public static void Msg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{DateTime.Now}] ");
            Console.ResetColor();
            Console.WriteLine($"{msg}");
        }
    }
}