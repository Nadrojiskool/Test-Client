using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Client
{
    public class Program
    {
        public static IPEndPoint Endpoint = new IPEndPoint(IPAddress.Parse("24.20.157.144"), 57000); // endpoint where server is listening
        public static UdpClient Client = new UdpClient();
        public static bool messageReceived = false;
        public static string[] informationToWriteBiome = new string[1000000];
        public static string[] informationToWriteMod = new string[1000000];
        public static byte[] receivedData = new byte[50000];

        public struct UdpState
        {
            public IPEndPoint Endpoint;
            public UdpClient Client;
        }

        static void Main(string[] args)
        {
            byte[] Data = new byte[50000];
            string Username;

            try
            {
                Client.Connect(Endpoint);
            }
            catch
            {
                Client.Connect(Endpoint);
            }

            // establish connection
            Username = Console.ReadLine();
            Client.Send(Encoding.Default.GetBytes(Username), Encoding.Default.GetBytes(Username).Count());
            Client.Receive(ref Endpoint);
            Console.Write($"Connection Established! {Endpoint}\n");

            ReceiveData();

            while (!messageReceived)
            {
                Thread.Sleep(500);
            }
            Console.WriteLine($"Completed! {informationToWriteBiome.Count()} {informationToWriteMod.Count()}");

            File.WriteAllLines("C:/Users/2/Desktop/test2biome.txt", informationToWriteBiome);
            File.WriteAllLines("C:/Users/2/Desktop/test2mod.txt", informationToWriteMod);

            Console.Read();
        }

        public static async Task ReceiveData()
        {
            for (int i = 0; i < 20; i++)
            {
                receivedData = await Task.Run(() => GrabPacket());
                for (int ii = 0; ii < 50000; ii++)
                {
                    informationToWriteBiome[(i * 50000) + ii] = receivedData[ii].ToString();
                }
                Client.Send(new byte[] { 1 }, 1);
            }

            for (int i = 0; i < 20; i++)
            {
                receivedData = await Task.Run(() => GrabPacket());
                for (int ii = 0; ii < 50000; ii++)
                {
                    informationToWriteMod[(i * 50000) + ii] = receivedData[ii].ToString();
                }
                Client.Send(new byte[] { 1 }, 1);
            }

            messageReceived = true;
        }

        public static byte[] GrabPacket()
        {
            byte[] b = new byte[50000];
            b = Client.Receive(ref Endpoint);
            return b;
        }
    }
}
