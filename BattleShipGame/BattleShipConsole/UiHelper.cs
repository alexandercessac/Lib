using System;
using System.Text;
using BattleShipGame;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BattleShipConsole
{
    internal static class UiHelper
    {

        public static Queue<Action> EventQueue = new Queue<Action>();

        public static void DoEvents()
        {
            while (EventQueue.Count > 0)
                EventQueue.Dequeue().Invoke();
        }

        public static void Draw(this Map map) => map.Draw(false);

        public static void Draw(Map myMap, Map oppenentMap, bool showLegend = false)
        {
            Console.Clear();
            if (showLegend)
                DrawLegend();
            DrawHeader();
            
            //Write Names line
            Console.Write("     ");// Padding for Y index numbers & Leading space for formatting
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteName(myMap);
            Console.ResetColor();
            Console.Write(" ");//Trailing space for formatting

            Console.Write("     "); //Map Separator
            
            Console.Write("     ");// Padding for Y index numbers & Leading space for formatting
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteName(oppenentMap);
            Console.ResetColor();
            Console.Write(" ");//Trailing space for formatting

            //end Write player names
            Console.WriteLine();

            


            //X coord headers
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0] ");
            Console.ResetColor();
            for (var x = 0; x < myMap.BoardWidth; x++) Console.Write($" [{x}] ");

            Console.Write("     "); //Map Separator

            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write($"[0] ");
            Console.ResetColor();
            for (var x = 0; x < oppenentMap.BoardWidth; x++) Console.Write($" [{x}] ");

            Console.WriteLine();
            //end X coord headers


            //What if maps are different heights?
            //BUG: your map can only be as tall as my map
            for (var y = 0; y < myMap.BoardHeight; y++)
            {
                WriteFiller(myMap.BoardWidth);
                //Console.Write("  |  ");
                Console.Write("     "); //Map Separator
                WriteFiller(oppenentMap.BoardWidth);
                Console.WriteLine();

                //draw width of first map
                Console.Write($"[{y}] ");
                for (var x = 0; x < myMap.BoardWidth; x++)
                    DrawTile(myMap.TileDictionary[new Coordinate(x, y)].Status);

                Console.Write("     "); //Map Separator

                //draw width of second map
                Console.Write($"[{y}] ");
                for (var x = 0; x < oppenentMap.BoardWidth; x++)
                    //DrawOpponentTile(oppenentMap.TileDictionary[new Coordinate(x, y)].Status);
                    DrawTile(oppenentMap.TileDictionary[new Coordinate(x, y)].Status);//replace with above to hide opponents ships from each other

                Console.WriteLine();
            }
        }

        private static void WriteName(Map myMap)
        {
            var sb = new StringBuilder();

            var strActiveShips = $"Remaining Ships: {myMap.ActiveShips}";
            //Write player names (5 characters for each tile - 2 so it looks nice ☺)
            var totalLength = (myMap.BoardWidth * 5) - 2;
            for (var x = 0; x < totalLength; x++)
            {
                if (x < myMap.CaptainName.Length)
                    sb.Append(myMap.CaptainName[x]);
                else if (totalLength - x <= strActiveShips.Length)
                    sb.Append(strActiveShips[(int)(totalLength - x - strActiveShips.Length) * -1]);
                else
                    sb.Append(" ");
                
                
            }
            Console.Write(sb.ToString());
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
                    DrawTile(map.TileDictionary[new Coordinate(x, y)].Status);
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
                

            var line = sb.ToString();
            Console.Write(line.Substring(0, line.Length - 2));
            
            Console.Write(" ");
        }

        private static void DrawHeader()
        {
            Console.Write("     ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write("############################################### BATTLE SHIP ###############################################");
            Console.ResetColor();
            Console.WriteLine();
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
            Console.Write(" ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("X");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write(" ");
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
            Console.ForegroundColor = ConsoleColor.Red;
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
            {
                Console.Clear();
                Environment.Exit(0);
            }
                
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