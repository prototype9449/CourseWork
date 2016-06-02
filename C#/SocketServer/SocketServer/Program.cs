using System.Net;

namespace SocketServer
{
    public static class Program
    {
        public static int Main()
        {
            var contextInfo = new ContextInfo
            {
                GreetingMessagelength = 5,
                Iterations = 1024,
                SizeOfMessage = 16384
            };

            //new TcpSocketListener(1337, IPAddress.Parse("127.0.0.1"), contextInfo).StartListening();
            new UdpSocketListener(3333, 3334, IPAddress.Parse("127.0.0.1"), contextInfo).StartListening();
            return 0;
        }
    }
}
