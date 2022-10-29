using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterInformation
{
    public enum TYPE_OF_ENEMY
    {
        Eye, Mushroom, Goblin, Skeleton
    }
    public enum TYPE_OF_SOLDIER
    {
        Spearman, Maceman, HeavySwordman
    }

    static readonly Vector2[] soldierSpawnLocations = new Vector2[]
    {
        Vector2.zero, Vector2.zero, Vector2.zero
    };

    static readonly int[] enemyDamage = new int[]
    {
        20, 20, 60, 100
    };

    static readonly int[] enemyHealth = new int[]
    {
        50, 50, 100, 200
    };

    static readonly int[] enemyValue = new int[]
    {
        10, 10, 30, 50
    };

    static readonly float[] enemySpeed = new float[]
    {
        2.5f, 1.2f, 1.4f, 1.5f
    };

    static readonly int[] soldierDamage = new int[]
    {
        20, 40, 30
    };

    static readonly int[] soldierHealth = new int[]
    {
        100, 200, 125
    };

    static readonly float[] soldierAttackSpeed = new float[]
    {
        0.1f, 0.2f, 0.1f
    };

    static public void SetSpawnLocation(TYPE_OF_SOLDIER type, Vector2 spawn)
    {
        soldierSpawnLocations[(int)type] = spawn;
    }

    static public Vector2 GetSoldierSpawnLocation(TYPE_OF_SOLDIER type)
    {
        return soldierSpawnLocations[(int)type];
    }


    public static int GetEnemyDamage(TYPE_OF_ENEMY type)
    {
        return enemyDamage[(int)type];
    }

    public static int GetEnemyHealth(TYPE_OF_ENEMY type)
    {
        return enemyHealth[(int)type];
    }

    public static int GetEnemyValue(TYPE_OF_ENEMY type)
    {
        return enemyValue[(int)type];
    }

    public static float GetEnemySpeed(TYPE_OF_ENEMY type)
    {
        return enemySpeed[(int)type];
    }

    public static int GetSoldierDamage(TYPE_OF_SOLDIER type)
    {
        return soldierDamage[(int)type];
    }

    public static int GetSoldierHealth(TYPE_OF_SOLDIER type)
    {
        return soldierHealth[(int)type];
    }

    public static float GetSoldierAttackSpeed(TYPE_OF_SOLDIER type)
    {
        return soldierAttackSpeed[(int)type];
    }


    public static AnimationStartingPoints GetEnemyAnimationStartingPoints(TYPE_OF_ENEMY type)
    {
        AnimationStartingPoints asp;

        if (type == TYPE_OF_ENEMY.Mushroom || type == TYPE_OF_ENEMY.Goblin)
        {
            asp.idle = 20;
            asp.idleEnd = 23;
            asp.walk = 24;
            asp.walkEnd = 31;
            asp.attack = 0;
            asp.attackEnd = 7;
            asp.die = 17;
            asp.dieEnd = 20;
            asp.takeDamage = 32;
            asp.takeDamageEnd = 35;
        }
        else if (type == TYPE_OF_ENEMY.Eye)
        {
            asp.idle = 20;
            asp.idleEnd = 27;
            asp.walk = 20;
            asp.walkEnd = 27;
            asp.attack = 0;
            asp.attackEnd = 7;
            asp.die = 16;
            asp.dieEnd = 19;
            asp.takeDamage = 28;
            asp.takeDamageEnd = 31;
        }
        else if (type == TYPE_OF_ENEMY.Skeleton)
        {
            asp.idle = 20;
            asp.idleEnd = 23;
            asp.walk = 32;
            asp.walkEnd = 35;
            asp.attack = 0;
            asp.attackEnd = 7;
            asp.die = 16;
            asp.dieEnd = 19;
            asp.takeDamage = 28;
            asp.takeDamageEnd = 31;
        }
        else
        {
            asp.idle = 0;
            asp.idleEnd = 0;
            asp.walk = 0;
            asp.walkEnd = 0;
            asp.attack = 0;
            asp.attackEnd = 0;
            asp.die = 0;
            asp.dieEnd = 0;
            asp.takeDamage = 0;
            asp.takeDamageEnd = 0;

            Debug.LogWarning("No animations found for " + type.ToString());
        }

        return asp;
    }

    public static AnimationStartingPoints GetSoldierAnimationStartingPoints(TYPE_OF_SOLDIER type)
    {
        AnimationStartingPoints asp;

        if (type == TYPE_OF_SOLDIER.Spearman || type == TYPE_OF_SOLDIER.Maceman)
        {
            asp.idle = 12;
            asp.idleEnd = 19;
            asp.walk = 22;
            asp.walkEnd = 29;
            asp.attack = 0;
            asp.attackEnd = 3;
            asp.die = 4;
            asp.dieEnd = 9;
            asp.takeDamage = 30;
            asp.takeDamageEnd = 33;
        }
        else if (type == TYPE_OF_SOLDIER.HeavySwordman)
        {
            asp.idle = 13;
            asp.idleEnd = 20;
            asp.walk = 23;
            asp.walkEnd = 30;
            asp.attack = 0;
            asp.attackEnd = 4;
            asp.die = 4;
            asp.dieEnd = 10;
            asp.takeDamage = 31;
            asp.takeDamageEnd = 34;
        }
        else
        {
            asp.idle = 0;
            asp.idleEnd = 0;
            asp.walk = 0;
            asp.walkEnd = 0;
            asp.attack = 0;
            asp.attackEnd = 0;
            asp.die = 0;
            asp.dieEnd = 0;
            asp.takeDamage = 0;
            asp.takeDamageEnd = 0;

            Debug.LogWarning("No animations found for " + type.ToString());
        }

        return asp;
    }

}

public static class HumansCounter
{
    public static int counter = 0;
    public static int nrOfHumans = 0;
    public static int max = 0; // depends on number of houses

    public static void Reset()
    {
        counter = 0;
        nrOfHumans = 0;
    }
}

public static class EnemyCounter
{
    public static int counter = 0;
    public static int nrOfEnemies = 0;
    public static int max = 5000;

    public static void Reset()
    {
        counter = 0;
        nrOfEnemies = 0;
    }
}

public static class SoldierCounter_Spearmen
{
    public static int counter = 0;
    public static int nrOfSoldiers = 0;
    public static int nrToSpawn = 0;
    public static int max = 5000;

    public static void Reset()
    {
        counter = 0;
        nrOfSoldiers = 0;
        nrToSpawn = 0;
    }
}

public static class SoldierCounter_Macemen
{
    public static int counter = 0;
    public static int nrOfSoldiers = 0;
    public static int nrToSpawn = 0;
    public static int max = 5000;

    public static void Reset()
    {
        counter = 0;
        nrOfSoldiers = 0;
        nrToSpawn = 0;
    }
}

public static class SoldierCounter_HeavySwordmen
{
    public static int counter = 0;
    public static int nrOfSoldiers = 0;
    public static int nrToSpawn = 0;
    public static int max = 5000;

    public static void Reset()
    {
        counter = 0;
        nrOfSoldiers = 0;
        nrToSpawn = 0;
    }
}
