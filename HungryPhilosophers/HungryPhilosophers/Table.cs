using System;
using System.Threading;
using System.Threading.Tasks;

namespace HungryPhilosophers
{
    public class Table
    {
        private AutoResetEvent[] Chopsticks;

        public bool FoodAvailable;

        FoodItem Dish;
        PhilosopherParty Party = new PhilosopherParty();

        public object Locker = new object();

        public delegate Task FoodArives();

        public FoodArives OnFoodArives;

        public void ServeTable(FoodItem dish)
        {

            //Maybe?
            Dish = dish;
            Dish.NoFoodLeft += FoodEaten;
            FoodAvailable = true;
            OnFoodArives?.Invoke();

        }

        public void SetTable(int numEaters)
        {
            var numChopsticks = numEaters > 1 ? numEaters : 2;

            Chopsticks = new AutoResetEvent[numChopsticks];
            for (var i = 0; i < Chopsticks.Length; i++)
                Chopsticks[i] = new AutoResetEvent(true);

            Party = new PhilosopherParty { Philosophers = new Philosopher[numEaters] };

        }

        public void SeatEaters()
        {
            var tmp = new Random();


            for (var i = 0; i < Party.Length; i++)
            {
                var tmpBit = (tmp.Next(0, (Party.Length * 2) - 1) & 1);
                var whatever = tmpBit == 0;
                SeatEater(i, whatever);

            }
        }

        private void SeatEater(int id, bool whatever)
        {
            Party.Philosophers[id] = new Philosopher(id)
            {
                LeftChopstick = Chopsticks[GetLeftChopstick(id)],
                RightChopstick = Chopsticks[GetRightChopstick(id)],
                Satisfaction = 0,
                HungerLevel = 25
            };

            if (whatever)
            {
                Party.Philosophers[id].EatFromTable += RightHandedEaterGetsFood;
            }
            else
            {
                Party.Philosophers[id].EatFromTable += LeftHandedEaterGetsFood;
            }
            OnFoodArives += Party.Philosophers[id].EatFoodUntillFull;
            Party.Philosophers[id].myTable = this;

        }

        private int GetLeftChopstick(int eaterId)
        {
            return eaterId;
        }

        private int GetRightChopstick(int eaterId)
        {
            return eaterId == Party.Length - 1 ? 0 : eaterId + 1;
        }

        public void RightHandedEaterGetsFood(Philosopher eater, TakeBitEventArgs e)
        {

            eater.RightChopstick.WaitOne();
            eater.LeftChopstick.WaitOne();

            Eat(eater);
            eater.RightChopstick.Set();
            eater.LeftChopstick.Set();



        }

        private void Eat(Philosopher eater)
        {
            lock (Locker)
            {
                //What if no food is delivered?
                if (Dish.TakeBite())
                {
                    
                    eater.HungerLevel--;

                    Console.WriteLine($"Eater # {eater.Id} takes a bite. Hunger level: {eater.HungerLevel} --- Remainging food: {Dish.Bites}");
                    eater.IsEating = false;
                }
            }
            Monitor.Pulse(Locker);

        }

        public void LeftHandedEaterGetsFood(Philosopher eater, TakeBitEventArgs e)
        {
            eater.LeftChopstick.WaitOne();
            eater.RightChopstick.WaitOne();

            Eat(eater);

            eater.LeftChopstick.Set();
            eater.RightChopstick.Set();

        }


        public void FoodEaten()
        {
            FoodAvailable = false;
            //Todo: alert the waiter?
            Console.WriteLine("table is out of food.");

        }


    }

    internal class PhilosopherParty
    {
        public int Length => Philosophers.Length;
        public Philosopher[] Philosophers { get; set; }
    }

    public class Philosopher
    {
        public bool Isfull => HungerLevel == 0;
        public bool IsEating { get; set; }
        internal AutoResetEvent RightChopstick { get; set; }
        internal AutoResetEvent LeftChopstick { get; set; }
        public int Satisfaction { get; set; }
        public int HungerLevel { get; set; }
        internal int Id { get; }
        public Table myTable { get; set; }

        public Philosopher(int id)
        {
            Id = id;
        }


        public Task EatFoodUntillFull()
        {
            return Task.Run(() =>
             {


                 while (!Isfull)
                 {
                     lock (myTable.Locker)
                     {

                         while (!myTable.FoodAvailable || IsEating)
                             Monitor.Wait(myTable.Locker);
                         Eat(new TakeBitEventArgs { });

                     }
                     
                 }
                 Console.WriteLine($"Eater# {Id} is full");
             });
        }

        public delegate void EatHandler(Philosopher foodItem, TakeBitEventArgs e);

        public event EatHandler EatFromTable;

        public void Eat(TakeBitEventArgs e)
        {
            IsEating = true;
            EatFromTable?.Invoke(this, e);
        }

    }

    public class TakeBitEventArgs : EventArgs
    {
        public int Satisfaction { get; set; }
    }

    public class FoodItem
    {
        //Should fire event when no bites are left
        public int Bites { get; set; }
        public int SatisfactionAmount { get; private set; }


        public delegate void TakeBiteHandler(Philosopher eater, TakeBitEventArgs e);

        public event TakeBiteHandler TakeBiteEvent;

        public bool TakeBite()
        {


            if (Bites == 0)
            {
                NoFoodLeft?.Invoke();
                return false;
            }
            else
            {
                Bites--;
            }
            return true;
        }

        public delegate void NoRemainingBites();

        public NoRemainingBites NoFoodLeft;

        protected virtual void OnTakeBiteEvent(Philosopher eater, TakeBitEventArgs e)
        {
            TakeBiteEvent?.Invoke(eater, e);
        }
    }
}
