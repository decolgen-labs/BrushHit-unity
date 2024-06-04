using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGrowUpHandler : MonoBehaviour
{
    public static UIGrowUpHandler Ins;

    [SerializeField] ToggleHandler[] toggleHandlers;
    [SerializeField] Animator _growUpCanvasAnimator;

    private int index = 0;

    private void Awake()
    {
        //Create Singleton
        if (Ins)
        {
            Destroy(Ins);
        }
        Ins = this;
        DontDestroyOnLoad(Ins);
    }

    //This function calls when player get 1 grow up point
    public void AddGrowUp()
    {
        //Toggle the UI
        toggleHandlers[index].OnToggle();

        //Show UI
        _growUpCanvasAnimator.SetTrigger("Show");
        index++;
        if(index == 3)
        {
            //Display notification when player get enough 3 grow up point and reset UI
            UIManager.Ins.DisplayNotification(1, 0);
            Reset();
        }
    }

    private void Reset()
    {
        index = 0;
        foreach (var toggle in toggleHandlers)
        {
            toggle.OnToggle();
        }
    }
}
