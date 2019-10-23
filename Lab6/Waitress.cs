﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    public class Waitress
    {
        // public static event PatronEvent;
        public enum State { WaitingForDirtyGlass, PickingUpGlass, WalkingToSink, WalkingToTable, CleaningGlass, LeavingWork, ShelfingGlass }
        ConcurrentBag<Glass> carryingGlasses;

        public delegate void WaitressEvent(string s);
        public event WaitressEvent Log;
        public State CurrentState { get; set; }
        public Waitress(Table table, Bar bar)
        {
            CurrentState = State.WalkingToTable;
            carryingGlasses = new ConcurrentBag<Glass>();
        }
        public void Simulate(Table table, Bar bar)
        {
            Task.Run(() =>
            {
                do
                {
                    switch (CurrentState)
                    {
                        case State.WaitingForDirtyGlass:
                            WaitingForDirtyGlass(table);
                            break;
                        case State.PickingUpGlass:
                            PickingUpGlass(table);
                            break;
                        case State.WalkingToSink:
                            WalkingToSink();
                            break;
                        case State.WalkingToTable:
                            WalkingToTable();
                            break;
                        case State.CleaningGlass:
                            CleaningGlass();
                            break;
                        case State.LeavingWork:
                            LeavingWork();
                            break;
                        case State.ShelfingGlass:
                            ShelfingGlass(bar);
                            break;
                        default:
                            break;
                    }
                } while (CurrentState != State.LeavingWork);
            });
        }
        void ShelfingGlass(Bar bar)
        {
            foreach (var glass in carryingGlasses)
            {
                bar.Shelf.Add(glass);
                carryingGlasses = new ConcurrentBag<Glass>(carryingGlasses.Except(new[] { glass }));
            }
            CurrentState = State.WalkingToTable;
        }
        bool CheckTableForDirtyGlass(Table table)
        {
            if (table.GlassesOnTable.Count > 0)
            {
                return true;
            }
            return false;
        }
        void LeavingWork()
        {
            Log("is leaving work");
            Thread.Sleep(5000);
            //Log Has left the establishment
        }
        void CleaningGlass()
        {
            //CleaningGlassEvent();
            Thread.Sleep(15000);
            foreach (var glass in carryingGlasses) // gör med lambda sedan
            {
                glass.CurrentState = Glass.State.Clean;
            }
            CurrentState = State.ShelfingGlass;
        }
        void WalkingToTable()
        {
            Log("is walking to the table");
            Thread.Sleep(5000);
            CurrentState = State.WaitingForDirtyGlass;
        }
        void WalkingToSink()
        {
            Log("is walking to the sink");
            Thread.Sleep(5000);
            CurrentState = State.CleaningGlass;
        }
        void PickingUpGlass(Table table)
        {
            Log("is picking up glasses");
            foreach (var glass in table.GlassesOnTable)
            {
                Thread.Sleep(10000);
                table.GlassesOnTable = new ConcurrentBag<Glass>(table.GlassesOnTable.Except(new[] { glass }));
                carryingGlasses.Add(glass);
            }
            CurrentState = State.WalkingToSink;
        }
        void WaitingForDirtyGlass(Table table)
        {
            if (!CheckTableForDirtyGlass(table))
            {
                Log("is waiting for dirty glasses");
            }
            while (!CheckTableForDirtyGlass(table))
            {
                Thread.Sleep(3000);
            }
            CurrentState = State.PickingUpGlass;
        }
    }
}