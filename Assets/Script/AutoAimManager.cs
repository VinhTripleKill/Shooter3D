using System.Collections.Generic;
using UnityEngine;
public static class AutoAimManager
{
    public static List<IAutoAimTarget> Targets =
        new List<IAutoAimTarget>();

    public static void Register(
        IAutoAimTarget target)
    {
        if (!Targets.Contains(target))
        {
            Targets.Add(target);
        }
    }

    public static void Unregister(
        IAutoAimTarget target)
    {
        Targets.Remove(target);
    }
}