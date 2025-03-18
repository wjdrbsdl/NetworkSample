using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;



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
            for (int i = 0; i < 10; i++)
            {
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4000);
                serverSocket.Connect(endPoint);

     
                Messege clientMsg = new Messege("안녕하세요");
                string json = JsonConvert.SerializeObject(clientMsg);
                byte[] sendBuff = Encoding.UTF8.GetBytes(json);
                int sendLength = serverSocket.Send(sendBuff);
                //Console.WriteLine("보낸 길이 " + sendLength);

                
                byte[] recvBuff = new byte[1024];
                int recvLength = serverSocket.Receive(recvBuff);
                string resMsgStr = Encoding.UTF8.GetString(recvBuff);
                Messege resMsg = JsonConvert.DeserializeObject<Messege>(resMsgStr);
                Console.WriteLine($"서버가 보낸 메시지{resMsg.msg}");

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