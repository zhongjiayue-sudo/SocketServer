using System.Net.Sockets;
using System.Net;


public class ChatRoomServer
{
    public static Socket socket;

    public static List<ClientInfo> clients = new();
    public class ClientInfo
    {
        public Socket socket;

        public byte[] buffer = new byte[1024];

        /// <summary>
        /// 构造函数不用加void
        /// </summary>
        /// <param name="socket"></param>
        public ClientInfo(Socket socket)
        {
            this.socket = socket;
        }
    }

    public static void Main(string[] argv)
    {
        try
        {
            var ip = GetLocalIPv4();
            Console.WriteLine("服务器已启动!");
            Console.WriteLine($"ip地址为:{ip}");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Any, 8848);//任意IP地址
            socket.Bind(point);//想要绑定的客户端
            socket.Listen(0);//开始监听来自其他计算机的连接，0表示无数个
            socket.BeginAccept(AcceptCallBack, socket);
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void AcceptCallBack(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = (Socket)asyncResult.AsyncState;//将accept到的Socket解析出来.解析一个Socket表示已经连接了一个客户端
            Socket client = socket.EndAccept(asyncResult);//Scoket中的数据存在client里，终止
            ClientInfo info = new(client);
            client.BeginReceive(info.buffer, 0, 1024, 0, ReceiveCallBack, info);//info句柄，带有Socket就行
            clients.Add(info);
            socket.BeginAccept(AcceptCallBack, socket);//接收完一个继续等待下一个用户连接
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void ReceiveCallBack(IAsyncResult asyncResult)
    {
        try
        {
            ClientInfo client = (ClientInfo)asyncResult.AsyncState;
            int count = client.socket.EndReceive(asyncResult);//解析一下接受数据长度，终止

            //发送消息给现已连接的客户端
            foreach (var c in clients)
                c.socket.Send(client.buffer, 0, count, 0);
            client.socket.BeginReceive(client.buffer, 0, 1024, 0, ReceiveCallBack, client);//继续异步接收
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static string GetLocalIPv4()
    {
        string hostName = Dns.GetHostName(); //得到主机名
        IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
        for (int i = 0; i < iPEntry.AddressList.Length; i++)
        {
            //从IP地址列表中筛选出IPv4类型的IP地址
            if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
            {
                return iPEntry.AddressList[i].ToString();
            }
        }
        return null;
    }
}