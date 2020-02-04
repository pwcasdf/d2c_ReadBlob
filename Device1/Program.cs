using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;


namespace Device1
{
    class Program
    {
        private static DeviceClient deviceClient;
        private readonly static string connectionString = "";

        static void Main(string[] args)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
        
            Console.ReadLine();
        }

        private static async void d2cMsgAsync()
        {

        }
    }
}
