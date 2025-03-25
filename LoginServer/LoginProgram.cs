using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using MySqlConnector;

namespace LoginServer
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

    public class LoginProgram
    {
        static object _lock = new object();
        static List<Thread> threadManager = new List<Thread>();
        static void Accept()
        {
            while (true)
            {
                Socket linkSocket = listenSocket.Accept(); //손놈과 연결된 소켓 - 여기 포트번호는 랜덤으로 설정되어있음
                lock (_lock)
                {
                    clientList.Add(linkSocket);
                }
                //Thread claThread = new Thread(() => SendMsg(linkSocket));
                Thread claThread = new Thread(new ParameterizedThreadStart(RecvPacket));
                threadManager.Add(claThread);
                claThread.Start(linkSocket);
            }

        }
        static void RecvPacket(Object _object)
        {
            Socket claSocket = (Socket)_object;
            while (true)
            {
                try
                {
                    byte[] lengthBuffer = new byte[2];
                    int recvLength = claSocket.Receive(lengthBuffer, 2, SocketFlags.None);
                    //바이트로 넘어온 데이터를 메시지 길이 인트로 변환
                    int recvMsgLength = IPAddress.NetworkToHostOrder((short)BitConverter.ToInt16(lengthBuffer, 0));

                    byte[] recvBuff = new byte[recvMsgLength];
                    int recv = claSocket.Receive(recvBuff, recvMsgLength, SocketFlags.None);
                    string recvMsg = Encoding.UTF8.GetString(recvBuff);
                    Console.WriteLine(recvMsg);

                    Messege request = JsonConvert.DeserializeObject<Messege>(recvMsg);
                    string connectionStr = "server=localhost;user=root;database=membership;password=970416";
                    MySqlConnection mySqlConnection = new MySqlConnection(connectionStr);
                    MySqlCommand mySqlCommand = new MySqlCommand();

                    if (request.code == "LogIn")
                    {
                        mySqlConnection.Open();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText =
                            "select* from users where user_id = @user_id and user_pwd = @user_pwd";
                        // "select* from user where user_id = {user_id} and user_pwd = {user_pwd}" //이렇게 하면 공격문도 들어올수있어서 보안 위험
                        //mySqlCommand.CommandText = "select* from users limit 0, 30"; //0부터 30개;
                        mySqlCommand.Prepare();
                        mySqlCommand.Parameters.AddWithValue("@user_id", request.param1);
                        mySqlCommand.Parameters.AddWithValue("@user_pwd", request.param2);
                        //id와 pwd를 조건으로 users에서 아이템을 찾는거라서 저게 둘다 맞아야함. 
                        
                        MySqlDataReader sqlReader = mySqlCommand.ExecuteReader();
                        if (sqlReader.Read())
                        {
                            Console.WriteLine("로그인 성공");
                        }
                        else
                        {
                            Console.WriteLine("로그인 실패");
                        }
                        mySqlConnection.Close();
                    }
                    else if (request.code == "SignUp")
                    {
                        mySqlConnection.Open();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText =
                           "insert into users (user_id, user_pwd, name, email) values ( @id, @pwd, @name, @mail)";
                        // "select* from user where user_id = {user_id} and user_pwd = {user_pwd}" //이렇게 하면 공격문도 들어올수있어서 보안 위험
                        mySqlCommand.Prepare();
                        mySqlCommand.Parameters.AddWithValue("@id", request.param1);
                        mySqlCommand.Parameters.AddWithValue("@pwd", request.param2);
                        mySqlCommand.Parameters.AddWithValue("@name", "testName");
                        mySqlCommand.Parameters.AddWithValue("@mail", "testMail");

                        int result = mySqlCommand.ExecuteNonQuery();
                        Console.WriteLine("결과" + result.ToString());
                    }
                    else if(request.code == "Chat")
                    {
                        Console.WriteLine(request.param1 +" 챗 전달");
                        for (int i = 0; i < clientList.Count; i++)
                        {
                            SendMsg(clientList[i], recvMsg);
                        }
                    }

                }
                catch
                {
                    //여기 락?
                    clientList.Remove(claSocket);
                    return;
                }
            }
        }

        static void SendMsg(Socket _toSocket, string _msg)
        {
            byte[] sendMsg = Encoding.UTF8.GetBytes(_msg);
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
            int sendLength = _toSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
        }

        static Socket listenSocket;
        static List<Socket> clientList = new();
        static void Main(string[] args)
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(
                IPAddress.Any //랜카드 여러개중 아무거나
                , 4000  //해당 프로그램에 할당할 논리적 주소
                );
            listenSocket.Bind(listenEndPoint); //socket에 주소(랜카드, 포트)를 연결
            listenSocket.Listen(10); //이제 들어오는 정보를 듣고거다. 몇개까지?

            List<Socket> checkRead = new(); //감시할것을 만듬
            clientList = new();

            //소켓 마다 리십, 센드를 해야하는데 
            Thread acceptThread = new Thread(new ThreadStart(Accept));
            acceptThread.Start();
            acceptThread.Join();
        }
    }
}
