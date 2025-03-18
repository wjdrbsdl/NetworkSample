using Newtonsoft.Json;
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

    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(
                IPAddress.Any //랜카드 여러개중 아무거나
                ,4000  //해당 프로그램에 할당할 논리적 주소
                );
            socket.Bind(listenEndPoint); //socket에 주소(랜카드, 포트)를 연결
            socket.Listen(10); //이제 들어오는 정보를 듣고거다. 몇개까지?

            while (true)
            {
                //동기 방식
                Socket clientSocket = socket.Accept(); //손놈과 연결된 소켓 - 여기 포트번호는 랜덤으로 설정되어있음

                byte[] receiveBuffer = new byte[1024];
                int recvLength = clientSocket.Receive(receiveBuffer); //받으면 길이부터 줌
                if (recvLength == 0)
                {
                    //닫은거
                }
                else if (recvLength < 0)
                {

                    //에러
                }
                string resMsgStr = Encoding.UTF8.GetString(receiveBuffer);
                Messege resMsg = JsonConvert.DeserializeObject<Messege>(resMsgStr);
                Console.WriteLine($"클라가 보낸 메시지{resMsg.msg}");

                Messege sendMsg = new Messege("반가워요");
                string sendJson = JsonConvert.SerializeObject(sendMsg);
                byte[] sendData = Encoding.UTF8.GetBytes(sendJson);
                int sendLength = clientSocket.Send(sendData);
                //sendLength가 0 이면 상대쪽이 끊은거
                //0 아래면 어딘진 몰라도 에러

                //이거 안하면 keep alive time();?? 보통 3분?
                clientSocket.Close();
            }
            
        }
    }
}
