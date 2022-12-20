using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UpgradeManager
{
    static BuildingManager buildMan;

    public enum TYPE_OF_UPGRADE
    {
        Building_Health, House, Barrack_Spear, Barrack_Mace, Barrack_HeavySword, ArcheryTower, Goldmine
    }

    static int Max_Building_Health_Level = 10;

    static int Building_Health_Level = 0;

    static readonly int[] cost = new int[]
    {
        100
    };

    public static void Init(BuildingManager inBuildMan)
    {
        buildMan = inBuildMan;
    }

    public static int GetUpgradeCost(TYPE_OF_UPGRADE type)
    {
        return cost[(int)type];
    }

    public static void IncreaseUpgradeLevel(TYPE_OF_UPGRADE type)
    {
        if (type is TYPE_OF_UPGRADE.Building_Health)
        {
            Building_Health_Level++;

            int extraHealth = 10;

            BuildingInformation.IncreaseBuildingHealth(extraHealth);

            foreach (Building building in buildMan.GetBuildings())
            {
                building.GetHealth().IncreaseMaxHealth(extraHealth);
            }
        }
    }

    public static bool MaxUpgradeLevelReached(TYPE_OF_UPGRADE type)
    {
        bool maxLevelReached = false;

        if (type is TYPE_OF_UPGRADE.Building_Health)
        {
            if (Building_Health_Level == Max_Building_Health_Level)
            {
                maxLevelReached = true;
            }
        }

        return maxLevelReached;
    }

    public static int GetLevel(TYPE_OF_UPGRADE type)
    {
        if (type is TYPE_OF_UPGRADE.Building_Health)
        {
            return Building_Health_Level;
        }

        return 0;
    }

    public static int GetMaxLevel(TYPE_OF_UPGRADE type)
    {
        if (type is TYPE_OF_UPGRADE.Building_Health)
        {
            return Max_Building_Health_Level;
        }

        return 0;
    }

    public static Sprite GetSprite(TYPE_OF_UPGRADE type)
    {
        if (type == TYPE_OF_UPGRADE.Building_Health)
        {
            return Resources.Load<Sprite>("Sprites/Upgrades/" + type.ToString());
        }

        return null;
    }
}
