using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumpadController : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.InputField ipAddressInputField;
    
    public void AddString(string s)
    {
        if (ipAddressInputField.text.Length < 20)
        {
            ipAddressInputField.text += s;
        }
    }

    public void Backspace()
    {
        if(ipAddressInputField.text.Length > 0)
        {
            ipAddressInputField.text = ipAddressInputField.text.Substring(0, ipAddressInputField.text.Length - 1);
            Sound.Instance.PlayerEffect("Spawn");
        }
    }
}
