using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static bool AcceptConnections = true;
        public static bool messageReceived = false;
        public static bool messageConfirmation = false;
        public static int connectedUsers = 0;
        public static List<User> UserList = new List<User>();
        public static UdpClient Client = new UdpClient(56000);
        public static IPEndPoint Endpoint = new IPEndPoint(IPAddress.Parse("24.20.157.144"), 57000); // endpoint where server is listening
        public static byte delay = 10;

        static void Main(string[] args)
        {
            Listener(Client);

            Task.Delay(1000);
            Speaker(new byte[] { 11 }, Client, Endpoint);

            while (AcceptConnections)
            {
                Task.Delay(1000);
            }
        }

        public static async Task Listener(UdpClient client)
        {
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();
            while (AcceptConnections == true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 56000);
                Console.WriteLine("Listening for Information..");
                Data data = new Data(await Task.Run(() => client.Receive(ref remoteEP)), remoteEP);
                Console.WriteLine($"Received Information.. {data.Byte[0]} // {remoteEP}");
                if (data.Byte[0] == 2)
                {

                }
                else
                {
                    if (data.Byte[0] == delay + 2)
                    {
                        delay = data.Byte[0];
                        Console.WriteLine($"New Delay: {delay}");
                        Speaker(new byte[] { (byte)(delay + 1) }, client, remoteEP);
                        Speaker(new byte[] { 2, (byte)(delay + 1) }, client, remoteEP);
                    }
                }
            }
        }

        public static async Task Speaker(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            if (packet[0] == 2)
            {
                Speaker2(packet, client, endpoint);
            }
            else
            {
                SpeakerElse(packet, client, endpoint);
            }
        }

        public static async Task Speaker2(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            while (packet[1] > delay)
            {
                Console.WriteLine($"Sending Confirmation ({packet[0]})");
                client.Send(packet, 1, endpoint);
                await Task.Delay(1000);
            }
        }

        public static async Task SpeakerElse(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            while (packet[0] > delay)
            {
                Console.WriteLine($"Requesting Delay Increase to: {packet[0]}");
                client.Send(packet, 1, endpoint);
                await Task.Delay(500);
            }
        }



        /*public static IPEndPoint Endpoint = new IPEndPoint(IPAddress.Parse("24.20.157.144"), 57000); // endpoint where server is listening
        public static bool messageReceived = false;
        public static bool messageStarted = false;
        public static bool messageCompleted = false;
        public static bool receiveStarted = false;
        public static string[] informationToWriteBiome = new string[1000000];
        public static string[] informationToWriteMod = new string[1000000];
        public static byte[] receivedBytes = new byte[50000];
        
        static void Main(string[] args)
        {
            NewMain().Wait();
        }

        public static async Task NewMain()
        {
            byte[] Data = new byte[50000];
            string Username;
            UdpClient Client = new UdpClient();

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
            Console.Write($"Connection Established! {Endpoint}\n");

            ReceiveData(Client);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Client.Send(new byte[] { 10, 1 }, 2);
            
            for (int i = 0; i < 1000; i++)
            {
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    Client.Send(new byte[] { 10, 1 }, 2);
                    Console.WriteLine("Sent 10");
                    stopwatch.Restart();
                }
                else if (messageStarted == true)
                {
                    break;
                }
                else { Thread.Sleep(50); }
            }

            while (!messageCompleted)
            {
                Thread.Sleep(500);
            }
            Console.WriteLine($"Completed! {informationToWriteBiome.Count()} {informationToWriteMod.Count()}");
        }

        public static async Task ReceiveData(UdpClient client)
        {
            for (int i = 0; i < 20; i++)
            {
                messageReceived = false;

                receivedBytes = await Task.Run(() => GrabPacket(client));

                Stopwatch stopwatch = new Stopwatch();
                messageStarted = true;

                for (int ii = 0; ii < 50000; ii++)
                {
                    informationToWriteBiome[(i * 50000) + ii] = receivedBytes[ii].ToString();
                }

                messageReceived = false;
            }

            for (int i = 0; i < 20; i++)
            {
                messageReceived = false;

                receivedBytes = await Task.Run(() => GrabPacket(client));

                client.Send(new byte[] { 2 }, 1);

                while (!messageReceived)
                {
                    Thread.Sleep(50);
                }

                for (int ii = 0; ii < 50000; ii++)
                {
                    informationToWriteMod[(i * 50000) + ii] = receivedBytes[ii].ToString();
                }

                messageReceived = false;
            }

            messageCompleted = true;
        }

        public static async Task<byte[]> GrabPacket(UdpClient client)
        {
            byte[] b = new byte[50000];
            Console.WriteLine("Listening..");
            b = client.Receive(ref Endpoint);
            messageReceived = true;
            Console.WriteLine($"{b[0].ToString()}");
            return b;
        }*/
    }
}
