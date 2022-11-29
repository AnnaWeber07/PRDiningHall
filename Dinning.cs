using AnnaWebDiningFin.Data.Enums;
using AnnaWebDiningFin.Data.MenuData;
using AnnaWebDiningFin.Domain;
using AnnaWebDiningFin.Infrastructure.Calculations;
using AnnaWebDiningFin.Server;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;

namespace AnnaWebDiningFin
{
    public class Dinning : BackgroundService
    {
        public DinningHallStartup server;

        // private readonly Menu _menu = new();

        public Menu menu = new();

        private readonly List<Table> _tables = new();
        private readonly List<Waiter> _waiters = new();
        private readonly List<Order> _orders = new();

        private readonly object _tablesLocker = new();
        private readonly object _waitersLocker = new();
        // private readonly object _menuLocker = new();
        private readonly object _ordersLocker = new();

        public List<Table> Tables
        {
            get
            {
                lock (_tablesLocker)
                {
                    return _tables;
                }
            }
        }

        public List<Waiter> Waiters
        {
            get
            {
                lock (_waitersLocker)
                {
                    return _waiters;
                }
            }
        }

        //public List<Food> Menu
        //{
        //    get
        //    {
        //        lock (_menuLocker)
        //        {
        //            return _menu.Values;
        //        }
        //    }
        //}

        public List<Order> Orders
        {
            get
            {
                lock (_ordersLocker)
                {
                    return _orders;
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            GenerateTables(5);
            GenerateWaiters(4);
            menu.PrepareMenu();
            return Task.CompletedTask;
        }

        public Dinning(DinningHallStartup server)
        {
            this.server = server;
            this.server.Start(this);
            menu.PrepareMenu();
        }

        public Table GetFirstWaitingTable()
        {
            Table firstWT = null;

            lock (_tablesLocker)
            {
                firstWT = _tables.FirstOrDefault(x => x.State == TableState.WaitToPlaceOrder);

                if (firstWT != null)
                {
                    firstWT.State = TableState.Ordering;
                }
            }

            return firstWT;
        }

        public void GenerateTables(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Tables.Add(new Table());
            }

            foreach (Table t in Tables)
            {
                t.PlaceGuests();
            }
        }

        private void GenerateWaiters(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Waiters.Add(new(this));
            }
        }

        public void ServeOrder(Order order)
        {
            var table = Tables.First(t => t.Id == order.TableId);
            var waitTime = (DateTime.Now.Ticks - table.timeOfOrder.Ticks) / (10000 * Table.TIME_UNIT);

            LogWriter.Log($"Table {table.Id} received the order number {order.Id}:");

            Rating.Rate(waitTime, order);

            Orders.RemoveAll(x => x.Id == order.Id);
            table.State = TableState.Free;
            table.PlaceGuests();
        }

    }
}