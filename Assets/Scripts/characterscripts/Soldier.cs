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
        float attackSpeed = CharacterInformation.GetSoldierAttackSpeed(sType);
        AnimationStartingPoints asp = CharacterInformation.GetSoldierAnimationStartingPoints(sType);
        switch (sType)
        {
            case CharacterInformation.TYPE_OF_SOLDIER.Spearman:
                boundingBoxOffset = new Vector2(0.0f, -0.5f);
                health.Init(go, "Sprites/SoldierHealth", hp, new Vector2(0.2f, 0.15f), -0.1f);
                sm.Init(go, "Sprites/Medieval Warrior Pack 2/SpritesSpear", asp, boundingBoxOffset, attackSpeed);
                break;
            case CharacterInformation.TYPE_OF_SOLDIER.Maceman:
                boundingBoxOffset = new Vector2(0.0f, -0.5f);
                health.Init(go, "Sprites/SoldierHealth", hp, new Vector2(0.2f, 0.15f), -0.1f);
                sm.Init(go, "Sprites/Medieval Warrior Pack 2/SpritesMace", asp, boundingBoxOffset, attackSpeed);
                break;
            case CharacterInformation.TYPE_OF_SOLDIER.HeavySwordman:
                boundingBoxOffset = new Vector2(0.0f, -0.5f);
                health.Init(go, "Sprites/SoldierHealth", hp, new Vector2(0.2f, 0.15f), -0.1f);
                sm.Init(go, "Sprites/Medieval Warrior Pack 2/SpritesHeavySword", asp, boundingBoxOffset, attackSpeed);
                break;
            case CharacterInformation.TYPE_OF_SOLDIER.Knight:
                boundingBoxOffset = new Vector2(0.0f, -0.3f);
                health.Init(go, "Sprites/SoldierHealth", hp, new Vector2(0.2f, 0.15f));
                sm.Init(go, "Sprites/Hero_Knight_2/Sprites", asp, boundingBoxOffset, attackSpeed);
                break;
            default:
                boundingBoxOffset = new Vector2(0.0f, 0.0f);
                health.Init(go, "Sprites/SoldierHealth", 0, new Vector2(0.0f, 0.0f), 0);
                Debug.LogError("No enemy type " + sType.ToString() + " was found!");
                break;
        }


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
                    //PlayWeaponSound(sType); // Gets too noisy
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
                sm.TakeDamage();
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
        else if (sType == CharacterInformation.TYPE_OF_SOLDIER.HeavySwordman)
        {
            weaponBox.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
            weaponBox.transform.localScale = new Vector2(GridManager.GetTileWidth() * 3.0f, GridManager.GetTileHeight() * 3.0f);
        }
        else if (sType == CharacterInformation.TYPE_OF_SOLDIER.Knight)
        {
            weaponBox.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
            weaponBox.transform.localScale = new Vector2(GridManager.GetTileWidth() * 4.0f, GridManager.GetTileHeight() * 4.0f);
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

    void PlayWeaponSound(CharacterInformation.TYPE_OF_SOLDIER type)
    {
        if (type == CharacterInformation.TYPE_OF_SOLDIER.Spearman)
        {
            int index = Random.Range(0, 3);

            switch (index)
            {
                case 0:
                    AudioManager.PlayWeaponsAudio3D("Spear Stab", 0.15f, go.transform.position);
                    break;
                case 1:
                    AudioManager.PlayWeaponsAudio3D("Spear Stab2", 0.15f, go.transform.position);
                    break;
                case 2:
                    AudioManager.PlayWeaponsAudio3D("Spear Stab3", 0.15f, go.transform.position);
                    break;
                default:
                    AudioManager.PlayWeaponsAudio3D("Spear Stab", 0.15f, go.transform.position);
                    break;
            }
        }
        else
        {
            int index = Random.Range(0, 4);

            switch (index)
            {
                case 0:
                    AudioManager.PlayWeaponsAudio3D("Sword Swing", 0.1f, go.transform.position);
                    break;
                case 1:
                    AudioManager.PlayWeaponsAudio3D("Sword Swing2", 0.1f, go.transform.position);
                    break;
                case 2:
                    AudioManager.PlayWeaponsAudio3D("Sword Swing3", 0.1f, go.transform.position);
                    break;
                case 3:
                    AudioManager.PlayWeaponsAudio3D("Sword Swing4", 0.1f, go.transform.position);
                    break;
                default:
                    AudioManager.PlayWeaponsAudio3D("Sword Swing", 0.1f, go.transform.position);
                    break;
            }
        }
    }
}
