using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainChoiseButton : MonoBehaviour, IInputClickHandler
{


#if UNITY_EDITOR
    private void OnMouseDown()
    {
        OnPressed();
    }
#endif


    public void OnPressed()
    {
        //声音
        Sound.Instance.PlayerEffect("MainChoiseButton");

        string str2 = gameObject.name;
        Dictionary<string, Action> acts = MessageRecever.Instance.acts;
       // DebuggerForMe.Instance.WriteForword("》" + str2,true);
        if (acts.ContainsKey(str2))
        {
            acts[str2]();
        }
        CustomMessages.Instance.SendCommand(str2);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        OnPressed();
    }
}
