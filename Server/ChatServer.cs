using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Messege
    {
        public string msg;

        public Messege(string _msg)
        {
            msg = _msg;
        }

        ~Messege()
        {

        }
    }

    class ChatServer
    {
        static object _lock = new object();
        static List<Thread> threadManager = new List<Thread>();
        static void Accept()
        {
            while (true)
            {
                Socket linkSocket = listenSocket.Accept(); //손놈과 연결된 소켓 - 여기 포트번호는 랜덤으로 설정되어있음
                lock (_lock)
                {
                    clientList.Add(linkSocket);
                }
                //Thread claThread = new Thread(() => SendMsg(linkSocket));
                Thread claThread = new Thread(new ParameterizedThreadStart(SendMsg));
                threadManager.Add(claThread);
                claThread.Start(linkSocket);
            }

        }
        static void SendMsg(Object _object)
        {
            Socket claSocket = (Socket)_object;
            while (true)
            {
                try
                {
                    byte[] lengthBuffer = new byte[2];
                    int recvLength = claSocket.Receive(lengthBuffer, 2, SocketFlags.None);
                    //바이트로 넘어온 데이터를 메시지 길이 인트로 변환
                    int recvMsgLength = IPAddress.HostToNetworkOrder((short)BitConverter.ToInt16(lengthBuffer, 0));

                    byte[] recvBuff = new byte[recvMsgLength];
                    int recv = claSocket.Receive(recvBuff, recvMsgLength, SocketFlags.None);
                    string recvMsg = Encoding.UTF8.GetString(recvBuff);
                    Console.WriteLine(recvMsg);


                    //보내기
                    string sendMsgStr = recvMsg + "보내본다.";
                    byte[] msgBuffer = Encoding.UTF8.GetBytes(sendMsgStr); //메시지 바이트로 만들고
                    ushort msgLength = (ushort)msgBuffer.Length;
                    byte[] sendLengthBuff = new byte[2];
                    sendLengthBuff = BitConverter.GetBytes(msgLength); //길이를 버퍼에 담아놓음

                    byte[] packet = new byte[sendLengthBuff.Length + msgBuffer.Length];
                    Buffer.BlockCopy(sendLengthBuff, 0, packet, 0, sendLengthBuff.Length);
                    Buffer.BlockCopy(msgBuffer, 0, packet, sendLengthBuff.Length, msgBuffer.Length);

                    //여기 락?
                    for (int i = 0; i < clientList.Count; i++)
                    {
                        clientList[i].Send(packet);
                    }
                }
                catch
                {
                    //여기 락?
                    clientList.Remove(claSocket);
                    return;
                }
            }
        }

        static Socket listenSocket;
        static List<Socket> clientList = new();
        static void Main(string[] args)
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(
                IPAddress.Any //랜카드 여러개중 아무거나
                , 4000  //해당 프로그램에 할당할 논리적 주소
                );
            listenSocket.Bind(listenEndPoint); //socket에 주소(랜카드, 포트)를 연결
            listenSocket.Listen(10); //이제 들어오는 정보를 듣고거다. 몇개까지?

            List<Socket> checkRead = new(); //감시할것을 만듬
            clientList = new();

            //소켓 마다 리십, 센드를 해야하는데 
            Thread acceptThread = new Thread(new ThreadStart(Accept));
            acceptThread.Start();
            acceptThread.Join();
        }
    }
}
