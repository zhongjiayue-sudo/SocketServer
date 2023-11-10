using System;
using System.Collections.Generic;
using UnityEngine;

public class WuZiQiManager : MonoBehaviour
{
    public GameObject crossPrefab;
    public Transform crossContainer;

    public GameObject chessPrefab;
    public Transform chessContainer;

    public static WuZiQiManager Instance;

    public static readonly int WIDTH = 10;
    public static readonly int HIGHT = 10;

    public GameStatus selfColor = GameStatus.Waiting;//本地棋子什么颜色
    public GameStatus global = GameStatus.Waiting;//全局状态，现在轮到谁走

    public Queue<Action> actions = new Queue<Action>();

    // Update is called once per frame
    void Update()
    {
        if (actions.Count > 0)
        {
            actions.Dequeue().Invoke();
        }
    }


    public enum GameStatus
    {
        Waiting = 0,
        Black = 1,
        White = 2,
        End = 3
    }


    public void Init()
    {
        InitCross();
        NetWorkManager.Connect();
        NetWorkManager.Send(new ReadyProto() { name = "ready" });
    }

    void Start()
    {
        Instance = this;
    }


    private void InitCross()
    {
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HIGHT; j++)
                Instantiate(crossPrefab, new Vector3(i, j), Quaternion.identity, crossContainer);
        Camera.main.transform.position = new Vector3(WIDTH / 2 - 0.5f, HIGHT / 2 - 0.5f, Camera.main.transform.position.z);
        Camera.main.orthographicSize = 7;
    }

    /// <summary>
    /// 点击的时候告诉服务器，服务器返回进行是实例化
    /// </summary>
    /// <param name="cross"></param>
    public void OnClickChess(Cross cross)
    {
        if (global == GameStatus.Waiting || selfColor != global)
            return;

        NetWorkManager.Send(new PlayProto()
        {
            name = "play",
            color = (selfColor == GameStatus.Black ? 0 : 1),
            x = (int)cross.transform.position.x,
            y = (int)cross.transform.position.y,
        });
    }


    public void DealProto(ProtoBase proto)
    {
        if (proto is MessageProto)
        {
            MessageProto msg = proto as MessageProto;
            actions.Enqueue(() =>
            {
                UIManager.Instance.text.text += "\n" + msg.content;
            });
        }
        else if (proto is ColorProto)
        {
            ColorProto colorProto = proto as ColorProto;
            selfColor = colorProto.color == 0 ? GameStatus.Black : GameStatus.White;
        }
        else if (proto is ReadyProto)
        {
            global = GameStatus.Black;
        }
        else if (proto is PlayProto)
        {
            var playProto = proto as PlayProto;
            Instantiate(chessPrefab, new Vector3(playProto.x, playProto.y), Quaternion.identity, chessContainer)
                .GetComponent<SpriteRenderer>().color = new Color(playProto.color, playProto.color, playProto.color);
            if (playProto.color == 0)
                global = GameStatus.Black;
            else
                global = GameStatus.White;
        }
        else if(proto is EndProto)
        {
            global = GameStatus.End;
        }
        Debug.Log(proto.name);
    }
    private void OnApplicationQuit()
    {
        NetWorkManager.Send("");
        NetWorkManager.socket.Close();
    }
}
