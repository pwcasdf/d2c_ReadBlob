using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlobReadAzureFunction
{
    class Program
    {
        private static DeviceClient deviceClient;
        private readonly static string connectionString = "Device_Connection_String";

        static void Main(string[] args)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            
            c2dMsgAsync();
            d2cMsgAsync();

            Console.ReadLine();
        }

        private static async void d2cMsgAsync()
        {
            while (true)
            {
                Console.WriteLine("give me the Device Name which device you want to read");

                string getDeviceName = Console.ReadLine();

                // Create JSON message
                var telemetryDataPoint = new
                {
                    CommandType = "readblob",
                    DeviceName = getDeviceName,
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Send the telemetry message
                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > DEVICE1 Sending message: {1}", DateTime.Now.ToString("yyyyMMddhhmm"), messageString);

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
                Console.WriteLine("Received message: {0}",
                Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}
