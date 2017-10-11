using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSendString : MonoBehaviour
{
    [SerializeField]
    NumpadController nc;
    
    public void SendString()
    {
        Sound.Instance.PlayerEffect("Move");
        nc.AddString(GetComponentInChildren<UnityEngine.UI.Text>().text);
    }
}
