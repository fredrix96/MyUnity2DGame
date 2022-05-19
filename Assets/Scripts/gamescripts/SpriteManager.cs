using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    SpriteRenderer sr;
    Sprite[] sprites;

    double animationTimer, deadTimer;

    float idleDelay, walkDelay, attackDelay, dieDelay;
    bool idleFlip;

    int idle, attack, walk, die;
    bool isIdle, isAttacking, isWalking, isDead;

    public void Init(GameObject go, string spritePath, string sortingLayer)
    {
        sprites = Resources.LoadAll<Sprite>(spritePath);
        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprites[idle];
        sr.sortingLayerID = SortingLayer.NameToID(sortingLayer);

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

            if (idle == 2)
            {
                idleFlip = true;
            }
            else if (idle == 0)
            {
                idleFlip = false;
            }

            animationTimer = 0;
        }
    }

    public void Walk()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer > walkDelay)
        {
            sr.sprite = sprites[walk];
            walk++;

            if (walk > 11)
            {
                walk = 7;
            }

            animationTimer = 0;
        }
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

            if (attack > 6)
            {
                attack = 3;

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

            if (die > 15)
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
        idle = 0;
        idleFlip = false;
        attack = 3;
        walk = 7;
        die = 12;

        animationTimer = 0;
        deadTimer = 0;
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
}
