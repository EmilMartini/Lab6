﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Bouncer
    {
        public event Action<string> Log;
        Random random = new Random();

        List<string> patronNames = new List<string>()
        {
            "Emma",
            "Olivia",
            "Ava",
            "Isabella",
            "Sophia",
            "Charlotte",
            "Mia",
            "Amelia",
            "Harper",
            "Evelyn",
            "Abigail",
            "Emily",
            "Elizabeth",
            "Mila",
            "Ella",
            "Avery",
            "Sofia",
            "Camila",
            "Aria",
            "Scarlett",
            "Victoria",
            "Madison",
            "Luna",
            "Grace",
            "Chloe",
            "Liam",
            "Noah",
            "William",
            "James",
            "Oliver",
            "Benjamin",
            "Elijah",
            "Lucas",
            "Mason",
            "Logan",
            "Alexander",
            "Ethan",
            "Jacob",
            "Michael",
            "Daniel",
            "Henry",
            "Jackson",
            "Sebastian",
            "Aiden",
            "Matthew",
            "Samuel",
            "David",
            "Joseph",
            "Carter",
            "Owen"
            };
        bool BusArrived { get; set; }
        DateTime BusTimer { get; set; }
        double SimulationSpeed { get; set; }
        double BouncerSpeed { get; set; }
        int PatronsPerEntry { get; set; }
        enum State { Waiting, Working, LeavingWork, StopBouncer}
        State CurrentState { get; set; }

        public Bouncer(Establishment establishment)
        {
            BouncerSpeed = establishment.BouncerSpeed;
            SimulationSpeed = establishment.SimulationSpeed;
            PatronsPerEntry = establishment.PatronsPerEntry;
            if (establishment.isBusloadState)
            {
                BusTimer = DateTime.Now + new TimeSpan(0, 0, 20);
            }
        }
        public void Simulate(Establishment establishment, CancellationToken ct)
        {
            CurrentState = State.Working;
            Task.Run(() =>
            {
                while(CurrentState != State.StopBouncer && !ct.IsCancellationRequested)
                {
                    switch (CurrentState)
                    {
                        case State.Waiting:
                            Wait(ct, establishment);
                            break;
                        case State.Working:
                            Work(establishment, ct);
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                    }
                }
            });
        }
        private void LeavingWork()
        {
            Log("Bouncer has left the pub.");// => Logger.Log(LogLevel.Info, Category.Bouncer, "Bouncer has left the pub.");
            currentState = State.StopBouncer;
        }
        private void Work(Establishment establishment, CancellationToken ct)
        {
            if (!establishment.IsOpen)
            {
                CurrentState = State.LeavingWork;
                return;
            }
            for (int i = 0; i < PatronsPerEntry; i++)
            {
                establishment.TotalPatrons++;
                Patron patron = new Patron(patronNames[random.Next(0, patronNames.Count - 1)], establishment, ct);
                establishment.CurrentPatrons.Insert(0, patron);
            }
            CurrentState = State.Waiting;
        }
        private void Wait(CancellationToken ct, Establishment establishment)
        {
            var timeToSleep = CalculateTimeToSleep(3000, 10001);
            while((DateTime.Now < timeToSleep) && !ct.IsCancellationRequested && establishment.IsOpen)
            {
                Thread.Sleep(10);
                if (!establishment.isBusloadState)
                {
                    continue;
                }
                if (busArrived && patronsPerEntry != establishment.PatronsPerEntry)
                {
                    patronsPerEntry = establishment.PatronsPerEntry;
                }
                if (!busArrived)
                {
                    if (DateTime.Now < BusTimer)
                    {
                        continue; 
                    } 
                    else
                    {
                        PatronsPerEntry = 20;
                        Log("Bus arrived");
                        BusArrived = true;
                        break;
                    }
                }
            }
            CurrentState = State.Working;
        }
        private DateTime CalculateTimeToSleep(int minRange, int maxRange)
        {
            int timeToSleepInMs = SpeedModifier(random.Next(minRange, maxRange));
            return DateTime.Now + new TimeSpan(0, 0, 0, 0,timeToSleepInMs);
        }
        private int SpeedModifier(int StartTime)
        {
            return (int)((StartTime / BouncerSpeed) / SimulationSpeed);
        }
    }
}