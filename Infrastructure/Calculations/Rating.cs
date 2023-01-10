using AnnaWebDiningFin.Domain;

namespace AnnaWebDiningFin.Infrastructure.Calculations
{
    public class Rating
    {
        private static float total = 0;
       
        public static void Rate(float realTime, Order order)
        {
            //Rating calculation
            int rating;

            if (realTime < order.MaxWait)
                rating = 5;
            else if (realTime <= order.MaxWait * 1.1)
                rating = 4;
            else if (realTime <= order.MaxWait * 1.2)
                rating = 3;
            else if (realTime <= order.MaxWait * 1.3)
                rating = 2;
            else if (realTime <= order.MaxWait * 1.4)
                rating = 1;
            else
                rating = 0;


            LogWriter.Log($"{order.Id}th order of rating {rating}. ");
            
           total = rating;

            LogWriter.Log($"Average mark is: {total:f2}\n");
        }
    }
}