using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public class Map
{
    short sizeX;
    short sizeZ;

    public MAPS Type
    {
        get;
    }
    public short SizeX
    {
        get { return sizeX; }
    }
    public short SizeZ
    {
        get { return sizeZ; }
    }

    public Map(MAPS map)
    {
        this.Type = map;
        Initialize();
    }

    void Initialize()
    {
        switch (Type)
        {
            case MAPS.DESERT:
                sizeX = 11;
                sizeZ = 11;
                break;
        }
    }
}
