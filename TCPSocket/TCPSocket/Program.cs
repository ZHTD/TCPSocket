using System;
using System.Collections.Generic;
using System.Text;
using Readearth.Data;

namespace TCPSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer ss = new SocketServer();
            ss.StatTCP(60800);
            ss.ReadData();
        }
    }
}
