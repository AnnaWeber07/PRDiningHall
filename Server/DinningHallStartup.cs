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
using System.Runtime.InteropServices;
using AnnaWebDiningFin.Data.MenuData;
using System.Net.Http.Json;

namespace AnnaWebDiningFin.Server
{
    public class DinningHallStartup
    {
        private static HttpListener listener;
        private static string sendUrl = "http://localhost:8081/";
        private static string receiveUrl = "http://localhost:8082/";
        private static string clientServiceUrl = "http://localhost:8084/";
        private static string foodOrderingUrl = "http://localhost:8085/";

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

                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/v2/order")
                {
                    //todo: get order from foodordering
                }

                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/v2/order/{id}")
                {
                    using StreamReader streamReader = new(request.InputStream, request.ContentEncoding);
                    //todo: get order by idm to client - if ready or not

                    GetOrderById getOrderById = new();
                    var sendObject = getOrderById;

                    var json = JsonSerializer.Serialize(sendObject);

                    //serialize and send menu over to client as a response
                    
                    // Get a response stream and write the response to it.

                    byte[] buffer = new byte[] { };

                    response.ContentType = "application/json";

                    buffer = Encoding.ASCII.GetBytes(json);
                    response.ContentLength64 = buffer.Length;

                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
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


        public void SendMenu(RegisterMenu registerMenu)
        {
            using var client = new HttpClient();
            //var sendMenu = JsonSerializer.Serialize(registerMenu);

            string mediaType = "application/json";
            //var postResponse = client.PostAsync(foodOrderingUrl + "register",
            //    new StringContent(sendMenu, Encoding.UTF8, mediaType))
            //    .GetAwaiter()
            //    .GetResult();

            var postResponse = client.PostAsJsonAsync<RegisterMenu>(foodOrderingUrl + "register", registerMenu, CancellationToken.None)
                .GetAwaiter()
                .GetResult();


            if (postResponse.StatusCode == HttpStatusCode.OK)
            {
                LogWriter.Log($"The restaurant's restaurant Menu had been sent");
            }
            else
            {
                LogWriter.Log($"The restaurant's restaurant Menu hadn't been sent");
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
