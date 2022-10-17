using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggles : MonoBehaviour
{
    [SerializeField] public bool multithreadingOn;

    // Start is called before the first frame update
    void Start()
    {
        multithreadingOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetMultithreadingOn()
    {
        return multithreadingOn;
    }
}
