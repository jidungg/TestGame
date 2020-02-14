using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;
using UnityEngine.SceneManagement;
public class BuildManager : MonoBehaviour
{
    public static  BuildManager instance;
    GameUIControl gameUIControl;
    //내건물을 건설할 타일맵
    TileMap tileMap;
    //적의 건물이 건설될 타일맵
    TileMap enemyTileMap;
    //내 타일맵의 영점
    Transform tileMapTransform;
    //적 타일맵 영점
    Transform enemyTileMapTransfrom;
    //현재 커서거 가리키는 타일맵 위의 위치 정보
    Vector3 currentTileCoord;
    //건물 짓기전에 표시해주는 오브젝트들.
    GameObject[] buildingIndicators;
    //내 덱에있는 건물들 풀링용 리스트
    public Queue<GameObject>[] myDeckBuildingsPool;
    //상대 덱에 있는 건물들 풀링용 리스트
    public Queue<GameObject>[] opDeckBuildingsPool;
    //풀링할 건물 오브젝트 갯수
    [SerializeField] int poolingCount;
    //현재 지으려고 하는 건물
    Card nowToBuild;


    //현재 선택된 건설부지를 표시해주는 하이라이터의 위치
    public Transform currentCellIndicater;

    private void Start()
    {
        Debug.Log("Start");
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            buildingIndicators = new GameObject[Constants.BUILDING_PER_DECK];
            myDeckBuildingsPool = new Queue<GameObject>[Constants.BUILDING_PER_DECK];
            opDeckBuildingsPool = new Queue<GameObject>[Constants.BUILDING_PER_DECK];

            for (int i = 0; i < Constants.BUILDING_PER_DECK; i++)
            {
                myDeckBuildingsPool[i] = new Queue<GameObject>();
                opDeckBuildingsPool[i] = new Queue<GameObject>();
            }
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }


    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("BuildManager OnSceneLoaded");
        if (scene.name.Equals("Desert"))
        {
            Initialize();

        }
    }

    public void Initialize()
    {
        Debug.Log("BuildManager Initiallize");
        nowToBuild = new Card(BuildingType.NONE);
        tileMap = GameObject.Find("BuildTileMap" + CBattleRoom.instance.playerMeIndex).GetComponent<TileMap>();
        enemyTileMap = GameObject.Find("BuildTileMap" + CBattleRoom.instance.GetEnemyIndex()).GetComponent<TileMap>();
        tileMapTransform = tileMap.GetComponent<Transform>();
        enemyTileMapTransfrom = enemyTileMap.GetComponent<Transform>();

        for (int i = 0; i < Constants.BUILDING_PER_DECK; i++)
        {
            buildingIndicators[i] = Instantiate(DeckManager.instance.NowDeck[i].buildingPrefap);
            buildingIndicators[i].GetComponent<UnitGenerator>().enabled = false;
            buildingIndicators[i].name = "BuildingIndicator";
            buildingIndicators[i].SetActive(false);
        }
        PoolingBuildings(DeckManager.instance.opponentDeck, false);
        PoolingBuildings(DeckManager.instance.NowDeck, true);

    }
    public void Reset()
    {

        for (int i = 0; i < Constants.BUILDING_PER_DECK; i++)
        {
            Destroy(buildingIndicators[i]);
            //for (int j = 0; j < poolingCount; j++)
            //{
            //    Destroy(myDeckBuildingsPool[i].Dequeue());
            //    Destroy(opDeckBuildingsPool[i].Dequeue());
            //}
        }
        buildingIndicators = new GameObject[Constants.BUILDING_PER_DECK];
        myDeckBuildingsPool = new Queue<GameObject>[Constants.BUILDING_PER_DECK];
        opDeckBuildingsPool = new Queue<GameObject>[Constants.BUILDING_PER_DECK];

        for (int i = 0; i < Constants.BUILDING_PER_DECK; i++)
        {
            myDeckBuildingsPool[i] = new Queue<GameObject>();
            opDeckBuildingsPool[i] = new Queue<GameObject>();
        }
    }
    /// <summary>
    /// 건물들을 풀링함.
    /// </summary>
    /// <param name="deck">풀링할 덱</param>
    /// <param name="isMe">내 덱인지 상대방 덱인지</param>
    public void PoolingBuildings(Deck deck,bool isMe)
    {
        Debug.Log("PoolingBuildings isme?: " + isMe+" deck count: " + deck.GetCount());
        for (int i = 0; i < deck.GetCount(); i++)
        {
            for (int j=0; j < poolingCount; j++)
            {
                GameObject obj = Instantiate(deck[i].buildingPrefap);
                obj.SetActive(false);
                if (isMe)
                {
                    opDeckBuildingsPool[i].Enqueue(obj);
                }
                else
                {

                    myDeckBuildingsPool[i].Enqueue(obj);
                }
            }
        }
    }
    //게임 화면에서 건설하기 버튼을 누르면 돌아가는 코루틴. 한번 더누르면 종료된다.
    public IEnumerator BuldingCoroutine()
    {
        MeshRenderer indicatorRenderer = currentCellIndicater.GetComponentInChildren<MeshRenderer>();
        short buldingPosIndex = 0;
        while (true)
        {
            yield return null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (tileMap.GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity))
            {

                int x = Mathf.FloorToInt((hitInfo.point.x - tileMapTransform.position.x) / tileMap.tileSize);
                int z = Mathf.FloorToInt((hitInfo.point.z - tileMapTransform.position.z) / tileMap.tileSize);
                buldingPosIndex = (short)(z * tileMap.sizeZ + x);
                currentTileCoord.x = x;
                currentTileCoord.z = z;
                currentCellIndicater.position = currentTileCoord * tileMap.tileSize + tileMapTransform.position;

                if (nowToBuild.CardType.Equals(BuildingType.NONE))
                {
                    continue;       //건설 불가능: 선택된 건물 없음.
                }
                buildingIndicators[nowToBuild.index].transform.position = currentTileCoord * tileMap.tileSize + tileMapTransform.position;
                if (!CBattleRoom.instance.CheckBuildAreaEmpty(CBattleRoom.instance.playerMeIndex, buldingPosIndex))//건설 불가능: 해당위치에 이미 건물있음 빨간색으로 
                {
                    indicatorRenderer.material.color = new Color(1, 0, 0, 0.3f);
                    continue;

                }
                else if (CBattleRoom.instance.GetNowGold()< nowToBuild.cost)//건설 불가능: 골드가 모자람 빨간색으로 
                {
                    indicatorRenderer.material.color = new Color(1, 0, 0, 0.3f);
                    continue;
                }
                else//건설 가능 : 초록색
                {
                    indicatorRenderer.material.color = new Color(0, 1, 0, 0.3f);
                }

                //건설 가능상태에서 마우스 우클릭 시 서버에 건설 요청을 보내게 됨.
                if (Input.GetMouseButtonDown(1))
                {
                    BuildRequest(nowToBuild, buldingPosIndex);
                }
            }
            indicatorRenderer.material.color = Color.clear;//마우스가 건설지역 밖으로 나감.완전투명

        }
    }
    //서버에 건설 요청을 함.
    void BuildRequest(Card card , short index)
    {
        if (!DeckManager.instance.NowDeck.HasCard(card))
        {
            return;
        }
        int x = index % tileMap.sizeZ;
        int z = index / tileMap.sizeZ;

        CBattleRoom.instance.BuildRequest(card, index);
    }
