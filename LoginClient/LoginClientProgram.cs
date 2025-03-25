using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LoginClient
{
    class Messege
    {
        public string code;
        public string param1;
        public string param2;

        public Messege(string _code, string _param1, string _param2)
        {
            code = _code;
            param1 = _param1;
            param2 = _param2;
        }

        ~Messege()
        {

        }
    }


    public class LoginClientProgram
    {
        static void ChatInput()
        {
            while (true)
            {
                // Console.Clear();
                Console.WriteLine("아이디 입력");
                string inputIdStr = Console.ReadLine();
                Console.WriteLine("비밀번호 입력");
                string inputPwdStr = Console.ReadLine();

                Messege signMsg = new Messege("Chat", inputIdStr, inputPwdStr);
                string json = JsonConvert.SerializeObject(signMsg);
                //// 패킷 만들기
                byte[] sendMsg = Encoding.UTF8.GetBytes(json);
                ushort msgLength = (ushort)sendMsg.Length;
                //길이  자료
                //[][] [][][][][]
                byte[] lengthBuffer = new byte[2];
                lengthBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)msgLength)); //랭스 길이를 바이트로 변환해놓기 2자릿수.
                byte[] sendBuffer = new byte[lengthBuffer.Length + sendMsg.Length]; //메시지 길이 + 메시지 자료

                Buffer.BlockCopy(lengthBuffer, 0, sendBuffer, 0, 2);
                Buffer.BlockCopy(sendMsg, 0, sendBuffer, 2, msgLength);
                ////
                //패킷 전송하기
                int sendLength = serverSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
            }

        }

        static void RecvMsg()
        {
            while (true)
            {
                byte[] lengthBuffer = new byte[2];
                int recvLength = serverSocket.Receive(lengthBuffer, 2, SocketFlags.None);
                //바이트로 넘어온 데이터를 메시지 길이 인트로 변환
                ushort recvMsgLength = (ushort)BitConverter.ToInt16(lengthBuffer, 0);

                byte[] recvBuff = new byte[recvMsgLength];
                int recv = serverSocket.Receive(recvBuff, recvMsgLength, SocketFlags.None);
                string recvMsg = Encoding.UTF8.GetString(recvBuff);
                Console.WriteLine(recvMsg);
            }

        }
        static Socket serverSocket;
        static void Main(string[] args)
        {

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.44"), 4000);
            serverSocket.Connect(endPoint);

            Thread inputTr = new Thread(new ThreadStart(ChatInput));
            Thread recvTr = new Thread(new ThreadStart(RecvMsg));

            inputTr.Start();
            recvTr.Start();
            inputTr.Join();
            recvTr.Join();
            /////받기

            //serverSocket.Close();


        }
    }
}
