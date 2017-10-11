using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class EveryItemOnPanel : MonoBehaviour,IInputClickHandler,IFocusable {

    #region 字段
    private GameObject rotateobj;
    private SpriteRenderer spriterenderer;
    #endregion

    #region UNITY回调
    void Start()
    {
        spriterenderer = GetComponentInChildren<SpriteRenderer>();
    }
#endregion



#if UNITY_EDITOR
    private void OnMouseDown()
    {
        OnPressed();
    }
#endif

#region Holotoolkit回调
    public void OnPressed()
    {
        //声音
        Sound.Instance.PlayerEffect("Spawn");

        string str = gameObject.name;
        string id = Guid.NewGuid().ToString();
        CustomMessages.Instance.SendSpawnCommand(str + "/" + id,transform.position);
        MessageRecever.Instance.AddobjectToWorld(str, id,transform.position);
    }

    public void OnFocusEnter()
    {
        spriterenderer.color = Color.green;
    }

    public void OnFocusExit()
    {
        spriterenderer.color = Color.white;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        OnPressed();
    }

    #endregion

}
