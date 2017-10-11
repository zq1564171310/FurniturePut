using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Examples.ColorPicker;
using HoloToolkit.Unity.InputModule;

public class UIcontroller : MonoBehaviour
{
    #region 字段
    TapToPlace conquertap;
    State show = new State();
    State close = new State();

    private Dictionary<string, Action> hituimap = new Dictionary<string, Action>();

    public GameObject delet, move, rotate, changcolor,colorpanel;

    public GazeableColorPicker picker;

    #endregion


    #region 属性
    #endregion

    #region Unity回调
    private void Start()
    {
        WorldSpaceUI.Instance.e_tap += HitOn;
        show.OnUpdate = Show;
        close.OnUpdate = Close;
    }



    private void Update()
    {
    //    OnUpdater(Time.deltaTime);
    }

    #endregion


    #region 事件回调

    #endregion


    #region 帮助方法

    public void Init(Vector3 pos,TapToPlace tap)
    {
        tap.act_callback = RevertTo;
        transform.position = pos;

        conquertap = tap;
        UpdateActions();

    }

    void UpdateActions()
    {
        changcolor.SetActive(conquertap.canchangecolor);
        delet.GetComponent<ItemsN>().Init(() => { conquertap.ChangeState( ModelState.Dele); });
        move.GetComponent<ItemsN>().Init(() => { conquertap.ChangeState(ModelState.Move); });


        GazeableColorPicker picker = changcolor.GetComponentInChildren<GazeableColorPicker>();
        //picker.Init(WorldSpaceUI.Instance.cursor);
        picker.OnPickedColor.RemoveAllListeners();
        picker.OnPickedColor.AddListener(conquertap.Changcolor);
    }


    private void RevertTo()
    {
        //    STATE = STATE == close ? show : close;
        transform.localScale = transform.localScale == Vector3.zero ? Vector3.one * 2 : Vector3.zero;
    //    DebuggerForMe.Instance.WriteForword("取反");
    }

    public void Show(float timer)
    {
        transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one * 2,0.25f);
    }

    public void Close(float timer)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.25f);
    }


    private void HitOn(string str)
    {
        //Camera.main.GetComponentInChildren<TextMesh>().text = "点击到￥：" + str;
        if (hituimap.ContainsKey(str) && GazeManager.Instance.HitObject != null && GazeManager.Instance.HitObject.transform.parent == gameObject)
        {
            hituimap[str]();
        } 
    }

    #endregion


    #region 接口实现
    #endregion

}
