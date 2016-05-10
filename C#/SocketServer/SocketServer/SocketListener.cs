using System.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    public class SocketListener
    {
        private readonly ManualResetEvent _threadManager = new ManualResetEvent(false);
        private int _port;
        private IPAddress _ipAdress;

        public SocketListener(int port, IPAddress ipAdress)
        {
            _port = port;
            _ipAdress = ipAdress;
        }

        public void StartListening()
        {
            var ipEndPoint = new IPEndPoint(_ipAdress, _port);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = StateObject.BufferSize,
                SendBufferSize = StateObject.BufferSize
            };

            try
            {
                socket.Bind(ipEndPoint);
                socket.Listen(100);

                while (true)
                {
                    _threadManager.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    socket.BeginAccept(AcceptCallback, socket);
                    _threadManager.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _threadManager.Set();
            var socket = ((Socket)ar.AsyncState).EndAccept(ar);
            Console.WriteLine("Client connected");

            var stateObject = new StateObject { WorkSocket = socket };
            socket.BeginReceive(stateObject.Buffer, 0, StateObject.BufferSize, SocketFlags.None, ReceiveCallback, stateObject);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var stateObject = (StateObject)ar.AsyncState;

            var socket = stateObject.WorkSocket;

            int bytesRead = socket.EndReceive(ar);
            if (bytesRead > 0)
            {
                stateObject.ByteReceived += bytesRead;
                socket.BeginSend(stateObject.Buffer.ToArray(), 0, bytesRead, 0, SendCallback, stateObject);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var stateObject = (StateObject)ar.AsyncState;
                var sentBytes = stateObject.WorkSocket.EndSend(ar);
                stateObject.ByteSent += sentBytes;

                if (stateObject.ByteSent == StateObject.MaxContentlength)
                {
                    Console.WriteLine("Connection was closed");
                    if (stateObject.ByteReceived != stateObject.ByteSent)
                    {
                        throw new Exception();
                    }
                    Console.WriteLine("total : {0}",stateObject.ByteReceived);
                    stateObject.WorkSocket.Shutdown(SocketShutdown.Both);
                    stateObject.WorkSocket.Close();
                }
                else
                {
                    stateObject.WorkSocket.BeginReceive(stateObject.Buffer, 0, StateObject.BufferSize, 0,
                        ReceiveCallback, stateObject);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}