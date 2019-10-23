﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab6
{
    class SimulationManager
    {
        public List<Patron> CurrentPatrons { get; private set; }
        public Bouncer bouncer { get; private set; }
        public Bartender bartender { get; private set; }
        public Waitress waiter { get; private set; }
        public LogManager logManager { get; private set; }
        public Establishment establishment { get; private set; }
        public SimulationManager(SimulationState stateToRun)
        {
            establishment = GetEstablishment(stateToRun);
            MainWindow window = (MainWindow)App.Current.MainWindow;
            bouncer = new Bouncer(establishment, this);
            bartender = new Bartender(establishment);
            waiter = new Waitress(establishment.Table);
            CurrentPatrons = new List<Patron>();
            logManager = new LogManager(window, this);
            logManager.SubscribeToEvents(this);
            StartSimulation(this);
        }

        private void StartSimulation(SimulationManager simulationManager)
        {
            bouncer.Simulate(simulationManager);
            bartender.Simulate(simulationManager.establishment);
        }
        private Establishment GetEstablishment(SimulationState state)
        {
            switch (state)
            {
                case SimulationState.Default:
                    return new Establishment(8, 9, 120, 1);
            }
            return null;
        }
        public Bartender GetBartender()
        {
            return bartender;
        }
        public Bouncer GetBouncer()
        {
            return bouncer;
        }
        public Waitress GetWaiter()
        {
            return waiter;
        }
    }
}