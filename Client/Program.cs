using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;



namespace Client
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

    class Program
    {
        static void Main(string[] args)
        {
           

            //서버에 붙을 소켓
            for (int i = 0; i < 1; i++)
            {
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4000);
                serverSocket.Connect(endPoint);

                Messege clientMsg = new Messege("안녕하세요");
                string json = JsonConvert.SerializeObject(clientMsg);
                //// 패킷 만들기
                byte[] sendMsg = Encoding.UTF8.GetBytes(json);
                ushort msgLength = (ushort)sendMsg.Length;
                //길이  자료
                //[][] [][][][][]
                byte[] lengthBuffer = new byte[2];
                lengthBuffer = BitConverter.GetBytes(msgLength); //랭스 길이를 바이트로 변환해놓기 2자릿수.
                byte[] sendBuffer = new byte[lengthBuffer.Length + sendMsg.Length]; //메시지 길이 + 메시지 자료

                Buffer.BlockCopy(lengthBuffer, 0, sendBuffer, 0, 2);
                Buffer.BlockCopy(sendMsg, 0, sendBuffer, 2, msgLength);
                ////
                
                //패킷 전송하기
                int sendLength = serverSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);

                ///받기
                int recvLength = serverSocket.Receive(lengthBuffer, 2, SocketFlags.None);
                //바이트로 넘어온 데이터를 메시지 길이 인트로 변환
                ushort recvMsgLength = (ushort)BitConverter.ToInt16(lengthBuffer, 0);

                byte[] recvBuff = new byte[4096];
                int recv = serverSocket.Receive(recvBuff, recvMsgLength, SocketFlags.None);
                string recvMsg = Encoding.UTF8.GetString(recvBuff);
                Console.WriteLine(recvMsg);
                serverSocket.Close();

            }

        }
    }
}


/*
 * 서버
 * 소켓만들기 - 인터넷, ip4, tcp
 * 소켓 바인드 - 아이피 주소와 내가만든 소켓을 연결
 * 소켓 리슨 - 내가 만든 소켓 듣는 상태로 변경
 * 소켓 어쎕 - 리슨 중에 들어오는 애 있을때까지 기다렸다가 받아줌 -> 반환 클라 소켓
 * 반환된 클라 소켓에 - 센드 혹은 리십
 * 
 * 클라
 * 소켓 만들기 - 인터넷, ip4, tcp
 * 서버로 소켓 연결 - (안에 목적지 endPoint 만들어서 넣기)
 * 서버로 소켓 센드, 리십
 */