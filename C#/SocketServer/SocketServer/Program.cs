using System.Net;

namespace SocketServer
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            int iterations = int.Parse(args[0]);
            int sizeOfMessage = int.Parse(args[1]);
            int portForReceiving = int.Parse(args[2]);
            int portForSending = int.Parse(args[3]);
            bool isTcp = args[4] == "tcp";

            var contextInfo = new ContextInfo
            {
                GreetingMessagelength = 5,
                Iterations = iterations,
                SizeOfMessage = sizeOfMessage
            };

            if (isTcp)
            {
                new TcpSocketListener(1337, IPAddress.Parse("127.0.0.1"), contextInfo).StartListening();
            }
            else
            {
                new UdpSocketListener(portForReceiving, portForSending, IPAddress.Parse("127.0.0.1"), contextInfo).StartListening();
            }
            return 0;
        }
    }
}
