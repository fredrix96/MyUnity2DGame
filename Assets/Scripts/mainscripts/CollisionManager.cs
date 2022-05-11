using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    void Start()
    {    
    }

    void OnCollisionEnter2D(Collision2D col)
    {   
        // If a soldier collides with an enemy
        if (col.collider.name[0] == 's' && col.otherCollider.name[0] == 'e')
        {
            col.gameObject.GetComponent<SpriteManager>().StartAttacking();
        }

        // If a enemy collides with a soldier or the player
        if (col.collider.name[0] == 'e' && (col.otherCollider.name[0] == 's' || col.otherCollider.name[0] == 'p'))
        {
            col.gameObject.GetComponent<SpriteManager>().StartAttacking();
        }
    }
}
