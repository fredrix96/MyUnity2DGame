using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimationStartingPoints
{
    public int idle;
    public int idleEnd;
    public int attack;
    public int attackEnd;
    public int walk;
    public int walkEnd;
    public int die;
    public int dieEnd;
}

public class SpriteManager : MonoBehaviour
{
    SpriteRenderer sr;
    Sprite[] sprites;
    BoxCollider2D bc;
    Rigidbody2D rb;
    AnimationStartingPoints asp;

    double animationTimer, deadTimer;

    float idleDelay, walkDelay, attackDelay, dieDelay;
    bool idleFlip;

    int idle, attack, walk, die;
    bool isIdle, isAttacking, isWalking, isDead;

    /// <summary>
    /// inAsp is a struct filled with information of which sprites the animations starts and stops.
    /// boundingBoxOffset makes sure that the boundingboxes are placed correctly to the incoming sprites
    /// </summary>
    public void Init(GameObject go, string spritePath, string sortingLayer, AnimationStartingPoints inAsp, Vector2 boundingBoxOffset, bool isPlayer = false, bool kinematic = true)
    {
        asp = inAsp;
        sprites = Resources.LoadAll<Sprite>(spritePath);
        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprites[0];
        sr.sortingLayerID = SortingLayer.NameToID(sortingLayer);

        float heightOfTile = GridManager.GetTileHeight();
        float widthOfTile = GridManager.GetTileWidth();

        bc = go.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(widthOfTile, heightOfTile);
        bc.offset = new Vector2(boundingBoxOffset.x, heightOfTile * boundingBoxOffset.y);

        if (isPlayer)
        {
            rb = go.AddComponent<Rigidbody2D>();
            if (kinematic)
            {
                rb.isKinematic = true;
                rb.useFullKinematicContacts = true;
            }
            else
            {
                rb.gravityScale = 0;
                rb.freezeRotation = true;
            }
        }

        ResetAnimations();

        isIdle = false;
        isAttacking = false;
        isWalking = false;
        isDead = false;

        walkDelay = 0.1f;
        idleDelay = 0.15f;
        attackDelay = 0.1f;
        dieDelay = 0.1f;

        StartWalking();
    }

    public void Idle()
    {
        ResetAttack();

        animationTimer += Time.deltaTime;
        
        if (animationTimer > idleDelay)
        {
            sr.sprite = sprites[idle];
        
            if (idleFlip)
            {
                idle--;
            }
            else
            {
                idle++;
            }
        
            if (idle == asp.idleEnd)
            {
                idleFlip = true;
            }
            else if (idle == asp.idle)
            {
                idleFlip = false;
            }
        
            animationTimer = 0;
        }
    }

    public void Walk()
    {
        ResetAttack();

        animationTimer += Time.deltaTime;
        
        if (animationTimer > walkDelay)
        {
            sr.sprite = sprites[walk];
            walk++;
        
            if (walk > asp.walkEnd)
            {
                walk = asp.walk;
            }
        
            animationTimer = 0;
        }
    }

    void ResetAttack()
    {
        attack = asp.attack;
    }

    public bool Attack()
    {
        // Only do damage when the animation is finished
        bool damage = false;

        animationTimer += Time.deltaTime;

        if (animationTimer > attackDelay)
        {
            sr.sprite = sprites[attack];
            attack++;

            if (attack > asp.attackEnd)
            {
                attack = asp.attack;

                // Do damage
                damage = true;
            }

            animationTimer = 0;
        }

        return damage;
    }

    public double Die()
    {
        deadTimer += Time.deltaTime;

        if (deadTimer > dieDelay && !IsDead())
        {
            sr.sprite = sprites[die];
            die++;

            if (die == asp.dieEnd)
            {
                StartDying();
            }

            deadTimer = 0;
        }

        return deadTimer;
    }

    public bool IsFlipped()
    {
        return sr.flipX;
    }

    public void FlipX()
    {
        if (IsFlipped())
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }

    public Vector4 GetCurrentSize(Vector2 currentScale)
    {
        return sr.sprite.bounds.size * currentScale;
    }

    public void ResetAnimations()
    {
        idleFlip = false;
        animationTimer = 0;
        deadTimer = 0;

        idle = asp.idle;
        attack = asp.attack;
        walk = asp.walk;
        die = asp.die;
    }

    public void StartAttacking()
    {
        isIdle = false;
        isAttacking = true;
        isWalking = false;
        isDead = false;
    }

    public void StartWalking()
    {
        isIdle = false;
        isAttacking = false;
        isWalking = true;
        isDead = false;
    }

    public void StartIdle()
    {
        isIdle = true;
        isAttacking = false;
        isWalking = false;
        isDead = false;
    }

    public void StartDying()
    {
        isIdle = false;
        isAttacking = false;
        isWalking = false;
        isDead = true;

        bc.isTrigger = true;

        if (rb != null) rb.useFullKinematicContacts = false;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsIdle()
    {
        return isIdle;
    }

    public List<Collider2D> GetListOfOverlapColliders(LayerMask layerMask, BoxCollider2D customBox = null)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(layerMask);

        List<Collider2D> results = new List<Collider2D>();

        // If no custom collider box
        if (customBox == null) bc.OverlapCollider(filter, results);
        else customBox.OverlapCollider(filter, results);

        return results;
    }

    public BoxCollider2D GetBoxCollider2D()
    {
        return bc;
    }

    public Vector3 GetColliderPivotPoint(GameObject go)
    {
        Vector3 pivotOffsetWorldSpace = go.transform.position - bc.bounds.center;

        return new Vector3(
                    go.transform.position.x,
                    go.transform.position.y + pivotOffsetWorldSpace.y,
                    go.transform.position.z
                    );
    }
}
