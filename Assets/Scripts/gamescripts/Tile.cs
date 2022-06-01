using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Entities;

public class Tile
{
    GameObject go;
    SpriteRenderer sr;
    Vector2 tilePosition;
    Vector2 tileWorldPos;
    Vector2 tileSize;

    Tile parent;

    string tileName;

    uint nrOfEnemiesOnTile, nrOfSoldiersOnTile;

    bool enemyOnTile, soldierOnTile, playerOnTile, objectOnTile, buildPermission;
    float value;

    public float f, g, h, currCost;

    public Tile(GameObject grid, Vector2 inTilePosition)
    {
        tilePosition = inTilePosition;

        tileName = "Tile[" + tilePosition.x + ", " + tilePosition.y + "]";

        if (Tools.DebugMode)
        {
            go = new GameObject { name = tileName };
            go.transform.parent = grid.transform;

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

        value = 999999;
        f = g = h = 0;

        parent = null;
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

    public bool IsCharacterPresent(Character.TYPE_OF_CHARACTER type)
    {
        if (type == Character.TYPE_OF_CHARACTER.Enemy)
        {
            return enemyOnTile;
        }
        else if (type == Character.TYPE_OF_CHARACTER.Soldier)
        {
            return soldierOnTile;
        }
        else if (type == Character.TYPE_OF_CHARACTER.Player)
        {
            return playerOnTile;
        }

        Debug.LogWarning("Could not find type " + type.ToString() + ". Can not tell if " + go.name + " is occupied!");
        return true;
    }

    public bool IsPlaceable()
    {
        bool placeable = true;

        if (!BuildPermission() || IsObjectPresent() ||
            IsCharacterPresent(Character.TYPE_OF_CHARACTER.Player) ||
            IsCharacterPresent(Character.TYPE_OF_CHARACTER.Enemy) ||
            IsCharacterPresent(Character.TYPE_OF_CHARACTER.Soldier))
        {
            placeable = false;
        }

        return placeable;
    }

    public void SetSize(Vector2 newSize)
    {
        if (Tools.DebugMode)
        {
            go.transform.localScale = newSize;
        }

        tileSize = newSize;
    }

    public Vector2 GetSize()
    {
        return tileSize;
    }

    public void SetWorldPos(Vector2 newPos)
    {
        if (Tools.DebugMode)
        {
            go.transform.position = newPos;
        }

        tileWorldPos = newPos;
    }

    public Vector2 GetWorldPos()
    {
        return tileWorldPos;
    }

    public void SetValue(float inValue)
    {
        f = inValue;
    }

    public float GetValue()
    {
        return f;
    }

    public Vector2 GetTilePosition()
    {
        return tilePosition;
    }

    public string GetName()
    {
        return tileName;
    }

    public void SetParent(Tile tile)
    {
        parent = tile;
    }

    public Tile GetParent()
    {
        return parent;
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(go);
    }
}
