using UnityEngine;

public class PositionRendererSorter : MonoBehaviour 
{
    int sortingOrderBase = 9999999; // This number should be higher than what any of your sprites will be on the position.y
    int offset = 0;
    bool runOnlyOnce = false;

    float timer;
    float timerMax = .1f;
    Renderer myRenderer;

    private void Start() 
    {
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() 
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) 
        {
            timer = timerMax;
            myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y * 10 - offset);
            if (runOnlyOnce) 
            {
                Destroy(this);
            }
        }
    }

}
