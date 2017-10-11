using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageRecever :Singleton<MessageRecever>
{
#region 字段

    //上一次打开的panel
    private GameObject lasthit;

    public TextMesh Debuggertext;

    public GameObject obj_parent;

#endregion



    #region UNITY回调

    void Start()
    {
        //默认开启中间的
        lasthit =WorldSpaceUI.Instance.panels[1].grid.gameObject;
        //绑定消息
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Commands] = OnCommandsRecive;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.SpawnCommands] = OnSpawnCommandsRecive;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.UpdatePos] = OnUpdatePosReceive;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Delet] = OnDeletRecive;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Color] = OnColorReceive;


        //注册事件
        acts.Add("Decorate", () => { OpenPanel("Decorate"); });
        acts.Add("Decoration", () => { OpenPanel("Decoration"); });
        acts.Add("Furniture", () => { OpenPanel("Furniture"); });
        acts.Add("TurnOffDebuggerText", () => { Debuggertext.transform.localScale = Vector3.zero; });
        acts.Add("TurnOnDebuggerText", () => { Debuggertext.transform.localScale = Vector3.one * 0.005f; });
    }
    #endregion

    #region 接受消息事件
    //命令语句与事件对应关系
    public Dictionary<string, Action> acts = new Dictionary<string, Action>();

    //生成物体与ID对应关系
    public Dictionary<string, Transform> maps = new Dictionary<string, Transform>();

    /// <summary>
    /// 接受到操作命令
    /// </summary>
    /// <param name="msg"></param>
    private void OnCommandsRecive(NetworkInMessage msg)
    {
        bool turnon = true;
        long userID = msg.ReadInt64();
        XString xs = msg.ReadString();
        string[] ss = xs.ToString().Split('/');
        Debug.Log("<color=yellow>收到消息：：：：" + xs.ToString() + "</color>");

        string id = ss[0];

        if (ss.Length == 1)
        {
            //控制命令
            if (acts.ContainsKey(id))
            {
                acts[id]();
            }
            else
            {
                Debug.Log("未注册该命令！！！！！！！！！！");
            }
        }




    }

    /// <summary>
    /// 接受到生成物体的命令
    /// </summary>
    /// <param name="msg"></param>
    private void OnSpawnCommandsRecive(NetworkInMessage msg)
    {
        long userID = msg.ReadInt64();
        XString xs = msg.ReadString();

        Vector3 pos = CustomMessages.Instance.ReadVector3(msg);
        string ss = xs.ToString();
        string[] meg = ss.Split('/');
        string name = meg[0];
        string id = meg[1];
        AddobjectToWorld(name, id, pos);
    }

    /// <summary>
    /// 接受到删除命令
    /// </summary>
    /// <param name="msg"></param>
    private void OnDeletRecive(NetworkInMessage msg)
    {
        long userID = msg.ReadInt64();
        XString str = msg.ReadString();
        string id = str.ToString();
        if (maps.ContainsKey(id))
        {
            Destroy(maps[id].gameObject);
            maps.Remove(id);
        }
        else
        {
            Debug.Log("<color=red>删除物体错误，不包含该ID::" + id + "</color>");
        }

    }

    /// <summary>
    /// 接受到位置更新消息
    /// </summary>
    /// <param name="msg"></param>
    private void OnUpdatePosReceive(NetworkInMessage msg)
    {
        long userID = msg.ReadInt64();
        XString str = msg.ReadString();
        string id = str.ToString();
        if (maps.ContainsKey(id))
        {
            maps[id].localPosition = CustomMessages.Instance.ReadVector3(msg);
            maps[id].localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        }
        else
        {
            Debug.Log("<color=red>不包含该ID::" + id + "</color>");
        }
    }

    /// <summary>
    /// 接受到颜色更改消息
    /// </summary>
    /// <param name="msg"></param>
    private void OnColorReceive(NetworkInMessage msg)
    {
        Debug.Log("接收到color消息");
        long userID = msg.ReadInt64();
        XString xs = msg.ReadString();

        string id = xs.ToString();
        if (maps.ContainsKey(id) && maps[id] != null)
        {
            Vector3 col = CustomMessages.Instance.ReadVector3(msg);

            Color c = new Color(col.x, col.y, col.z);
            maps[id].GetComponentInChildren<MeshRenderer>().material.color = c;
        }

    }

    /// <summary>
    /// 生成新物体
    /// </summary>
    /// <param name="path">Resources下的路径</param>
    /// <param name="id">生成物的ID</param>
    public void AddobjectToWorld(string path, string id, Vector3 pos)
    {
        //GameObject go = Resources.Load<GameObject>("Furniture/" + path);
        GameObject go = Resources.Load<GameObject>("GrateFurniture/" + path);

        if (go == null)
        {
            Debug.Log("<color=red>Resource为空</color>");
            return;
        }
        GameObject furniture = Instantiate(go);
        furniture.transform.SetParent(obj_parent.transform);
        furniture.transform.position = pos;

        Vector3 spawnpos = transform.position - transform.forward * 0.6f- transform.up * 0.5f;
        furniture.GetComponent<TapToPlace>().Init(id, spawnpos);
        maps.Add(id, furniture.transform);

    }
    #endregion

    #region 帮助方法
    void OpenPanel(string path)
    {
        lasthit.SetActive(false);
        GameObject go = transform.Find("Plane/" + path.ToString() + "/Plane").gameObject;
        go.SetActive(true);
        lasthit = go;
    }
    #endregion

}
