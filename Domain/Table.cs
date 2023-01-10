using AnnaWebDiningFin.Data;
using AnnaWebDiningFin.Data.Enums;
using AnnaWebDiningFin.Data.MenuData;
using AnnaWebDiningFin.Infrastructure.Calculations;
using AnnaWebDiningFin.Server;
using System.Runtime.Serialization;

namespace AnnaWebDiningFin.Domain
{
    public class Table
    {
        private static readonly ObjectIDGenerator idGenerator = new();
        private static readonly Random randomizer = new(DateTime.Now.Millisecond);

        public const int TIME_UNIT = 1000;
        public const int WAITER_MAX_WAIT = 3;
        public const int ORDER_MIN = 2;
        public const int ORDER_MAX = 4;

        public long Id { get; private set; }
        public TableState State { get; set; }
        public DateTime timeOfOrder;

        public Table()
        {
            Id = idGenerator.GetId(this, out _);
            State = TableState.Free;
        }

        public void PlaceGuests()
        {
            Task.Run(() =>
            {
                int time = GenerateTime(WAITER_MAX_WAIT);
                Thread.Sleep(time);
                State = TableState.WaitToPlaceOrder;
            });
        }

        public Order GetOrder(Waiter waiter, List<Food> menu)
        {
            waiter.State = WaiterState.Get;
            var time = GenerateTime(ORDER_MAX, ORDER_MIN);
            Thread.Sleep(time);
            LogWriter.Log($"Table {Id} has placed an order in {time} milliseconds");

            var order = GenerateOrder(menu);
            
            foreach (var i in order.Items)
            {
                Console.WriteLine($"{i} is in order");
            }

            State = TableState.ReceiveOrder;
            return order;
        }

        private Order GenerateOrder(List<Food> menu)
        {
            var result = new Order();

            int quantity = GenerateNumber(5);

            for (int i = 0; i < quantity; i++)
            {
                var random = new Random();
                long idx = random.Next(1, 13);

                result.Items.Add(idx);
            }

            SortByPrepTime(menu, result);

            result.TableId = Id;
            result.Priority = GeneratePriority(result);

            return result;
        }

        public Order SortByPrepTime(List<Food> menu, Order o)
        {
            o.MaxWait = o.Items
                   .Select(x => menu
               .First(i => i.Id == x).PreparationTime)
               .OrderByDescending(t => t).First();

            return o;
        }

        private static int GeneratePriority(Order order)
        {
            var priority = 1;
            if ((new int[] { 1, 4 }).ToList().Contains(order.Items.Count))
            {
                priority += 2;
            }

            if (order.MaxWait <= 30)
            {
                priority += 1;
            }

            return priority;
        }


        public static int GenerateTime(int max, int min = 0)
        {

            return randomizer.Next(min * TIME_UNIT, max * TIME_UNIT);
        }

        public static int GenerateNumber(int max, int min = 1)
        {
            if (min > max)
            {
                int temp = min;
                min = max;
                max = temp;
            }
            return randomizer.Next(min, max + 1);
        }

    }
}