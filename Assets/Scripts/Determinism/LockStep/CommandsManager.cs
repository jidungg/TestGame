using FreeNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

/// <summary>
/// 네트워크 통신 시 락스텝 동기화 처리
/// </summary>
class CommandsManager : MonoBehaviour
{
    public static CommandsManager instance;
    //현재 턴 번호
    public UInt64 nowTurn;
    //턴당 프레임 수
    const byte framesPerTurn = 6;
    //서버로부터 받은 명령들 홀수턴엔 0번인덱스,짝수턴엔 1번인덱스의 큐가 실행된다.
    CommandsQueue commandsReceved;
    // 네트워크 데이터 송,수신을 위한 네트워크 매니저 레퍼런스.
    CNetworkManager networkManager;

    string text;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);


        nowTurn = 0;
        commandsReceved = new CommandsQueue();
        Application.targetFrameRate = 30;
        this.networkManager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();
    }

    /// <summary>
    /// 턴 시작 코루틴, 이 코루틴을 실행하면 턴들이 시작됨.
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnsCoroutine()
    {
        networkManager.messageReceiver = this;
        byte frame = 1;
        Debug.Log("TurnsCoroutine Starts");
        while (true)
        {
            switch (frame)
            {
                case 1:
                    nowTurn++;
                    //저저번턴에 받은명령들(이번턴에 실행할 명령들) 실행하기
                    commandsReceved.ProcessAllCommands();                  
                    break;

                default:

                    break;
            }
            if (++frame >= framesPerTurn)
            {
                frame = 1;
            }
            yield return new WaitForFixedUpdate();
        }

    }

    public void OnRecv(CPacket msg)
    {
        Command command = new Command(msg);
        Debug.Log("OnRecv protocol: " + command.protocol + " turntoProcess: " + command.turnToProcess+" nowturn: "+CommandsManager.instance.nowTurn) ;
        text = "OnRecv protocol: " + command.protocol + " turntoProcess: " + command.turnToProcess + " nowturn: " + CommandsManager.instance.nowTurn;
        if (command.turnToProcess.Equals(nowTurn))//지금 턴에 실행해야하는 경우
        {
            command.Process();
        }
        else//다음턴에 혹은 그 이후 실행해야하는 경우
        {

            commandsReceved.Add(command);


        }
   
    }
    void OnGUI()//소스로 GUI 표시.
    {
        GUI.Label(new Rect(0, Screen.height * 0.5f,100, 100), text);
    }
}
struct Command
{
    //명령 프로토콜
    public PROTOCOL protocol;
    //명령을 실행할 턴
    public UInt64 turnToProcess;
    //나머지 패킷
    public CPacket packet;

    public Command(CPacket packet)
    {
        protocol = (PROTOCOL)packet.pop_protocol_id();
        turnToProcess = packet.pop_uint64()+2;

        this.packet = packet;
    }
    public void Process()
    {
        Debug.Log("Process " + protocol);
        switch (protocol)
        {
            case PROTOCOL.ROOM_REMOVED:
                CBattleRoom.instance.BackToMain();
                break;

            case PROTOCOL.BUILD_SUCCESS:
                byte playerIndex = packet.pop_byte();
                short buildingType = packet.pop_int16();
                short buildingIndex = packet.pop_int16();
                Debug.Log("building type: " + buildingType.ToString()+" playerindex: "+playerIndex+" buildingIndex: "+buildingIndex);
                BuildManager.instance.Build(playerIndex, DeckManager.instance.NowDeck.GetCardIndex((BuildingType)buildingType), buildingIndex);
                break;

            case PROTOCOL.BUILD_FAILED:
                CBattleRoom.instance.BuildFail();
                break;


        }
    }
}
class CommandsQueue 
{

    public int Count
    {
        get { return list.Count; }
    }

    List<Command> list;


    /// <summary>
    /// 명령 큐 생성
    /// </summary>
    /// <param name="isEven">명령을 실행할 턴이 짝수인지 홀수인지</param>
    public CommandsQueue()
    {
        list = new List<Command>();
    }

    public void Add(Command command)
    {
        Debug.Log("Command process in turn " + command.turnToProcess );

        list.Add(command);

    }


    public void ProcessAllCommands()
    {
        for(int i=0;i<list.Count;i++)
        {
            if (list[i].protocol.Equals(PROTOCOL.ROOM_REMOVED))
            {
                list[i].Process();
                list.Clear();
                continue;
            }
            if (list[i].turnToProcess.Equals(CommandsManager.instance.nowTurn))
            {
                list[i].Process();
                list.RemoveAt(i);
            }

        }
    }

}

