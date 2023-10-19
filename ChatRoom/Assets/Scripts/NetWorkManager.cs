using System;
using System.Net;
using System.Net.Sockets;
using UnityEditor.VersionControl;

public static class NetWorkManager
{
    public static Socket socket;

    public static byte[] buffer = new byte[1024];

    public static void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint point = new IPEndPoint(IPAddress.Parse("10.0.0.39"), 8848);
        socket.Connect(point);
        socket.BeginReceive(buffer, 0, 1024, 0, ReceiveCallBack, socket);
    }

    /// <summary>
    /// 接收的回调
    /// </summary>
    /// <param name="result"></param>
    public static void ReceiveCallBack(IAsyncResult result)
    {
        Socket socket = (Socket)result.AsyncState;
        int count = socket.EndReceive(result);//终止接收，获取数据长度

        //BeginReceive异步的进程中修改的text,在Unity中主线程中不起作用，所以不会更新，将要执行的方法存入队列中，在主线程上调用
        //ChatManager.Instance.content.text += "\n" + System.Text.Encoding.UTF8.GetString(buffer, 0, count);
        GameManager.Instance.actions.Enqueue(() =>
        {
            ChatManager.Instance.content.text += "\n" + System.Text.Encoding.UTF8.GetString(buffer, 0, count);
        });
        socket.BeginReceive(buffer, 0, 1024, 0, ReceiveCallBack, socket);
    }

    public static void Send(string message)
    {
        //System.Text.Encoding.UTF8,string和byte[]之间转换
        socket.Send(System.Text.Encoding.UTF8.GetBytes(message));
    }
}
