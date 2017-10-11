using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class FurnitureManager : MonoBehaviour {


    #region 字段
    public SharingStage sharingStage;

    public AsyncDialog dialog;


    public GameObject ModelUI;

    public bool RunAsHoloLensMode = false;
    public string HololensAutoConnectIPAddress;

    #endregion


    #region 属性
    #endregion

    #region Unity回调
    // Use this for initialization
    void Start()
    {
        StartCoroutine(SetupConnections());
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion


    #region 事件回调

    #endregion


    #region 帮助方法

    private IEnumerator SetupConnections()
    {
        //this.sharingStage.enabled = true;
        this.sharingStage.SharingManagerConnected += (sender, e) =>
        {
            //to do ...
            ModelUI.gameObject.SetActive(true);
            dialog.CloseDialog();
        };

        if (IsHoloLens())
        {
            string autoConnectIPAddress = GetAutoConnectIPAddress();

            while (!this.sharingStage.IsConnected)
            {
                string ipAddress = autoConnectIPAddress;
                autoConnectIPAddress = null;
                if (ipAddress == null)
                {
                    // Dialog for IP address
                    var dialogResult = dialog.ShowDialog();
                    yield return dialogResult;
                    ipAddress = dialogResult.GetIPAddress();
                    // Connect to server
                    if (dialogResult.IsSolo())
                    {
                        //this.sharingStage.ConnectSolo();
                        this.sharingStage.enabled = false;
                        break;
                    }
                }
                Debug.Log("Received dialog IP address: " + ipAddress);
                this.sharingStage.ServerAddress = ipAddress;
                this.sharingStage.ConnectToServer(ipAddress, 20602);
                this.sharingStage.enabled = true;
                // Wait for 4 seconds
                for (int i = 0; i < 8 && !this.sharingStage.IsConnected; i++)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            yield return -1;

            // Create read movable ball
            //InitializeSpeechRecognigzer();

            //InitializeGestureRecognizer();
        }
        else
        {
            this.sharingStage.enabled = true;
        }
    }



    private bool IsHoloLens()
    {
        return RunAsHoloLensMode ||
            (SystemInfo.deviceType == DeviceType.Desktop &&
            SystemInfo.deviceModel.StartsWith("HoloLens"));
    }


    private string GetAutoConnectIPAddress()
    {
        IPAddress address;
        if (!string.IsNullOrEmpty(HololensAutoConnectIPAddress) && IPAddress.TryParse(HololensAutoConnectIPAddress, out address))
        {
            return HololensAutoConnectIPAddress;
        }
        return null;
    }

    #endregion


    #region 接口实现
    #endregion



}
