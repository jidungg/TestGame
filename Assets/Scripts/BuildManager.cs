using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public class BuildManager : MonoBehaviour
{
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
    //덱에있는 건물들 풀링용 리스트
    GameObject[] myDeckBuildingsPool;
    //현재 지으려고 하는 건물
    Card nowToBuild;

    //현재 선택된 건설부지를 표시해주는 하이라이터의 위치
    public Transform currentCellIndicater;


    public void Initialize()
    {
        nowToBuild = new Card(BuildingType.NONE);
        tileMap = GameObject.Find("BuildTileMap" + CBattleRoom.instance.playerMeIndex).GetComponent<TileMap>();
        enemyTileMap = GameObject.Find("BuildTileMap" + CBattleRoom.instance.GetEnemyIndex()).GetComponent<TileMap>();
        tileMapTransform = tileMap.GetComponent<Transform>();
        enemyTileMapTransfrom = enemyTileMap.GetComponent<Transform>();
        Deck nowdeck = DeckManager.instance.NowDeck;

        //myDeckBuildingsPool 초기화
        myDeckBuildingsPool = new GameObject[Constants. BUILDING_PER_DECK];
        for (int i=0;i< nowdeck.GetCount(); i++)
        {
            myDeckBuildingsPool[i] = GameObject.Instantiate(nowdeck[i].buildingPrefap,this.transform);
            myDeckBuildingsPool[i].SetActive(false);
        }

    }
    //게임 화면에서 건설하기 버튼을 누르면 돌아가는 코루틴. 한번 더누르면 종료된다.
    public IEnumerator BuldingCoroutine()
    {
        MeshRenderer indicatorRenderer = currentCellIndicater.GetComponentInChildren<MeshRenderer>();
        short index = 0;
        while (true)
        {
            yield return null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (tileMap.GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity))
            {

                int x = Mathf.FloorToInt((hitInfo.point.x - tileMapTransform.position.x) / tileMap.tileSize);
                int z = Mathf.FloorToInt((hitInfo.point.z - tileMapTransform.position.z) / tileMap.tileSize);
                index = (short)(z * tileMap.sizeZ + x);
                currentTileCoord.x = x;
                currentTileCoord.z = z;
                currentCellIndicater.position = currentTileCoord * tileMap.tileSize + tileMapTransform.position;

                if (nowToBuild.CardType.Equals(BuildingType.NONE))
                {
                    continue;       //건설 불가능: 선택된 건물 없음.
                }
                myDeckBuildingsPool[nowToBuild.index].transform.position = currentTileCoord * tileMap.tileSize + tileMapTransform.position;
                if (!CBattleRoom.instance.CheckBuildAreaEmpty(CBattleRoom.instance.playerMeIndex, index))//건설 불가능: 해당위치에 이미 건물있음 빨간색으로 
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
                    BuildRequest(nowToBuild, index);
                }
            }
            indicatorRenderer.material.color = Color.clear;//마우스가 건설지역 밖으로 나감.완전투명

        }
    }
    //서버에 건설 요청을 함.
    void BuildRequest(Card card , short index)
    {
        int x = index % tileMap.sizeZ;
        int z = index / tileMap.sizeZ;
        Debug.Log("type: " + card.CardType + "position: " + index + "xz: " + x + z);

        CBattleRoom.instance.BuildRequest(card, index);
    }
    //해당 지역에 건물을 건설함.
    public void Build(byte playerIndex, Card card,short index)
    {
        int x = index % tileMap.sizeZ;
        int z = index / tileMap.sizeZ;
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
        GameObject obj = Instantiate(card.buildingPrefap, placeToBuild, Quaternion.Euler(0, 0, 0), parent);
        Building building = obj.GetComponent<Building>();
        CBattleRoom.instance.AddBuilding(playerIndex, building,index);

        building.MakeItWork();//건물이 건설된 후 하게될 일을 시작시킴.
    }
    //건설 화면에서 건설할 카드를 선택했을때 호출되는 함수. nowToBuild를 선택한 카드에 맞게 바꿔줌.
    public void BuildingButtonClick(Card card)
    {
        if (!nowToBuild.CardType.Equals(BuildingType.NONE) )//선택하기 이전에 카드가 이미 선택되어있던 경우
        {
            myDeckBuildingsPool[nowToBuild.index].SetActive(false);
        }

        myDeckBuildingsPool[card.index].SetActive(true);
        nowToBuild = card;

    }
}
