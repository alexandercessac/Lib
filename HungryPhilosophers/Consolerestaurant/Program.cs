using System;
using System.Threading.Tasks;
using HungryPhilosophers;

namespace Consolerestaurant
{
    class Program
    {
        static void Main(string[] args)
        {
            var tableOne = new Table();

            Console.WriteLine("Setting table..");
            tableOne.SetTable(3);
            Console.WriteLine("Seating diners table..");
            tableOne.SeatEaters();
            Console.WriteLine("Serving 45 bite food item..");
            tableOne.ServeTable(new FoodItem { Bites = 45, NoFoodLeft = null });

            var complete = ReadKeys(tableOne);

            complete.Wait();

        }

        private static Task ReadKeys(Table theTable)
        {
            return Task.Run(() =>
            {
                var key = Console.ReadLine();
                while (key.Trim().ToLower() != "q")
                {
                    var dishBites = 0;
                    if (int.TryParse(key.Trim(), out dishBites) && dishBites > -1)
                    {
                        theTable.ServeTable(new FoodItem { Bites = dishBites, NoFoodLeft = null });
                    }
                    else
                    {
                        Console.WriteLine("invalid number. Please enter a number greater than 0 or 'Q' to quit");
                    }
                    key = Console.ReadLine();
                }
            });
        }
    }
}

