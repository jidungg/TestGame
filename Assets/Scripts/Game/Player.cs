using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public enum PLAYER_STATE
{
    HUMAN,
    AI
}
public class Player : MonoBehaviour
{
    private Transform masterTargetPoint;
    private int gold;

    public Dictionary<short, Building> buildings;
    public byte playerIndex { get; private set; }
    public PLAYER_STATE state { get; private set; }
    public Vector3 genPosition;
    public int Gold
    {
        get {
            return gold;
        }
    }

    public void initialize(byte player_index)
    {
        this.playerIndex = player_index;
        if (player_index.Equals(0))
        {
            genPosition = GameObject.FindGameObjectWithTag("GenPosition0").GetComponent<Transform>().position;
            masterTargetPoint = GameObject.FindGameObjectWithTag("MasterTargetPoint0").GetComponent<Transform>();
        }
        else
        {
            genPosition = GameObject.FindGameObjectWithTag("GenPosition1").GetComponent<Transform>().position;
            masterTargetPoint = GameObject.FindGameObjectWithTag("MasterTargetPoint1").GetComponent<Transform>();
        }
        buildings = new Dictionary<short, Building>();
    }
    public int GainGold(int amount)
    {
        gold += amount;
        return gold;
    }
    public int SpendGold(int amount)
    {
        gold -= amount;
        return gold;
    }
    public void AddBuilding(Building building,short index)
    {
        building.Initiallize(playerIndex,index, masterTargetPoint.position,genPosition);
        buildings[index] = building;
    }

    public void BattleStart()
    {
        foreach(var pair in buildings)
        {
            pair.Value.MakeItWork();
        }
    }

    private void MoveAllUnits()
    {

    }
    public void MoveMasterTargetPoint(Vector3 point)
    {
        masterTargetPoint.position = point;

    }


}
