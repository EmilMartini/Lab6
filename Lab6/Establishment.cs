﻿using System;
using System.Collections.Concurrent;

namespace Lab6
{
    public class Establishment
    {
        public int MaxGlasses { get; private set; }
        public int MaxChairs { get; private set; }
        public int SimulationSpeed { get; private set; }
        public bool IsOpen { get; set; }
        public int TimeToClose { get; set; }
        public Table Table { get; set; }

        public Establishment(int maxGlasses, int maxChairs, int timeToClose, int simulationSpeed)
        {
            MaxGlasses = maxGlasses;
            MaxChairs = maxChairs;
            TimeToClose = timeToClose;
            SimulationSpeed = simulationSpeed;
            Table = new Table(this);
            IsOpen = true;
        }
    }
}
