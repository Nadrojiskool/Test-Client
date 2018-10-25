using System;
using System.Collections.Generic;
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

            string[] informationToWriteBiome = new string[1000000];
            string[] informationToWriteMod = new string[1000000];
            byte[] receivedData = new byte[50000];

            UdpState state = new UdpState();
            state.Endpoint = Endpoint;
            state.Client = Client;

            // establish connection
            Username = Console.ReadLine();
            Client.Send(Encoding.Default.GetBytes(Username), Encoding.Default.GetBytes(Username).Count());
            Client.Receive(ref Endpoint);
            Console.Write($"Connection Established! {Endpoint}\n");

            // receive data
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("Listening for Messages");
                Client.BeginReceive(new AsyncCallback(ReceiveCallback), state);
                while (messageReceived != true)
                {
                    Thread.Sleep(50);
                }
                Client.Send(new byte[] { 1 }, 1);

                messageReceived = false;
            }

            // then receive data

            Console.Read();
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint endpoint = (IPEndPoint)((UdpState)(ar.AsyncState)).Endpoint;
            UdpClient client = (UdpClient)((UdpState)(ar.AsyncState)).Client;

            byte[] receiveBytes = client.EndReceive(ar, ref endpoint);

            Console.WriteLine($"Received Response! {receiveBytes.Count()}");
            messageReceived = true;
        }
    }
}
