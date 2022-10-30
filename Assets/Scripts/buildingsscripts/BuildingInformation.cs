using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingInformation
{
    private static int castleCounter = 0;
    private static int houseCounter = 0;
    private static int barrackSpearCounter = 0;
    private static int barrackMaceCounter = 0;
    private static int barrackHeavySwordCounter = 0;
    private static int archeryTowerCounter = 0;
    private static int goldmineCounter = 0;

    private static int castleMax = 1;
    private static int houseMax = 10;
    private static int barrackSpearMax = 1;
    private static int barrackMaceMax = 1;
    private static int barrackHeavySwordMax = 1;
    private static int archeryTowerMax = 5;
    private static int goldmineMax = 3;

    public static void Reset()
    {
        castleCounter = 0;
        houseCounter = 0;
        barrackSpearCounter = 0;
        barrackMaceCounter = 0;
        barrackHeavySwordCounter = 0;
        archeryTowerCounter = 0;
    }

    public enum TYPE_OF_BUILDING
    {
        Castle, House, Barrack_Spear, Barrack_Mace, Barrack_HeavySword, ArcheryTower, Goldmine
    }

    static readonly int[] cost = new int[]
    {
        2000, 250, 800, 800, 800, 1000, 1500
    };

    static readonly int[] health = new int[]
    {
        3000, 500, 1500, 1500, 1500, 1000, 2000
    };

    // The scaling works better for now if the sizes are in odd numbers to make sure that there is always a tile in the center
    static readonly Vector2[] size = new Vector2[]
    {
        new Vector2(7,11), new Vector2(5,7), new Vector2(7,9), new Vector2(7,9), new Vector2(7,9), new Vector2(5,13), new Vector2(7,9)
    };

    public static int GetBuildingHealth(TYPE_OF_BUILDING type)
    {
        return health[(int)type];
    }

    public static int GetBuildingCost(TYPE_OF_BUILDING type)
    {
        return cost[(int)type];
    }

    public static Vector2 GetBuildingSize(TYPE_OF_BUILDING type)
    {
        return size[(int)type];
    }

    public static bool MaxLimitReached(TYPE_OF_BUILDING type)
    {
        bool limitReached = false;

        if (type is TYPE_OF_BUILDING.Castle)
        {
            if (castleCounter == castleMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.House)
        {
            if (houseCounter == houseMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.Barrack_Spear)
        {
            if (barrackSpearCounter == barrackSpearMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.Barrack_Mace)
        {
            if (barrackMaceCounter == barrackMaceMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.Barrack_HeavySword)
        {
            if (barrackHeavySwordCounter == barrackHeavySwordMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.ArcheryTower)
        {
            if (archeryTowerCounter == archeryTowerMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.Goldmine)
        {
            if (goldmineCounter == goldmineMax)
            {
                limitReached = true;
            }
        }

        return limitReached;
    }

    public static int GetCounter(TYPE_OF_BUILDING type)
    {
        int counter = -1;

        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                counter = castleCounter;
                break;
            case TYPE_OF_BUILDING.House:
                counter = houseCounter;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                counter = barrackSpearCounter;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                counter = barrackMaceCounter;
                break;
            case TYPE_OF_BUILDING.Barrack_HeavySword:
                counter = barrackHeavySwordCounter;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                counter = archeryTowerCounter;
                break;
            case TYPE_OF_BUILDING.Goldmine:
                counter = goldmineCounter;
                break;
            default:
                Debug.LogWarning("Warning! Could not return the " + type.ToString() + "Counter...");
                break;
        }

        return counter;
    }

    public static int GetMax(TYPE_OF_BUILDING type)
    {
        int max = -1;

        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                max = castleMax;
                break;
            case TYPE_OF_BUILDING.House:
                max = houseMax;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                max = barrackSpearMax;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                max = barrackMaceMax;
                break;
            case TYPE_OF_BUILDING.Barrack_HeavySword:
                max = barrackHeavySwordMax;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                max = archeryTowerMax;
                break;
            case TYPE_OF_BUILDING.Goldmine:
                max = goldmineMax;
                break;
            default:
                Debug.LogWarning("Warning! Could not return the " + type.ToString() + "Max...");
                break;
        }

        return max;
    }

    public static void IncreaseCounter(TYPE_OF_BUILDING type)
    {
        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                castleCounter++;
                break;
            case TYPE_OF_BUILDING.House:
                houseCounter++;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                barrackSpearCounter++;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                barrackMaceCounter++;
                break;
            case TYPE_OF_BUILDING.Barrack_HeavySword:
                barrackHeavySwordCounter++;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                archeryTowerCounter++;
                break;
            case TYPE_OF_BUILDING.Goldmine:
                goldmineCounter++;
                break;
            default:
                Debug.LogWarning("Warning! Could not increase the " + type.ToString() + "Counter...");
                break;
        }
    }

    public static void DecreaseCounter(TYPE_OF_BUILDING type)
    {
        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                castleCounter--;
                break;
            case TYPE_OF_BUILDING.House:
                houseCounter--;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                barrackSpearCounter--;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                barrackMaceCounter--;
                break;
            case TYPE_OF_BUILDING.Barrack_HeavySword:
                barrackHeavySwordCounter--;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                archeryTowerCounter--;
                break;
            case TYPE_OF_BUILDING.Goldmine:
                goldmineCounter--;
                break;
            default:
                Debug.LogWarning("Warning! Could not decrease the " + type.ToString() + "Counter...");
                break;
        }
    }
}
