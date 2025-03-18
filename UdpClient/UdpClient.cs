
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;

public class UdpClient
{
    static void Main(string[] args)
    {
        //Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //IPEndPoint svEndPoint = new IPEndPoint(IPAddress.Loopback, 6000);//목적지

        //byte[] buffer = new byte[1024]; //상대가 이것보다 큰거 보내면 나머진 못받음? 에러남. -
        //  //-- C로 보내면 에러는 안나고 멈춤. 
        //string message = "안녕ㅎ";
        //buffer = Encoding.UTF8.GetBytes(message);
        //int sendLength = serverSocket.SendTo(buffer, buffer.Length, SocketFlags.None, svEndPoint);

        //byte[] buffer2 = new byte[1024];
        //EndPoint remotePoint = svEndPoint;
        //int recv = serverSocket.ReceiveFrom(buffer2, ref remotePoint);

        //string message2 = Encoding.UTF8.GetString(buffer);
        //Console.WriteLine(message2);

        //serverSocket.Close();

        IPHostEntry hostEntry = Dns.GetHostEntry("naver.com");
        foreach (IPAddress item in hostEntry.AddressList)
        {

        }
    }
}
