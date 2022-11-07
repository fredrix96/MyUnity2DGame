using UnityEngine;

public class PositionRendererSorter : MonoBehaviour 
{
    int sortingOrderBase = 32767; // Maximum layer
    bool runOnlyOnce = false;

    float timer;
    float timerMax = .1f;
    Renderer myRenderer;
    float offsetY;
    Vector2 originalPos;

    private void Start() 
    {
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalPos = gameObject.transform.position;
        offsetY = myRenderer.bounds.extents.y * 2;
    }

    private void LateUpdate() 
    {
        if (gameObject != null)
        {
            originalPos = gameObject.transform.position;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f) 
        {
            timer = timerMax;
            myRenderer.sortingOrder = (int)(sortingOrderBase - originalPos.y * 10 - offsetY * 10);
            if (runOnlyOnce) 
            {
                Destroy(this);
            }
        }
    }

    public void UpdateOrder()
    {
        //myRenderer.sortingOrder = (int)(sortingOrderBase - originalPos.y * 10 - offsetY * 10);
    }

}
