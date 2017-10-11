using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public delegate void E_Airtap(string str);

public class WorldSpaceUI : Singleton<WorldSpaceUI> {


    #region 字段

   

    //点击后将被点击名字广播出去
    public event E_Airtap e_tap;

    //手势类
    public GestureRecognizer recognizer;


    public TapPanels[] panels = new TapPanels[(int)UItype.panel_count];



    //凝视光标
    public InteractiveMeshCursor cursor;

    #endregion


    #region 类型

    public enum UItype
    {
        Decorate,   //装饰
        Furniture,  //家具
        Decoration, //物件
        panel_count
    }

    [Serializable]
    public class TapPanels
    {
        public GameObject button;
        public SpriteRenderer maskgrund;
        public SpriteRenderer backgrund;
        public Transform grid;
        public List<Transform> gridbuttons = new List<Transform>();
    }

    #endregion


   

}
