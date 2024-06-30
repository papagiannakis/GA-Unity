using R41;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A pool of R410 arrays. This is a singleton class, and the instance is created during the Awake event.
/// </summary>
public class R410Pool : MonoBehaviour
{
    /// <summary>
    /// Static instance of the R410Pool class.
    /// </summary>
    public static R410Pool Instance { get; private set; }

    /// <summary>
    /// Pool size of the R410 arrays.
    /// </summary>
    public int poolSize = 5000;

    // The pool of float arrays
    private Queue<R410> r410Queue = new Queue<R410>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            r410Queue.Enqueue(new R410());
        }
    }

    /// <summary>
    /// Get an R410 array from the pool. If the pool is empty, a new R410 array is created.
    /// </summary>
    /// <returns> An R410 object from the pool.</returns>
    public static R410 GetFromPool()
    {
        if (Instance == null || Instance.r410Queue.Count == 0)
        {
            Instance.r410Queue.Enqueue(new R410());
        }
        return Instance.r410Queue.Dequeue();
    }

    /// <summary>
    /// Returns an R410 object to the pool after resetting its internal state. If the provided object is null, the method does nothing.
    /// </summary>
    /// <param name="floatArray">The R410 object to be returned to the pool.</param>
    public static void ReturnToPool(R410 floatArray)
    {
        if (floatArray == null)
        {
            return;
        }
        Array.Clear(floatArray._mVec, 0, floatArray._mVec.Length);
        Instance.r410Queue.Enqueue(floatArray);
    }
}