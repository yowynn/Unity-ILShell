using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class Test
{
    public static void Invoke(this MonoBehaviour instance, string methodName, float time)
    {
        Debug.Log("??????????????????");
    }

    public static void Invoke2(this MonoBehaviour instance, string methodName, float time)
    {
        Debug.Log("??????????????????");
    }
}
