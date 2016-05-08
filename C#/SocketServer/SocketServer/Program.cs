using System.Net;

namespace SocketServer
{
    public static class Program
    {
        public static int Main()
        {
            //new SocketListener(1337, IPAddress.Parse("127.0.0.1")).StartListening();
            new UdpSocketListener(1337, IPAddress.Parse("127.0.0.1")).StartListening();
            return 0;
        }
    }
}
