using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitContorl : MonoBehaviour
{
    public List<Building> allyBuildings;

    [SerializeField] private Transform masterTargetPoint;
    private void Awake()
    {
        if (masterTargetPoint.Equals(null))
        {
            masterTargetPoint = GameObject.FindGameObjectWithTag("MasterTargetPoint").GetComponent<Transform>();
        }
    }

    void Start()
    {
        GetAllyBuildingList();  
    }

    public void GetAllyBuildingList()
    {
        GameObject[] buildingArray = GameObject.FindGameObjectsWithTag("AllyBuilding");
        for (int i = 0; i < buildingArray.Length; i++)
        {
            allyBuildings.Add(buildingArray[i].GetComponent<Building>());
        }
    }
    private void MoveAllUnits()
    {
        for (int i = 0; i < allyBuildings.Count; i++)
        {
            allyBuildings[i].MoveUnits();
        }
    }

    public void MoveMasterTargetPoint(Vector3 point)
    {
        masterTargetPoint.position = point;
        for(int i= 0; i<allyBuildings.Count; i++)
        {
            allyBuildings[i].MoveTargetPoint(masterTargetPoint.position);
        }
    }
}
