using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    public class UdpSocketListener
    {
        private readonly ManualResetEvent _threadManager = new ManualResetEvent(false);
        private EndPoint _receiveEndPoint;
        private EndPoint _sendEndPoint;

        public UdpSocketListener(int portForSending, int portForReceiving, IPAddress ipAdress)
        {
            _sendEndPoint = new IPEndPoint(ipAdress, portForSending);
            _receiveEndPoint = new IPEndPoint(ipAdress, portForReceiving);
        }

        public void StartListening()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveBufferSize = StateObject.BufferSize,
                SendBufferSize = StateObject.BufferSize
            };

            try
            {
                socket.Bind(_receiveEndPoint);

                while (true)
                {
                    _threadManager.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    var stateObject = new StateObject { WorkSocket = socket };
                    socket.BeginReceiveFrom(stateObject.Buffer, 0, StateObject.BufferSize, SocketFlags.None,
                        ref _receiveEndPoint, ReceiveCallback, stateObject);
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

        private void ReceiveCallback(IAsyncResult ar)
        {
            var stateObject = (StateObject)ar.AsyncState;

            var socket = stateObject.WorkSocket;

            int bytesRead = socket.EndReceiveFrom(ar, ref _receiveEndPoint);
            if (bytesRead > 0)
            {
                stateObject.ByteReceived += bytesRead;
                socket.BeginSendTo(stateObject.Buffer.ToArray(), 0, bytesRead, 0, _sendEndPoint, SendCallback, stateObject);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var stateObject = (StateObject)ar.AsyncState;
                var sentBytes = stateObject.WorkSocket.EndSendTo(ar);
                stateObject.ByteSent += sentBytes;

                if (stateObject.ByteSent == StateObject.MaxContentlength)
                {
                    Console.WriteLine("Connection was closed");
                    if (stateObject.ByteReceived != stateObject.ByteSent)
                    {
                        throw new Exception();
                    }
                    Console.WriteLine("total : {0}", stateObject.ByteReceived);
                    stateObject.WorkSocket.Shutdown(SocketShutdown.Both);
                    stateObject.WorkSocket.Close();
                }
                else
                {
                    stateObject.WorkSocket.BeginReceiveFrom(stateObject.Buffer, 0, StateObject.BufferSize, 0, ref _receiveEndPoint,
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