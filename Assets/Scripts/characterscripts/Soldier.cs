using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Character
{
    CharacterInformation.TYPE_OF_SOLDIER sType;

    public Soldier(GameObject inGo, CharacterInformation.TYPE_OF_SOLDIER inSType)
    {
        type = TYPE_OF_CHARACTER.Soldier;
        sType = inSType;

        go = new GameObject { name = "soldier" + SoldierCounter_Spearmen.counter };
        go.transform.SetParent(inGo.transform);
        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        go.layer = LayerMask.NameToLayer("Soldiers");

        cm = go.AddComponent<CollisionManager>();

        boundingBoxOffset = new Vector2(0.0f, -0.25f);

        sm = go.AddComponent<SpriteManager>();

        health = go.AddComponent<Health>();

        int hp = CharacterInformation.GetSoldierHealth(sType);
        AnimationStartingPoints asp = CharacterInformation.GetSoldierAnimationStartingPoints(sType);
        switch (sType)
        {
            case CharacterInformation.TYPE_OF_SOLDIER.Spearman:
                boundingBoxOffset = new Vector2(0.0f, -0.5f);
                health.Init(go, "Sprites/SoldierHealth", hp, new Vector2(0.2f, 0.15f));
                attackSpeed = 0.1f;
                sm.Init(go, "Sprites/Medieval Warrior Pack 2/SpritesSpear", asp, boundingBoxOffset, attackSpeed);
                break;
            case CharacterInformation.TYPE_OF_SOLDIER.Maceman:
                boundingBoxOffset = new Vector2(0.0f, -0.5f);
                health.Init(go, "Sprites/SoldierHealth", hp, new Vector2(0.2f, 0.15f));
                attackSpeed = 0.2f;
                sm.Init(go, "Sprites/Medieval Warrior Pack 2/SpritesMace", asp, boundingBoxOffset, attackSpeed);
                break;
            default:
                boundingBoxOffset = new Vector2(0.0f, 0.0f);
                health.Init(go, "Sprites/SoldierHealth", 0, new Vector2(0.0f, 0.0f), 0);
                Debug.LogError("No enemy type " + sType.ToString() + " was found!");
                break;
        }


        //float randomY = Random.Range(0, GridManager.GetRes().y - 1);
        Vector2 spawn = CharacterInformation.GetSoldierSpawnLocation(sType);
        go.transform.position = GridManager.GetTile(spawn).GetWorldPos();

        // This is to make sure that feet of the character wont walk on another sprite
        pivotHeightDiff = Mathf.Abs(go.transform.position.y - sm.GetColliderPivotPoint(go).y);
        go.transform.position = sm.GetColliderPivotPoint(go);
        ph.position = new Vector2(go.transform.position.x, go.transform.position.y);
        lastXPos = ph.position.x;

        speed = 2.0f;
        direction = 1;
        damage = CharacterInformation.GetSoldierDamage(CharacterInformation.TYPE_OF_SOLDIER.Spearman);

        currTile = GridManager.GetTile(spawn);
        currTile.IncreaseCharacters(this);
        GridManager.GetCharacterTiles(type).Add(currTile);

        ph.type = type;
        ph.isIdle = false;

        UpdatePositionHandler();
    }

    public override void Update()
    {
        if (!isDead)
        {
            if (sm.IsWalking())
            {
                sm.Walk();

                UpdatePositionHandler();

                WalkToNewPosition();

                MarkTile(type);

                CheckDirection(type);

            }
            else if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    PlaySwordSound();
                    Damage();
                }
            }
            else if (sm.IsIdle())
            {
                UpdatePositionHandler();

                sm.Idle();
            }
            else if (sm.IsTakingDamage())
            {
                if (sm.TakeDamage())
                {
                    // SOUND
                }
            }

            if (health.GetHealth() <= 0)
            {
                isDead = true;
                currTile.DecreaseCharacters(this);
                GridManager.GetCharacterTiles(type).Remove(currTile);
            }
        }
        else
        {
            if (sm.Die() > 3)
            {
                shouldBeRemoved = true;
            }
        }
    }

    void Damage()
    {
        GameObject weaponBox = new GameObject { name = "WeaponBox" };

        float range = GridManager.GetTileWidth();
        if (sm.IsFlipped())
        {
            range *= -1;
        }

        if (sType == CharacterInformation.TYPE_OF_SOLDIER.Spearman)
        {
            weaponBox.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x + range, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
            weaponBox.transform.localScale = new Vector2(GridManager.GetTileWidth() * 4.0f, GridManager.GetTileHeight() * 1.0f);
        }
        else if (sType == CharacterInformation.TYPE_OF_SOLDIER.Maceman)
        {
            weaponBox.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x + range, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
            weaponBox.transform.localScale = new Vector2(GridManager.GetTileWidth() * 2.0f, GridManager.GetTileHeight() * 2.0f);
        }

        BoxCollider2D weaponBc = weaponBox.AddComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(sm.GetBoxCollider2D(), weaponBc);

        List<Collider2D> results = sm.GetListOfOverlapColliders(LayerMask.GetMask("Enemies"), weaponBc);

        foreach (Collider2D col in results)
        {
            if (col.gameObject.GetComponent<Health>() != null)
            {
                col.gameObject.GetComponent<Health>().Damage(damage, false, false);

                // Turn towards the target
                if (go.transform.position.x > col.transform.position.x && direction == 1)
                {
                    direction = -1;
                    sm.FlipX();
                }
                else if (go.transform.position.x < col.transform.position.x && direction == -1)
                {
                    direction = 1;
                    sm.FlipX();
                }

                // Only hit one character per attack
                //break;
            }
        }

        // Are there any enemies left around the character?
        if (results.Count == 0)
        {
            // Else, resume walking
            sm.StartWalking();

            // Turn if necessary
            if (direction == -1)
            {
                direction = 1;
                sm.FlipX();
            }
        }

        Object.Destroy(weaponBox);
    }

    public CharacterInformation.TYPE_OF_SOLDIER GetSoldierType()
    {
        return sType;
    }

    public bool Remove()
    {
        return shouldBeRemoved;
    }

    public override void Destroy()
    {
        Object.Destroy(go);
    }
}
