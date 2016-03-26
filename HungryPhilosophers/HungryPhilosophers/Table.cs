using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HungryPhilosophers
{
    class Table
    {
        private AutoResetEvent[] Chopsticks;

        FoodItem Dish;
        PhilosopherParty Party;

        public void SetTable(int numEaters)
        {
            var numChopsticks = numEaters > 1 ? numEaters : 2;

            Chopsticks = new AutoResetEvent[numChopsticks];
            Party.Philosophers = new Philosopher[numEaters];

            EatFood += Feed;
        }

        public void SeatEaters()
        {
            for (var i = 0; i < Party.Length - 1; i++)
            {
                SeatEater(i);
            }
        }

        private void SeatEater(int id)
        {
            Party.Philosophers[id] = new Philosopher(id)
            {
                LeftChopstick = GetLeftChopstick(id),
                RightChopstick = GetRightChopstick(id)
            };
        }

        private int GetLeftChopstick(int eaterId)
        {
            return eaterId;
        }

        private int GetRightChopstick(int eaterId)
        {
            return eaterId == Party.Length - 1 ? 0 : eaterId + 1;
        }

        void Feed(Philosopher eater)
        {
            
        }
        
        public delegate void TakeBite(Philosopher eater);

        public event TakeBite EatFood;
    }

    internal class PhilosopherParty
    {
        public int Length => Philosophers.Length;
        public Philosopher[] Philosophers { get; set; }
    }

    public class Philosopher
    {
        internal int RightChopstick { get; set; }
        internal int LeftChopstick { get; set; }
        public int HungerLevel { get; private set; }

        public Philosopher(int id)
        {
            Id = id;
        }
        internal int Id { get; private set; }
        
    }

    public class TakeBitEventArgs : EventArgs
    {
        public int Satisfaction { get; set; }
    }

    public class FoodItem
    {
        private int Bites { get; set; }
        private int SatisfactionAmount { get; set; }
    }
}
