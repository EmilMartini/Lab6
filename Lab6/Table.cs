﻿using System;
using System.Collections.Concurrent;

namespace Lab6
{
    public class Table
    {
        BlockingCollection<Glass> GlassesOnTable;
        BlockingCollection<Chair> ChairsAroundTable;

        public Table(Establishment est)
        {
            GlassesOnTable = new BlockingCollection<Glass>();
            ChairsAroundTable = new BlockingCollection<Chair>(est.MaxChairs);
        }

        public void InitTable()
        {
            for (int i = 0; i < ChairsAroundTable.BoundedCapacity; i++)
            {
                var chair = new Chair();
                chair.Available = true;
                ChairsAroundTable.TryAdd(chair);
            }
        }
    }
}
