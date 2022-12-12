using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public static class MultithreadingSystem
{
    static Thread[] threads;
    static IEnumerator coroutine;

    public static void Init()
    {
        int threadsCount = SystemInfo.processorCount;
        threads = new Thread[threadsCount];
    }

    public static Thread GetAvailableThread()
    {
        foreach (Thread thread in threads)
        {
            if (thread.ThreadState != ThreadState.Running)
            {
                return thread;
            }
        }

        return null;
    }

    public static void AssignJobToAvailableThread(ParameterizedThreadStart func)
    {
        Thread newThread = GetAvailableThread();
        newThread = new Thread(func);
        newThread.Start();
    }

    static IEnumerator WaitForThreadToFinish(Thread thread)
    {
        thread.Join();

        while (true)
        {
            yield return thread.ThreadState == ThreadState.Running;
        }
    }

    public static void FinishAndWaitForAllThreads()
    {
        foreach (Thread thread in threads)
        {
            if (thread != null)
            {
                thread.Join();
            }
        }

        ResetAllThreads();
    }

    public static void ResetAllThreads()
    {
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = null;
        }
    }
}
