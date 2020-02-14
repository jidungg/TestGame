using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomb : Building
{
    public override void OnIitiallize(byte playerIndex,short index)
    {

    }

    public override void MakeItWork()
    {
        nowWorking = true;
        unitGenerator.StartGenerate();
    }



    public override void Upgrade()
    {
        level += 1;
        unitGenerator.ChangeUnitPrefap();
    }
}
