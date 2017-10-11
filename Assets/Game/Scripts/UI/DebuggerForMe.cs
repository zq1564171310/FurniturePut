using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerForMe : Singleton<DebuggerForMe>
{

    private TextMesh selftext;


    private void Start()
    {
        selftext = GetComponent<TextMesh>();
    }

    public void WriteForword(string text,bool needclean = false)
    {
        if (needclean)
        {
            selftext.text = text;
        }
        else {
            selftext.text += text;
        }
    }

}
