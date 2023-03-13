using System;
using System.Collections.Generic;

public static class ThreadManager
{
    private static Queue<Action> ToExecute = new Queue<Action>();
    private static Queue<Action> Copy = new Queue<Action>();
    private static bool Empty = true;

    public static void AddAction(Action toAdd)
    {
        lock (ToExecute)
        {
            ToExecute.Enqueue(toAdd);
            Empty = false;
        }
    }

    internal static void Execute()
    {
        if (!Empty)
        {
            lock (ToExecute)
            {
                Copy = ToExecute;
                ToExecute = new Queue<Action>();
                Empty = true;
            }
            lock (Copy)
            {
                for (int i = 0; i < Copy.Count; i++)
                {
                    var action = Copy.Dequeue();
                    action?.Invoke();
                }
            }
        }
    }
}