using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public abstract class Building : MonoBehaviour
{
    //플레이어 인덱스
    public byte playerIndex;
    //건물의 위치 인덱스
    public short buildingindex;
    //건물이 목표로하는 지점
    public Transform targetPoint;
    //현재 작동중인지 나타냄.
    public bool nowWorking;
    //유닛 소환기
    public UnitGenerator unitGenerator;
    //건물 레벨
    public int level;
    //건물 최대 레벨
    public int maxLevel;


    private void Awake()
    {
    }
    /// <summary>
    /// 건물 종류별 초기화, 다른 세부 초기화에서 호출됨.
    /// </summary>
    public abstract void OnIitiallize(byte playerIndex,short buildingIndex);

    /// <summary>
    /// 건물에 유닛 소환기능이 없을경우 사용.
    /// </summary>
    /// <param name="index">건물의 인덱스(위치)</param>
    /// <param name="targetPos">건물의 대상 지점</param>
    public void Initiallize(byte playerIndex, short buildingIndex, Vector3 targetPos)
    {
        OnIitiallize(playerIndex,buildingIndex);
        nowWorking = false;
        this.playerIndex = playerIndex;
        this.buildingindex = buildingIndex;
        this.targetPoint.position = targetPos;
        level = 1;
        maxLevel = 3;
    }

    /// <summary>
    /// 건물에 유닛 소환기능이 있을경우 사용, unitGenerator 반드시 초기화 할것.
    /// </summary>
    /// <param name="index">건물의 인덱스(위치)</param>
    /// <param name="targetPos">건물의 대상 지점</param>
    /// <param name="genPos"> 유닛이 소환될 지점 위치</param>
    public void Initiallize(byte playerIndex, short buildingIndex, Vector3 targetPos, Vector3 genPos)
    {
        OnIitiallize(playerIndex,buildingIndex);
        nowWorking = false;
        this.playerIndex = playerIndex;
        this.buildingindex = buildingIndex;
        this.targetPoint.position = targetPos;
        unitGenerator = gameObject.GetComponent<UnitGenerator>();
        unitGenerator.Initiallize(3, this.transform.position);
        level = 1;
        maxLevel = 3;
        Debug.Log("Building Initialized");
    }

    //건설이 끝나고 건물이 해야할 일.
    public abstract void MakeItWork();

    //건물 업그레이드
    public abstract void Upgrade();

}


 
