using UnityEngine;

public static class MaximumSortingOrder
{
    public static int sortingOrderBase = 32767; // Maximum possible layer

    public static int GetID()
    {
        return sortingOrderBase;
    }
}

public class PositionRendererSorter : MonoBehaviour 
{
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
            myRenderer.sortingOrder = (int)(MaximumSortingOrder.GetID() - originalPos.y * 10 - offsetY);
            if (runOnlyOnce) 
            {
                Destroy(this);
            }
        }
    }

    /// <summary>
    /// This function sets the offset of the rendering sorting order manually
    /// </summary>
    public void SetOffsetManually(float inOffsetY)
    {
        offsetY = inOffsetY;
    }

    public void SetIsOnlyRunOnce()
    {
        runOnlyOnce = true;
    }

    public void UpdateOrder()
    {
        //myRenderer.sortingOrder = (int)(sortingOrderBase - originalPos.y * 10 - offsetY * 10);
    }

}
