using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShipGame;
using BattleShipGame.Ships;

namespace BattleShipConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var map = new Map();

            var shipSize = 2;

            var shipCoords = new Point[2]
            {
                new Point(0, 0),
                new Point(0, 1)
            };

            var ship = new Ship(shipCoords, (uint) shipSize);

            map.SetShip(ship);

            var result = map.Fire(new Point(0, 0)) ? "Hit" : "Miss";

            Console.WriteLine($"{result}!");

            Console.WriteLine("Press any key to fire again");
            Console.ReadKey();

            result = map.Fire(new Point(0, 1)) ? "Hit" : "Miss";

            Console.WriteLine($"{result}!");

            Console.WriteLine("Press any key to fire again");
            Console.ReadKey();

        }
    }
}
