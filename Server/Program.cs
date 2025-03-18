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
                //OS 내부에 버퍼에서 복사해오는데, 자료 전부 가져오는게 아님

                ///받기
                //먼저 길이 부터 받음
                byte[] lengthBuffer = new byte[2];
                int recvLength = clientSocket.Receive(lengthBuffer, 2, SocketFlags.None);
                //바이트로 넘어온 데이터를 메시지 길이 인트로 변환
                int recvMsgLength = BitConverter.ToInt16(lengthBuffer, 0);

                byte[] recvBuff = new byte[1000];
                int recv = clientSocket.Receive(recvBuff, recvMsgLength, SocketFlags.None);
                string recvMsg = Encoding.UTF8.GetString(recvBuff);
                Console.WriteLine(recvMsg);


                //보내기
                string sendMsgStr = "보내본다.";
                byte[] msgBuffer = Encoding.UTF8.GetBytes(sendMsgStr); //메시지 바이트로 만들고
                ushort msgLength = (ushort)msgBuffer.Length;
                byte[] sendLengthBuff = new byte[2];
                sendLengthBuff = BitConverter.GetBytes(msgLength); //길이를 버퍼에 담아놓음

                byte[] packet = new byte[sendLengthBuff.Length + msgBuffer.Length];
                Buffer.BlockCopy(sendLengthBuff, 0, packet, 0, sendLengthBuff.Length);
                Buffer.BlockCopy(msgBuffer, 0, packet, sendLengthBuff.Length, msgBuffer.Length);

                clientSocket.Send(packet);
                //sendLength가 0 이면 상대쪽이 끊은거
                //0 아래면 어딘진 몰라도 에러

                //이거 안하면 keep alive time();?? 보통 3분?
                clientSocket.Close();
            }
            
        }
    }
}
