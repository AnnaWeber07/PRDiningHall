using AnnaWebDiningFin.Domain;

namespace AnnaWebDiningFin.Infrastructure.Calculations
{
    public class Rating
    {
        private static float total = 0;
        private static int counter = 0;

        public static void Rate(float realTime, Order order)
        {
            //Rating calculation
            int rating;

            if (realTime < order.MaxWaitingTime)
                rating = 5;
            else if (realTime <= order.MaxWaitingTime * 1.1)
                rating = 4;
            else if (realTime <= order.MaxWaitingTime * 1.2)
                rating = 3;
            else if (realTime <= order.MaxWaitingTime * 1.3)
                rating = 2;
            else if (realTime <= order.MaxWaitingTime * 1.4)
                rating = 1;
            else
                rating = 0;


            LogWriter.Log($"{order.Id}th order of rating {rating}.");
            LogWriter.Log($"Customer waited for {realTime}. Initial max waiting time {order.MaxWaitingTime})");

            counter++;
            total = rating;

            LogWriter.Log($"\nStats:\nTotal number of Orders: {counter}\nAverage mark is: {total / counter:f2}\n");
        }
    }
}