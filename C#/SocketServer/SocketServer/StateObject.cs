using System.Net.Sockets;

namespace SocketServer
{
    public class StateObject
    {
        public Socket WorkSocket;
        public readonly byte[] Buffer = new byte[BufferSize];
        public const int BufferSize = 32768;
        public long ByteReceived = 0;
        public long ByteSent = 0;
    }

    public class ContextInfo
    {
        public int MaxContentlength
        {
            get { return Iterations * SizeOfMessage + GreetingMessagelength; }
        }
        public int GreetingMessagelength { get; set; }
        public int SizeOfMessage { get; set; }
        public int Iterations { get; set; }
    }
}