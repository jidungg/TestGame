using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefap;
    [SerializeField] Building generationBuilding;
    [SerializeField] int pullingAmount;

    // Start is called before the first frame update
    private void Awake()
    {
        if (generationBuilding.Equals(null))
        {
            generationBuilding = this.gameObject.GetComponent<Building>();
        }
        if (pullingAmount.Equals(0))
        {
            pullingAmount = 10;
        }
    }
    void Start()
    {
        Debug.Log("unitGenrator start");
        StartCoroutine(StartGenerating());
    }
    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("Level" + level);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(unitPrefap, this.transform.position+new Vector3(0,0,1), Quaternion.identity);
        }
    }

    IEnumerator StartGenerating()
    {
        Debug.Log("startgenerating coroutine");
        while (true)
        {
            if(generationBuilding.NumEnabledUnits() < generationBuilding.maxIncludedUnit)
            {
                GenerateUnit();
            }

            yield return new WaitForSeconds(1);
        }

    }

    void GenerateUnit()
    {
        Debug.Log("generate unit");
        if ((generationBuilding.NumAllUnits() <= 0) || (generationBuilding.NumEnabledUnits() >= generationBuilding.NumAllUnits()))
        {
            PullingNew(pullingAmount);
            EnableAUnitInPull();
            return;

        }
        if (generationBuilding.NumAllUnits() >= 0 && generationBuilding.NumEnabledUnits() < generationBuilding.NumAllUnits())
        {
            EnableAUnitInPull();


            return;
        }

    }
    void EnableAUnitInPull()
    {
        Debug.Log("enableAunitInPull");
        Debug.Log(generationBuilding.EnableAUnit()) ;
        Debug.Log("numallunits:" + generationBuilding.NumAllUnits() + " numenalbledunits:" + generationBuilding.NumEnabledUnits());
    }

    void PullingNew(int num)
    {
        Debug.Log("PullingNew");
        if (generationBuilding.NumAllUnits() >= generationBuilding.maxIncludedUnit)
        {
            return;
        }
        Transform parent = generationBuilding.transform;
        for (int i = 0; i < num; i++)
        {
            Debug.Log("PullingNew routine " + i);
            GameObject obj = Instantiate(unitPrefap, this.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
            Unit unit = obj.GetComponent<Unit>();
            generationBuilding.AddUnitInList(obj);
            unit.genPoint = generationBuilding.genPoint;
            unit.targetPoint = generationBuilding.targetPoint;
            obj.transform.position = generationBuilding.genPoint.position;
            obj.SetActive(false);
            obj.transform.parent =parent;
            if (generationBuilding.gameObject.CompareTag("AllyBuilding"))
            {
                obj.tag = "AllyUnit";
            }
            else
            {
                obj.tag = "EnemyUnit";
            }
            Debug.Log("obj id: "+obj.GetInstanceID());
        }
    }
}
