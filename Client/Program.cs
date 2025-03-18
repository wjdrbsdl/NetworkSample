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

                string jsonMessge = "{\"키\" : \"값\"}";
                Messege clientMsg = new Messege("안녕하세요");
                string json = JsonConvert.SerializeObject(clientMsg);
                byte[] sendBuff = Encoding.UTF8.GetBytes(json);
                int sendLength = serverSocket.Send(sendBuff);
                //Console.WriteLine("보낸 길이 " + sendLength);

                
                //이미지 파일 바이트로 받음
                byte[] recvBuff = new byte[100];
                int recvLength = serverSocket.Receive(recvBuff);
                int imageLength = BitConverter.ToInt32(recvBuff, 0); //이미지 데이터 길이
                byte[] fiBuff = new byte[imageLength]; //이미지 총 길이로 버프 생성
                int copyPoint = 0;
                //길이 알려주던 4 바이트 빼서 진행
                Array.Copy(recvBuff, 4, fiBuff, copyPoint, recvLength-4);
                copyPoint += recvLength - 4;
                //전달받은 크기를 다 받을때까지 루프 
                while (copyPoint < imageLength)
                {
                    byte[] rereBuff = new byte[100];
                    int rere = serverSocket.Receive(rereBuff);
                    Array.Copy(rereBuff, 0, fiBuff, copyPoint, rere);
                    copyPoint += rere;
                }

                File.WriteAllBytes("5.webp", fiBuff);


                serverSocket.Close();


                ///*
                // */
                //string jsonStr = "FFF";
                //JObject jsonTest = JObject.Parse(jsonStr);
                //JObject answer = new JObject();
                //answer.Add("키", "메시지");
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