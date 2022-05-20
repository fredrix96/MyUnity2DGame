using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile
{
    GameObject go;
    SpriteRenderer sr;
    Vector2 tilePosition;

    uint nrOfEnemiesOnTile, nrOfSoldiersOnTile;

    bool enemyOnTile, soldierOnTile, playerOnTile, objectOnTile, buildPermission;
    float value;

    public Tile(GameObject grid, Vector2 inTilePosition)
    {
        tilePosition = inTilePosition;

        go = new GameObject {  name = "Tile[" + tilePosition.x + ", " + tilePosition.y + "]" };
        go.transform.parent = grid.transform;

        if (Tools.DebugMode)
        {
            sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Sprites/Tile");
            sr.color = Color.white;
        }

        enemyOnTile = false;
        soldierOnTile = false;
        playerOnTile = false;
        objectOnTile = false;
        buildPermission = true;
        nrOfEnemiesOnTile = 0;
        nrOfSoldiersOnTile = 0;

        value = 0;
    }

    public uint GetNrOfCharactersOnTile(Character type)
    {
        if (type is Enemy)
        {
            return nrOfEnemiesOnTile;
        }
        else if (type is Soldier)
        {
            return nrOfSoldiersOnTile;
        }

        Debug.LogWarning("Could not find type " + type.GetName() + ". No number of characters was returned!");
        return 0;
    }

    public void IncreaseCharacters(Character type)
    {
        if (type is Enemy)
        {
            enemyOnTile = true;
            nrOfEnemiesOnTile++;
        }
        else if (type is Soldier)
        {
            soldierOnTile = true;
            nrOfSoldiersOnTile++;
        }
        else
        {
            Debug.LogWarning("Could not find type " + type.GetName() + ". No character was increased!");
        }
    }

    public void DecreaseCharacters(Character type)
    {
        if (type is Enemy)
        {
            // A tile can not have a negative number of enemies
            if (nrOfEnemiesOnTile != 0)
            {
                nrOfEnemiesOnTile--;
            }

            if (nrOfEnemiesOnTile == 0)
            {
                enemyOnTile = false;
            }
        }
        else if (type is Soldier)
        {
            if (nrOfSoldiersOnTile != 0)
            {
                nrOfSoldiersOnTile--;
            }

            if (nrOfSoldiersOnTile == 0)
            {
                soldierOnTile = false;
            }
        }
        else
        {
            Debug.LogWarning("Could not find type " + type.GetName() + ". No character was decreased!");
        }
    }

    public void PlayerOnTile(bool onTile)
    {
        playerOnTile = onTile;

        if (Tools.DebugMode && playerOnTile)
        {
            sr.color = Color.black;
        }
        else if (Tools.DebugMode && !playerOnTile && !objectOnTile && buildPermission)
        {
            sr.color = Color.white;
        }
    }

    public void ObjectOnTile(bool onTile)
    {
        objectOnTile = onTile;

        if (Tools.DebugMode && objectOnTile)
        {
            sr.color = Color.black;
        }
        else if (Tools.DebugMode && !objectOnTile)
        {
            sr.color = Color.white;
        }
    }

    public void SetPermissionToBuild(bool permission)
    {
        buildPermission = permission;

        if (Tools.DebugMode && buildPermission)
        {
            sr.color = Color.white;
        }
        else if (Tools.DebugMode && !buildPermission)
        {
            sr.color = Color.black;
        }
    }

    public bool IsObjectPresent()
    {
        return objectOnTile;
    }

    public bool BuildPermission()
    {
        return buildPermission;
    }

    public bool IsCharacterPresent(Type type)
    {
        if (type == typeof(Enemy))
        {
            return enemyOnTile;
        }
        else if (type == typeof(Soldier))
        {
            return soldierOnTile;
        }
        else if (type == typeof(Player))
        {
            return playerOnTile;
        }

        Debug.LogWarning("Could not find type " + type.Name + ". Can not tell if " + go.name + " is occupied!");
        return true;
    }

    public bool IsPlaceable()
    {
        bool placeable = true;

        if (!BuildPermission() || IsObjectPresent() || IsCharacterPresent(typeof(Player)) || IsCharacterPresent(typeof(Enemy)) || IsCharacterPresent(typeof(Soldier)))
        {
            placeable = false;
        }

        return placeable;
    }

    public void SetSize(Vector2 newSize)
    {
        go.transform.localScale = newSize;
    }

    public Vector2 GetSize()
    {
        return go.transform.localScale;
    }

    public void SetPos(Vector2 newPos)
    {
        go.transform.position = newPos;
    }

    public Vector2 GetPos()
    {
        return go.transform.position;
    }

    public void SetValue(float inValue)
    {
        value = inValue;
    }

    public float GetValue()
    {
        return value;
    }

    public Vector2 GetTilePosition()
    {
        return tilePosition;
    }

    public string GetName()
    {
        return go.name;
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(go);
    }
}
