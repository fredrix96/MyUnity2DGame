using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static bool DebugMode = false;

    static double time = 0;
    static bool timeStarted = false;

    public static float CalculateDistance(float x1, float x2)
    {
        float dist;

        if (x1 < 0)
        {
            dist = Mathf.Sqrt(Mathf.Pow(x2 + Mathf.Abs(x1), 2));
        }
        else
        {
            dist = Mathf.Sqrt(Mathf.Pow(x2 - x1, 2));
        }

        return dist;
    }

    public static int SortByValue(Tile t1, Tile t2)
    {
        return t1.GetValue().CompareTo(t2.GetValue());
    }

    public static void StartTimer()
    {
        if (!timeStarted)
        {
            time = Time.realtimeSinceStartup;
            timeStarted = true;
        }
        else
        {
            Debug.LogWarning("Time has already started! Stop the timer before starting it again!");
        }
    }

    public static void StopTimer()
    {
        if (timeStarted)
        {
            double finalTimeMs = (Time.realtimeSinceStartup - time) * 1000;
            Debug.Log("Time (ms): " + finalTimeMs);
            time = 0;
            timeStarted = false;
        }
        else
        {
            Debug.LogWarning("The timer has not started yet! Start the timer before you can stop it!");
        }
    }

    public static void LogFPS()
    {
        Debug.Log("FPS: " + (int)(1f / Time.unscaledDeltaTime));
    }
}
