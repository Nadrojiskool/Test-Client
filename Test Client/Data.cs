using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test_Client
{
    public class Data
    {
        public byte[] Bytes = new byte[50000];
        public IPEndPoint Endpoint;

        public Data(byte[] b, IPEndPoint ep)
        {
            Bytes = b;
            Endpoint = ep;
        }
    }
}
