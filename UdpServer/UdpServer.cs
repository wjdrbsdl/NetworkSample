using System.Net;
using System.Net.Sockets;

public class UdpServer
{
    static void Main(string[] args)
    {
        //udp 형태로 만들어놓음. 
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint svEndPoint = new IPEndPoint(IPAddress.Any, 6000);
        serverSocket.Bind(svEndPoint);

        byte[] buffer = new byte[1024]; //상대가 이것보다 큰거 보내면 나머진 못받음?
        EndPoint clientEnPoint = (EndPoint)svEndPoint;
        int recvLength = serverSocket.ReceiveFrom(buffer, ref clientEnPoint);
        int sendLength = serverSocket.SendTo(buffer, clientEnPoint);
        serverSocket.Close();
    }
}

