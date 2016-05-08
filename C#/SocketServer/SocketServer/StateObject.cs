using System.Net.Sockets;

namespace SocketServer
{
    public class StateObject
    {
        public Socket WorkSocket;
        public readonly byte[] Buffer = new byte[BufferSize];
        public const int BufferSize = 65536;
        public long ByteReceived = 0;
        public long ByteSent = 0;
    }
}