using System;
using System.Collections;
using UnityEngine;


public static class Utils 
{
    public static IEnumerator WaitAndExecute(float time, Action callback = null)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
}
