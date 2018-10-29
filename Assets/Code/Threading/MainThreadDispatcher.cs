using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static Queue<Action> _awaitingExecution = new Queue<Action>();

    private void Update()
    {
        lock(_awaitingExecution)
        {
            while (_awaitingExecution.Count > 0)
            {
                _awaitingExecution.Dequeue().Invoke();
            }
        }
    }

    public static void Add(Action action)
    {
        lock (_awaitingExecution)
        {
            _awaitingExecution.Enqueue(action);
        }
    }
}
