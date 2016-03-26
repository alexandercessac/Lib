﻿using System;
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

        private ManualResetEvent FoodAvailable;

        FoodItem Dish;
        PhilosopherParty Party;

        public void ServeTable(FoodItem dish)
        {

            //Maybe?
            Dish = dish;
            Dish.CanGetFood = FoodAvailable;
            FoodAvailable.Set();

        }

        public void SetTable(int numEaters)
        {
            var numChopsticks = numEaters > 1 ? numEaters : 2;

            Chopsticks = new AutoResetEvent[numChopsticks];
            for (var i = 0; i < Chopsticks.Length - 1; i++)
                Chopsticks[i] = new AutoResetEvent(false);

            Party.Philosophers = new Philosopher[numEaters];

            for (var i = 0; i < Party.Length -1; i++)
            {
                Party.Philosophers[i].EatFromTable += EaterGetsFood;
                Party.Philosophers[i].Satisfaction = 0;
            }

            
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
                LeftChopstick = Chopsticks[GetLeftChopstick(id)],
                RightChopstick = Chopsticks[GetRightChopstick(id)]
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

        public void EaterGetsFood(Philosopher eater, TakeBitEventArgs e)
        {
            
            FoodAvailable.WaitOne();

            

            eater.HungerLevel --;
            Dish.Bites --;



        }


        
    }

    internal class PhilosopherParty
    {
        public int Length => Philosophers.Length;
        public Philosopher[] Philosophers { get; set; }
    }

    public class Philosopher
    {
        public bool Isfull { get; private set; }
        internal AutoResetEvent RightChopstick { get; set; }
        internal AutoResetEvent LeftChopstick { get; set; }
        public int Satisfaction { get; set; }
        public int HungerLevel { get; set; }

        public Philosopher(int id)
        {
            Id = id;
        }
        internal int Id { get; private set; }

        public void EatFoodUntillFull()
        {
            Eat(new TakeBitEventArgs {});
        }

        public delegate void EatHandler(Philosopher foodItem, TakeBitEventArgs e);

        public event EatHandler EatFromTable;

        public void Eat(TakeBitEventArgs e)
        {
            LeftChopstick.WaitOne();
            RightChopstick.WaitOne();
            EatFromTable?.Invoke(this, e);
            LeftChopstick.Set();
            RightChopstick.Set();
        }

        protected virtual void OnEatFood(Philosopher eater, TakeBitEventArgs e)
        {
            
        }
    }

    public class TakeBitEventArgs : EventArgs
    {
        public int Satisfaction { get; set; }
    }

    public class FoodItem
    {
        public int Bites { get; set; }
        public int SatisfactionAmount { get; private set; }
        public ManualResetEvent CanGetFood { get; set; }

        public delegate void TakeBiteHandler(Philosopher eater, TakeBitEventArgs e);

        public event TakeBiteHandler TakeBiteEvent;

        public void TakeBite()
        {
            Bites --;

            if (Bites == 0)
            {
                CanGetFood.Reset();
            }
        }

        protected virtual void OnTakeBiteEvent(Philosopher eater, TakeBitEventArgs e)
        {
            TakeBiteEvent?.Invoke(eater, e);
        }
    }
}
