using System.Net.Sockets;
using System.Net;
using System.Net.Security;

namespace ImageSendServer
{
    public class ImageClientProgram
    {
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4000);
            serverSocket.Connect(endPoint);

            FileStream fsOutput = new FileStream("3_copy.webp", FileMode.Create);
            //열었으면 무조건 닫기. 

            int recv = 0;
            do
            {
                byte[] recvBuff = new byte[100];
                recv = serverSocket.Receive(recvBuff);
                if (recv <= 79)
                {

                }
                fsOutput.Write(recvBuff, 0, recv);
                Console.WriteLine("받은 숫자 "+recv);
            } while (recv>= 80);


            fsOutput.Close();
        }
    }
}
