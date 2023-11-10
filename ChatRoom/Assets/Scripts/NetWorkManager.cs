using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

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
        string message = System.Text.Encoding.UTF8.GetString(buffer, 0, count);
        ProtoBase proto = DeCode(message);
        WuZiQiManager.Instance.DealProto(proto);

        //BeginReceive异步的进程中修改的text,在Unity中主线程中不起作用，所以不会更新，将要执行的方法存入队列中，在主线程上调用
        //ChatManager.Instance.content.text += "\n" + System.Text.Encoding.UTF8.GetString(buffer, 0, count);
        /*GameManager.Instance.actions.Enqueue(() =>
        {
            //ChatManager.Instance.content.text += "\n" + System.Text.Encoding.UTF8.GetString(buffer, 0, count);
            //LoginManager.Instance.content.text += "\n" + System.Text.Encoding.UTF8.GetString(buffer, 0, count);
        });*/
        socket.BeginReceive(buffer, 0, 1024, 0, ReceiveCallBack, socket);
    }

    public static void Send(string message)
    {
        //System.Text.Encoding.UTF8,string和byte[]之间转换
        socket.Send(System.Text.Encoding.UTF8.GetBytes(message));
    }

    public static void Send(ProtoBase proto)
    {
        var data = Encode(proto);
        socket.Send(data, 0, data.Length, SocketFlags.None);
    }

    public static byte[] Encode(ProtoBase proto)
    {
        return System.Text.Encoding.UTF8.GetBytes(proto.name + "|" + JsonUtility.ToJson(proto));
    }

    public static ProtoBase DeCode(string message)
    {
        var args=message.Split('|');
        Debug.Log(args[0] + " " + args[1] + " " + args[2]);

        int messageLength = int.Parse(args[0]);
        var messageTemp = args[2].Substring(0, messageLength - args[1].Length - 1);

        return args[1] switch
        {
            "message" => JsonUtility.FromJson<MessageProto>(messageTemp),
            "color" => JsonUtility.FromJson<ColorProto>(messageTemp),
            "ready" => JsonUtility.FromJson<ReadyProto>(messageTemp),
            "play" => JsonUtility.FromJson<PlayProto>(messageTemp),
            "end" => JsonUtility.FromJson<EndProto>(messageTemp),
            _ => null,
        };
    }
}
