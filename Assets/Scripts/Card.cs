using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public class Card
{
    //건설에 필요한 코스트
    public int cost;
    //건물의 타입
    public BuildingType CardType { get; set; }
    //실제 건물 프리팹
    public GameObject buildingPrefap;
    //덱에서의 인덱스(순서)
    public int index;

    public Card(BuildingType type)
    {
        CardType = type;

        switch (CardType)
        {
            case BuildingType.NONE:
                buildingPrefap = null;
                cost = 0;
                break;
            case BuildingType.TOMB_STONE:
                buildingPrefap= DeckManager.instance.buildingPrefapList[(int)type];
                cost = 10;
                break;
        }
    }
}
