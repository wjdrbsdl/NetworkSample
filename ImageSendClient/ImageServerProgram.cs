using System.Net.Sockets;
using System.Net;

namespace ImageSendClient
{
    internal class ImageServerProgram
    {
        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(
                IPAddress.Any //랜카드 여러개중 아무거나
                , 4000  //해당 프로그램에 할당할 논리적 주소
                );
            listenSocket.Bind(listenEndPoint); //socket에 주소(랜카드, 포트)를 연결
            listenSocket.Listen(10); //이제 들어오는 정보를 듣고거다. 몇개까지?

            Socket clientSocket = listenSocket.Accept();

            FileStream fsInput = new FileStream("1.webp", FileMode.Open);
            byte[] readBuffer = new byte[1000];

            int readSize = 0;
            do 
            {
                readSize = fsInput.Read(readBuffer, 0, readBuffer.Length);
                int sendSize = clientSocket.Send(readBuffer, readSize, SocketFlags.None);
            }
            while (readSize > 0);

            fsInput.Close();
            listenSocket.Close();
            clientSocket.Close();
        }
    }
}