/// <summary>
/// 건물을 건설함
/// </summary>
/// <param name="playerIndex">건설할 플레이어의 인덱스</param>
/// <param name="cardIndex">건설할 건물카드의 덱 내에서의 인덱스</param>
/// <param name="posIndex">건설할 지역의 위치 인덱스</param>
    public void Build(byte playerIndex, int cardIndex,short posIndex)
    {
        int x = posIndex % tileMap.sizeZ;
        int z = posIndex / tileMap.sizeZ;
        GameObject obj;
        if (playerIndex.Equals(CBattleRoom.instance.playerMeIndex))
        {

             obj= myDeckBuildingsPool[cardIndex].Dequeue();
        }
        else
        {
            obj = opDeckBuildingsPool[cardIndex].Dequeue();
        }

        Vector3 placeToBuild ;
        Transform parent;
        if (playerIndex.Equals(CBattleRoom.instance.playerMeIndex)) //우리 땅에 건설
        {
            placeToBuild = tileMapTransform.position;
            parent = tileMapTransform;
            CBattleRoom.instance.SpendGold(nowToBuild.cost);
        }
        else // 적 땅에 건설
        {
            placeToBuild = enemyTileMapTransfrom.position;
            parent = enemyTileMapTransfrom;
        }
        placeToBuild += new Vector3(x * tileMap.tileSize, 0, z * tileMap.tileSize);
        obj.transform.position = placeToBuild;
        Building building = obj.GetComponent<Building>();
        CBattleRoom.instance.AddBuilding(playerIndex, building,posIndex);
        obj.SetActive(true);
        if (CBattleRoom.instance.GameState.Equals(CBattleRoom.GAME_STATE.INITIAL_BUILD))
        {
            return;
        }
        building.MakeItWork();//건물이 건설된 후 하게될 일을 시작시킴.
    }
    //건설 화면에서 건설할 카드를 선택했을때 호출되는 함수. nowToBuild를 선택한 카드에 맞게 바꿔줌.
    public void BuildingButtonClick(Card card)
    {
        if (!nowToBuild.CardType.Equals(BuildingType.NONE) )//선택하기 이전에 카드가 이미 선택되어있던 경우
        {
            buildingIndicators[nowToBuild.index].SetActive(false);
        }

        buildingIndicators[card.index].SetActive(true);
        nowToBuild = card;

    }
}
