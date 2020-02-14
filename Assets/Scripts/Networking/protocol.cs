using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandWarGameServer
{
    public struct Constants
    {
        public const int MAX_DECK_CAPACITY = 1;//저장할 수 있는 총 덱 수
        public const int BUILDING_PER_DECK = 1;//덱 하나당 포함가능한 건물 종류(카드) 수
        public static int MAX_UNIT_PER_BUILDING(BuildingType type)//건물 당 최대 소환 가능한 유닛 수
        {
            switch (type)
            {
                case BuildingType.NONE:
                    return -1;

                case BuildingType.TOMB_STONE:
                    return 5;

            }
            return -1;
        }
    }

    public enum PROTOCOL : short
    {
        BEGIN = 0,

        // 로딩을 시작해라. 패킷에 플레이어 인덱스를 동봉한다.
        START_LOADING = 1,

        LOADING_COMPLETED = 2,

        // 상대방 플레이어가 나가 방이 삭제되었다.
        ROOM_REMOVED = 3,

        // 게임방 입장 요청.
        ENTER_GAME_ROOM_REQ = 4,

        //게임 시작
        GAME_START = 5,

        //건설 요청
        BUILD_REQUEST = 6,

        //건설 성공
        BUILD_SUCCESS = 7,

        //건설 실패
        BUILD_FAILED = 8,



        END
    }


    public enum MAPS : short
    {
        DESERT
    }
    public enum BuildingType : short
    {
        NONE = 0,

        TOMB_STONE = 1,
    }
    public enum UnitType : short
    {
        ZOMBIE = 0,
    }

}