using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public class UnitGenerator : MonoBehaviour
{
    List<Unit> units = new List<Unit>();
    [SerializeField] List<GameObject> unitPrefpas;
    [SerializeField] Building generationBuilding;
    [SerializeField] int pullingAmount;
    [SerializeField] int maxUnitNum;
    [SerializeField] float genTerm;
    [SerializeField] public Transform genPoint;


    public void Initiallize(float genTerm,Vector3 genPos)
    {
        if (generationBuilding.Equals(null))
        {
            generationBuilding = this.gameObject.GetComponent<Building>();
        }
        if (pullingAmount.Equals(0))
        {
            pullingAmount = 10;
        }
        this.genTerm = genTerm;
        genPoint.position = genPos;
    }

    public void StartGenerate()
    {
        Debug.Log("start generate");
        StartCoroutine(GenerateCoroutine());
    }
    bool CheckGenCondition()
    {
        if (!generationBuilding.nowWorking)
        {
            return false;
        }
        if(units.Count >= maxUnitNum)
        {
            return false;
        }
        return true;
    }
    IEnumerator GenerateCoroutine()
    {
        while (true)
        {
            if(CheckGenCondition())
            {
                CBattleRoom.instance.UnitGenRequest(unitPrefpas[generationBuilding.level].GetComponent<Unit>().type, generationBuilding.buildingindex);
            }

            yield return new WaitForSeconds(genTerm);
        }

    }

    public void GenerateUnit(UnitType unit)
    {
        GameObject obj = Instantiate(unitPrefpas[generationBuilding.level], this.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
        Unit newUnit = obj.GetComponent<Unit>();
        units.Add(newUnit);
        newUnit.Initiallize(generationBuilding.playerIndex,(byte)(units.Count-1),newUnit.type, genPoint, generationBuilding.targetPoint);
        newUnit.ResetUnit();
        obj.transform.parent = this.transform;
        newUnit.AfterBirth();
    }

    
    public void MoveUnits()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].StartMoving();
        }
    }

    public void MoveTargetPoint(Vector3 point)
    {
        generationBuilding.targetPoint.position = point;
        MoveUnits();
    }

    public void ChangeUnitPrefap()
    {

    }
}
