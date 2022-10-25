using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileCounter
{
    public static int projectileCounter = 0;
}

public class ProjectileManager : MonoBehaviour
{
    GameObject projectiles;

    List<Projectile> projectilesList;

    // Start is called before the first frame update
    void Start()
    {
        projectiles = new GameObject("Projectiles");
        projectiles.transform.SetParent(GameManager.GameManagerObject.transform);
        projectilesList = new List<Projectile>();
    }

    public void SpawnProjectile(GameObject source, Vector2 origin, Vector2 destination, Sprite sprite, Vector2 size, int damage, float timeToLive, LayerMask target)
    {
        Projectile proj = new Projectile();
        proj.Init(source, projectiles, origin, destination, sprite, size, damage, timeToLive, target);

        projectilesList.Add(proj);

        ProjectileCounter.projectileCounter++;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < projectilesList.Count; i++)
        {
            if(!projectilesList[i].Update())
            {
                projectilesList.Remove(projectilesList[i]);
            }
        }
    }
}
