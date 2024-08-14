using System;
using System.Collections;
using UnityEngine;

namespace Arcadian.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void RunEndOnFrame(this MonoBehaviour monoBehaviour, Action function)
        {
            monoBehaviour.StartCoroutine(DelayedFunction(function));
        }

        private static IEnumerator DelayedFunction(Action function)
        {
            yield return new WaitForEndOfFrame();
            
            function?.Invoke();
        }
    }
}