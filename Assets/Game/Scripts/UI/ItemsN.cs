using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemsN : MonoBehaviour,IInputClickHandler
{

    private Action act;


#if UNITY_EDITOR
    private void OnMouseDown()
    {
        OnPressed();
    }
#endif

    public void Init( Action call)
    {
        act = call;
    }

    public void OnPressed()
    {
    //    DebuggerForMe.Instance.WriteForword("OnPressed" + gameObject.name,true);
        if (act != null)
        {
            act();
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        OnPressed();
    }
}
