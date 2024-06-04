using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace NOOD
{
    public class DelayCall : MonoBehaviorInstance<DelayCall>
    {
        Queue<Action> mainThreadQueue = new Queue<Action>();

        private void Update()
        {
            if (mainThreadQueue.Count > 0)
            {
                Action action = mainThreadQueue.Dequeue();
                action();
            }
        }

        public void StartDelayCall(Action action, float delaySecond)
        {
            GameObject delayObj = new GameObject("DelayActionGameObject");
            DelayAction delay = delayObj.AddComponent<DelayAction>();

            delay.StartDelayFunction(() =>
            {
                action?.Invoke();
                Destroy(delay.gameObject, 0.5f);
            }, "", delaySecond);
        }
    }

    // public class DelayAction : MonoBehaviour
    // {
    //     public Action OnComplete;
    //     Action delayAction;
    //     float delaySecond;

    //     private IEnumerator Co_Function()
    //     {
    //         yield return new WaitForSeconds(delaySecond);
    //         delayAction?.Invoke();
    //         OnComplete?.Invoke();
    //         Destroy(this.gameObject);
    //     }

    //     public void StartDelayFunction(Action action, float second)
    //     {
    //         this.delayAction = action;
    //         this.delaySecond = second;
    //         StartCoroutine(Co_Function());
    //     }
    // }
}
