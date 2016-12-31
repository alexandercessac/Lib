using System;
using System.Collections.Generic;
using BattleShipGame;
using System.Text.RegularExpressions;

namespace BattleShipConsole
{
    internal static class UiHelper
    {

        public static Queue<Action> EventQueue = new Queue<Action>();

        public static void Draw(this Map map) => map.Draw(false);

        public static void Draw(this Map map, bool showLegend)
        {
            if (showLegend)
                DrawLegend();
            drawHeader();

            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0] ");
            Console.ResetColor();

            for (var x = 0; x < map.BoardWidth; x++)
                Console.Write($" [{x}] ");

            Console.WriteLine();

            for (var y = 0; y < map.BoardHeight; y++)
            {
                WriteFillerLine(map.BoardWidth);

                Console.Write($"[{y}] ");
                for (var x = 0; x < map.BoardWidth; x++)
                    DrawTile(map.Tiles[new Coordinate(x, y)].Status);
                Console.WriteLine();
            }
        }

        private static void WriteFillerLine(uint length)
        {
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0]  ");
            Console.ResetColor();
            for (var x = 0; x < length; x++)
                Console.Write("-----");

            Console.CursorLeft--;
            Console.CursorLeft--;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write("  ");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void drawHeader()
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

        private static void DrawTile(TileStatus tileStatus)
        {
            Console.Write("|");
            switch (tileStatus)
            {
                case TileStatus.OpenOcean:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write("   ");
                    Console.ResetColor();
                    break;
                case TileStatus.Ship:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" █ ");
                    Console.ResetColor();
                    break;
                case TileStatus.Hit:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" X ");
                    Console.ResetColor();
                    break;
                case TileStatus.Miss:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" O ");
                    Console.ResetColor();
                    break;
                case TileStatus.Sunk:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" X ");
                    Console.ResetColor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tileStatus), tileStatus, null);
            }
            Console.Write("|");
        }

        public static string GetInput(string msg, string regex)
        {
            Match input;
            string rawInput;

            do
            {
                Msg(msg);

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
            Msg("Are you sure you want to quit? [y|n]");
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

        public static void Msg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{DateTime.Now}] ");
            Console.ResetColor();
            Console.Write($"{msg}: ");
        }
    }
}