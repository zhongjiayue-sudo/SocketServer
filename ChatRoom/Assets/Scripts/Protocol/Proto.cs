using System;

//自定义前后端协议，一种类型消息一个协议

[Serializable]
public class ProtoBase
{
    public string name;
}

[Serializable]
public class MessageProto : ProtoBase
{
    public string content;
}

[Serializable]
public class ColorProto : ProtoBase
{
    public int color;
}

[Serializable]
public class ReadyProto : ProtoBase
{

}

[Serializable]
public class PlayProto : ProtoBase
{
    public int color;
    public int x;
    public int y;
}

[Serializable]
public class EndProto : ProtoBase
{
    public int color;
}
