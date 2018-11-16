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
            UserList.Add(new User("Bob", Endpoint));

            Job job = new Job((byte)UserList[0].JobList.Count(), 5, UserList[0].Endpoint, Endpoint);
            Console.WriteLine($"Starting Job ID {UserList[0].JobList.Count()}");
            UserList[0].JobList.Add(job);
            JobManager(job);

            Listener(Client).Wait();
        }
        public static async Task JobManager(Job job)
        {
            job.IsActive = true;
            while (job.IsActive)
            {
                if (job.Type == 2)
                {
                    Console.WriteLine($"Sending Confirmation for Job ID {job.ID}...");
                    Speaker(new byte[] { 2, job.ID }, Client, Endpoint);
                    if (job.ElapsedTime.ElapsedMilliseconds > 5)
                    {
                        job.IsActive = false;
                        return;
                    }
                    await Task.Delay(1000);
                }
                else if (job.Type == 5)
                {
                    if (job.ByteList.Count == 25)
                    {
                        Console.WriteLine($"Byte List Full!");
                        Job j = new Job(job.ID, 2, job.Employee, job.Employer);
                        UserList[0].JobList.Add(j);
                        JobManager(j);
                        job.IsCompleted = true;
                        job.IsActive = false;
                        return;
                    }
                    Speaker(new byte[] { 5, job.ID }, Client, Endpoint);
                }
                await Task.Delay(1000);
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
                Console.WriteLine($"Received Information.. {data.Bytes[0]} // {remoteEP}");
                DataProcessor(data.Bytes, client, remoteEP);
            }
        }

        public static async Task DataProcessor(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            Console.WriteLine($"Received Packet Type {packet[0]} ID {packet[1]}");
            if (packet[0] == 2)
            {

            }
            else if (packet[0] == 5)
            {
                foreach (Job job in UserList[0].JobList)
                {
                    if (job.ID == packet[1])
                    {
                        if (!job.ByteList.Any(b => b.Length == packet.Length))
                        {
                            Console.WriteLine($"Adding Packet of Length: {packet.Length}");
                            job.ByteList.Add(packet);
                            job.ByteList.Sort((a, b) => a.Length.CompareTo(b.Length));
                            Console.WriteLine($"ByteList Length of: {job.ByteList.Count}");
                        }
                    }
                }
            }
            else
            {

            }
        }

        public static async Task Speaker(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            if (packet[0] == 2)
            {
                Console.WriteLine($"Sending Confirmation ({packet[0]})");
                client.Send(packet, packet.Length, endpoint);
            }
            else if (packet[0] == 5)
            {
                Console.WriteLine($"Sending Request Type {packet[0]} ID {packet[1]} ");
                client.Send(packet, packet.Length, endpoint);
            }
            else
            {

            }
        }



        /*
        public static string[] informationToWriteBiome = new string[1000000];
        public static string[] informationToWriteMod = new string[1000000];
        public static byte[] receivedBytes = new byte[50000];


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
