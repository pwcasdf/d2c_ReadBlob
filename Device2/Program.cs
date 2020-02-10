using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;


namespace Device2
{
    class Program
    {
        private static DeviceClient deviceClient;
        private readonly static string connectionString = "Device_Connection_String";

        static void Main(string[] args)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

            d2cMsgAsync();
            c2dMsgAsync();

            Console.ReadLine();
        }

        private static async void d2cMsgAsync()
        {
            double minTemperature = 20;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;

                String commandType = "telemetry";
                string typeString = "BGM";
                string toString = "HUB";
                string unitString = "mgdl";

                int valueInt = 100;
                int value1Int = 100;
                int value2Int = 100;
                int value3Int = 100;
                int value4Int = 100;
                int value5Int = 100;
                int value6Int = 100;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    CommandType = commandType,
                    Type = typeString,
                    To = toString,
                    Value = valueInt,
                    Unit = unitString,
                    Time = DateTime.Now.ToString("yyyyMMddhhmm"),
                    Value1 = value1Int,
                    Value2 = value2Int,
                    Value3 = value3Int,
                    Value4 = value4Int,
                    Value5 = value5Int,
                    Value6 = value6Int,
                    DeviceName = "device2",
                    temperature = currentTemperature
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.

                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                // Send the telemetry message
                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > DEVICE2 Sending message: {1}", DateTime.Now.ToString("yyyyMMddhhmm"), messageString);

                await Task.Delay(3000);
            }
        }

        private static async void c2dMsgAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message, DEVICE2: {0}",
                Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}
