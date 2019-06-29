using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private List<Unit> BuildingIncludedUnits = new List<Unit>();
    [SerializeField] public int maxIncludedUnit;
    [SerializeField] public Transform targetPoint;
    [SerializeField] public Transform genPoint;

    // Start is called before the first frame update
    private void Awake()
    {
        targetPoint.position = GameObject.FindGameObjectWithTag("MasterTargetPointer").transform.position;
    }
    void Start()
    {
        Debug.Log("building start");

    }

    public void MoveUnits()
    {
        Unit unit;
        for (int i = 0; i < BuildingIncludedUnits.Count; i++)
        {
            unit = BuildingIncludedUnits[i];

            if (unit.isMoving == false && unit.gameObject.activeSelf)
            {
                unit.StartCoroutine(unit.Moving(targetPoint));
            }
        }
    }

    public void MoveTargetPoint(Vector3 point)
    {
        targetPoint.position = point;
        MoveUnits();
    }

    public int NumEnabledUnits()
    {
        int num=0;
        for(int i=0; i<BuildingIncludedUnits.Count; i++)
        {
            if (BuildingIncludedUnits[i].gameObject.activeSelf)
            {
                num++;
            }
        }
        return num;
    }

    public int NumAllUnits()
    {
        return BuildingIncludedUnits.Count;
    }

    public void AddUnitInList(GameObject obj)
    {
        BuildingIncludedUnits.Add(obj.GetComponent<Unit>());
    }
    public bool EnableAUnit()
    {
        Debug.Log("EnableAUnit(building) count "+ BuildingIncludedUnits.Count);
        GameObject obj;
        for (int i = 0; i < BuildingIncludedUnits.Count; i++)
        {
            Debug.Log("EnableAUnit(building) routine " + i);
            obj = BuildingIncludedUnits[i].gameObject;
            if (obj.activeSelf.Equals(false))
            {
                obj.SetActive(true);
                return true ;
            }
        }
        return false;
    }
}
