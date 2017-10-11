using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class AsyncDialog : MonoBehaviour {

    private bool isDone = false;
    public string ipAddress;
    private bool isSolo = false;

    //private TextMesh debugger;
    private InputField addressInput;
    private TouchScreenKeyboard keyboard;
    public static string keyboardText = "";

    private void Start()
    {
        //debugger = GameObject.Find("FPSDisplay/FPSText").GetComponent<TextMesh>();
        addressInput = this.transform.Find("IPAddressInput").GetComponent<InputField>();

        //取出数据，避免重复输入
        string ip = PlayerPrefs.GetString("IP");
        if (string.IsNullOrEmpty(ip))
        {
            ip = "";
        }
        addressInput.text = ip;
        //debugger.text = "playerprefs.getstring(IP)" + ip;
    }

    public DialogOperation ShowDialog()
    {
        this.isDone = false;
        this.ipAddress = "";


        this.gameObject.SetActive(true);
        return new DialogOperation(this);
    }



    public void CloseDialog()
    {
        this.gameObject.SetActive(false);
    }

    public void OnOk()
    {
        var ipString = GetIPAddressText();

        IPAddress address;
        if (IPAddress.TryParse(ipString, out address))
        {
            //保存数据
            PlayerPrefs.SetString("IP",ipString);
            this.ipAddress = address.ToString();
            Debug.Log("IP address is correct: " + address.ToString());
            this.gameObject.SetActive(false);
            isDone = true;
        }
        else
        {
            var caption = this.transform.Find("Text").GetComponent<Text>();
            caption.text = "无效IP地址，请重新输入";
            Debug.Log("Invalid IP " + this.ipAddress);
        }
    }

    public void OnSolo()
    {
        isSolo = true;
        isDone = true;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        // TODO: Load system soft keyboard
        //if (addressInput.isFocused && keyboard == null)
        //{
        //    keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad, false, false, false, false);
        //}
        //if (keyboard != null && TouchScreenKeyboard.visible == false)
        //{
        //    if (keyboard.done == true)
        //    {
        //        var textInput = this.transform.Find("IPAddressInput/Text");
        //        textInput.GetComponent<Text>().text = keyboard.text;
        //    }
        //}
    }

    private string GetIPAddressText()
    {
        var textInput = this.transform.Find("IPAddressInput/Text");
        Debug.Assert(textInput != null);
        var text = textInput.GetComponent<Text>();
        Debug.Assert(text != null);
        return text.text;
    }

    public class DialogOperation : CustomYieldInstruction
    {
        private AsyncDialog dlg;
        public DialogOperation(AsyncDialog dlg)
        {
            this.dlg = dlg;
        }
        public override bool keepWaiting
        {
            get
            {
                return !dlg.isDone;
            }
        }
        public string GetIPAddress()
        {
            return dlg.ipAddress;
        }
        public bool IsSolo()
        {
            return dlg.isSolo;
        }
    }
}
