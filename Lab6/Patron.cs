﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Patron
    { 
        public enum State { WaitingForChair, WaitingForBeer, DrinkingBeer, WalkingToBar, WalkingToChair, LeavingEstablishment, RemovePatron }
        public delegate void PatronEvent(string s);
        public static event PatronEvent Log;

        public string Name { get; private set; }
        public State CurrentState { get; set; }
        public ConcurrentBag<Glass> Holding { get; set; }
        public Patron(string name, Establishment establishment)
        {
            Name = name;
            CurrentState = State.WalkingToBar;
            Holding = new ConcurrentBag<Glass>();
            Simulate(establishment);
        }
        void Simulate(Establishment establishment)
        {
            Task.Run(() =>
            {
                do
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForChair:
                            WaitingForChair(establishment.Table);
                            break;
                        case State.WaitingForBeer:
                            WaitingForBeer(establishment.Bar);
                            break;
                        case State.DrinkingBeer:
                            DrinkingBeer(establishment.Table);
                            break;
                        case State.WalkingToBar:
                            WalkingToBar(establishment.Bar);
                            break;
                        case State.WalkingToChair:
                            WalkingToChair();
                            break;
                        case State.LeavingEstablishment:
                            LeavingEstablishment(establishment);
                            break;
                        default:
                            break;
                    }
                } while (CurrentState != State.RemovePatron);
                RemovePatron(this, establishment);
            });
        }

        private void RemovePatron(Patron patron, Establishment establishment)
        {
            establishment.CurrentPatrons.Remove(patron);
        }

        bool CheckBarTopForBeer(Bar bar)
        {
            if(bar.BarTop.Count > 0)
            {
                return true;
            }
            return false;
        }
        bool CheckForEmptyChair(Table table)
        {
            foreach (var chair in table.ChairsAroundTable)
            {
                if (chair.Available)
                {
                    return true;
                }
            }
            return false;
        }
        void DrinkingBeer(Table table)
        {
            Log($"{this.Name} is drinking a beer");
            Thread.Sleep(15000);
            foreach (var glass in Holding) // gör med lambda sedan
            {
                glass.CurrentState = Glass.State.Dirty;
                table.GlassesOnTable.Add(glass);
                Holding = new ConcurrentBag<Glass>(Holding.Except(new[] { glass }));
            }
            CurrentState = State.LeavingEstablishment;
        }
        void WaitingForChair(Table table) 
        {
            if (!CheckForEmptyChair(table))
            {
                Log($"{this.Name} is waiting for a chair");
            }
            while (!CheckForEmptyChair(table))
            {
                Thread.Sleep(3000);
            }
            foreach (var chair in table.ChairsAroundTable)
            {
                if (chair.Available)
                {
                    chair.Available = false;
                    CurrentState = State.DrinkingBeer;
                    return;
                }
            }

        }
        void WaitingForBeer(Bar bar)
        {
            if (!CheckBarTopForBeer(bar))
            {
                Log($"{this.Name} is waiting for a beer"); //detta är ett generiskt event, vad som helst skulle kunna hända
                //det ända ni gör någonsin är att logga. Då kanske de bör represetera det.
                //Log(this,"Patron is waiting for beer");

            }
            while (!CheckBarTopForBeer(bar))
            {
                Thread.Sleep(3000);
            }
            if (CheckBarTopForBeer(bar)) // och först i kön
            {
                Glass glass = bar.BarTop.ElementAt(0);
                Holding.Add(glass);
                bar.BarTop = new ConcurrentBag<Glass>(bar.BarTop.Except(new[] { glass }));
                bar.BarQueue = new ConcurrentQueue<Patron>(bar.BarQueue.Except(new[] { this }));
                CurrentState = State.WalkingToChair;
            }
            
        }
        void LeavingEstablishment(Establishment establishment)
        {
            foreach (var chair in establishment.Table.ChairsAroundTable)
            {
                if (!chair.Available)
                {
                    chair.Available = true;
                    break;
                }
            }
            Log($"{this.Name} is leaving establishment");
            Thread.Sleep(5000);
            CurrentState = State.RemovePatron;
            
        }
        void WalkingToBar(Bar bar)
        {
            Log($"{this.Name} is walking to the bar.");
            Thread.Sleep(5000);
            bar.BarQueue.Enqueue(this);
            CurrentState = State.WaitingForBeer;
        }
        void WalkingToChair()
        {
            Log($"{this.Name} is walking to a chair");
            Thread.Sleep(5000);
            CurrentState = State.WaitingForChair;
        }
    }
}
