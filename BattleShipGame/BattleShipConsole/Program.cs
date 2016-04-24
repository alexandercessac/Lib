using System;
using System.Drawing;
using BattleShipGame;
using BattleShipGame.Ships;

namespace BattleShipConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var map = new Map();

            var shipSize = 2;

            var shipCoords = new[]
            {
                new Coordinate(0, 0),
                new Coordinate(0, 1)
            };

            Console.WriteLine("Enter name of first Ship");
            var shipName = Console.ReadLine();

            var ship = new Ship(shipCoords, (uint) shipSize, shipName);
            
            ship.OnSinking += () => Console.WriteLine($"{ship.Name} has been sunk!");

            map.SetShip(ship);

            var result = map.Fire(new Coordinate(0, 0)) ? "Hit" : "Miss";

            Console.WriteLine($"{result}!");

            Console.WriteLine("Press any key to fire again");
            Console.ReadKey();

            result = map.Fire(new Coordinate(0, 1)) ? "Hit" : "Miss";

            Console.WriteLine($"{result}!");

            Console.WriteLine("Press any key to end");
            Console.ReadKey();

        }
    }
}
