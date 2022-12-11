using AnnaWebDiningFin.Data.Enums;
using AnnaWebDiningFin.Infrastructure.Calculations;
using System.Runtime.Serialization;

namespace AnnaWebDiningFin.Domain
{
    public class Waiter : IDisposable
    {
        private static readonly ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        private readonly Thread _waiterWorkThread;
        private readonly object _waiterStateLocker = new();
        private WaiterState _state;

        public long Id { get; private set; }
        public WaiterState State
        {
            get
            {
                lock (_waiterStateLocker)
                {
                    return _state;
                }
            }
            set
            {
                lock (_waiterStateLocker)
                {
                    _state = value;
                }
            }
        }

        public Dinning Dinning { get; private set; }


        public Waiter(Dinning dinning)
        {
            Id = idGenerator.GetId(this, out _);
            State = WaiterState.Wait;
            Dinning = dinning;

            _waiterWorkThread = new Thread(WaiterWorkMethod)
            {
                IsBackground = true,
                Name = $"WaiterWorkThread_{Id}"
            };
            _waiterWorkThread.Start();
        }

        public void WaiterWorkMethod()
        {
            try
            {
                while (true)
                {
                    if (State == WaiterState.Wait)
                    {
                        var table = Dinning.GetFirstWaitingTable();
                        if (table == null)
                        {
                            continue;
                        }

                        table.State = TableState.Ordering;
                        LogWriter.Log($"Waiter number {Id} reached the Table {table.Id}");

                        Order order = table.GetOrder(this, Dinning.restaurantMenu.Values);
                        table.timeOfOrder = DateTime.Now;
                        Dinning.Orders.Add(order);

                        Dinning.server.SendOrder(this, order, table);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine($"{_waiterWorkThread.Name} interrupted");
            }
        }

        public void Dispose()
        {
            _waiterWorkThread.Interrupt();
        }
    }
}
