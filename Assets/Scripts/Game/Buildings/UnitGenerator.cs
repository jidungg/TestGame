using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public class UnitGenerator : MonoBehaviour
{
    List<Unit> units = new List<Unit>();
    [SerializeField] List<GameObject> unitPrefpas;
    [SerializeField] Building generationBuilding;
    [SerializeField] int maxUnitCount;
    [SerializeField] float genTerm;
    [SerializeField] public Transform genPoint;

    /// <summary>
    /// 건물이 가진 유닛들 풀, 리스트의 인덱스에는 건물의 레벨을 대입하면 된다.
    /// </summary>
    List<Queue<Unit>> unitPool;


    public void Initiallize(float genTerm,Vector3 genPos)
    {
        if (generationBuilding.Equals(null))
        {
            generationBuilding = this.gameObject.GetComponent<Building>();
        }
        this.genTerm = genTerm;
        genPoint.position = genPos;
        unitPool = new List<Queue<Unit>>();
        PoolingUnits();
    }

    public void StartGenerate()
    {
        StartCoroutine(GenerateCoroutine());
    }
    bool CheckGenCondition()
    {
        if (!generationBuilding.nowWorking)
        {
            return false;
        }
        if(units.Count >= maxUnitCount)
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
                //CBattleRoom.instance.UnitGenRequest(unitPrefpas[generationBuilding.level].GetComponent<Unit>().type, generationBuilding.buildingindex);
                GenerateUnit(generationBuilding.level);
            }

            yield return new WaitForSeconds(genTerm);
        }

    }

    public void GenerateUnit(int level)
    {
        Unit newUnit = unitPool[level-1].Dequeue();
        units.Add(newUnit);
        newUnit.gameObject.SetActive(true);
        newUnit.Initiallize(generationBuilding,(byte)(units.Count-1),newUnit.type, genPoint, generationBuilding.targetPoint);
        newUnit.ResetUnit();
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

    void PoolingUnits()
    {

        foreach(var unit in unitPrefpas)
        {
            Queue<Unit> list = new Queue<Unit>();
            for (int i = 0; i < maxUnitCount; i++)
            {
                GameObject obj = Instantiate(unit, this.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
                obj.transform.parent = this.transform;
                Unit newUnit = obj.GetComponent<Unit>();
                list.Enqueue(newUnit);
                obj.SetActive(false);
            }

            unitPool.Add(list);
        }                 
    }
    public void ReturnUnitToPool(Unit unit)
    {
        unitPool[unit.level-1].Enqueue(unit);
    }
}
