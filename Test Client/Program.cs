using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Test_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] Data = new byte[50000];
            UdpClient client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("24.20.157.144"), 57000); // endpoint where server is listening

            try
            {
                client.Connect(ep);
            }
            catch
            {
                client.Connect(ep);
            }

            for (int i = 0; i < 50000; i++)
            {
                Data[i] = 0;
            }

            // send data
            for (int i = 0; i < 20; i++)
            {
                client.Send(Data, 50000);
                var receivedData = client.Receive(ref ep);
                Console.Write($"receive data from {ep}\n");
            }

            // then receive data

            Console.Read();
        }
    }
}
