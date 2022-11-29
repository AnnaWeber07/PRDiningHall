using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AnnaWebDiningFin.Data.Enums;
using AnnaWebDiningFin;
using AnnaWebDiningFin.Infrastructure.Calculations;
using AnnaWebDiningFin.Domain;

namespace AnnaWebDiningFin.Server
{
    public class DinningHallStartup
    {
        private static HttpListener listener;
        private static string receiveUrl = "http://localhost:8082/";
        private static string sendUrl = "http://localhost:8081/";

        private Dinning dining;


        public async Task HandleIncomingConnections()
        {
            bool isRunning = true;

            while (isRunning)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/ready")
                {
                    using StreamReader streamReader = new(request.InputStream, request.ContentEncoding);
                    Order order = JsonSerializer.Deserialize<Order>(streamReader.ReadToEnd());
                    dining.ServeOrder(order);
                }

                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/shutdown")
                {
                    Console.WriteLine("Shutdown of server");
                    isRunning = false;
                }

                response.StatusCode = 200;
                response.Close();
            }
        }

        public void SendOrder(Waiter waiter, Order order, Table table)
        {
            using var client = new HttpClient();

            var message = JsonSerializer.Serialize(order);
            string mediaType = "application/json";

            var response = client.PostAsync(sendUrl + "order", new StringContent(message, Encoding.UTF8, mediaType))
                                 .GetAwaiter()
                                 .GetResult();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                waiter.State = WaiterState.Wait;
                LogWriter.Log($"Order {order.Id} was sent.");
            }
        }

        

        public async void Start(Dinning dining)
        {
            this.dining = dining;

            listener = new HttpListener();
            listener.Prefixes.Add(receiveUrl);
            listener.Start();
            
            await HandleIncomingConnections();

            listener.Close();
        }
    }
}
