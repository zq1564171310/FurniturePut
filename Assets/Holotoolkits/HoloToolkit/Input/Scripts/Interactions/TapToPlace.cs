// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;

public enum ModelState
{
    Ido, Move, Rotatet, ChangeColor, Dele, Lerp
}


namespace HoloToolkit.Unity.SpatialMapping
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// This script is used in conjunction with GazeManager, GestureManager,
    /// and SpatialMappingManager.
    /// TapToPlace also adds a WorldAnchor component to enable persistence.
    /// </summary>

    public class TapToPlace : StateMechinePro, IFocusable, IManipulationHandler,IInputClickHandler
    {

        #region 字段
        //定义状态
        State ido = new State();
        State delet = new State();
        State move = new State();
        State rotate = new State();
        State changecolor = new State();

        //初始Lerp到预定位置
        State Lerpfirst = new State();

        [SerializeField]
        private Vector3 spawnPos;

        Material selfmaterial;

        public Action act_callback;

        [Tooltip("Supply a friendly name for the anchor as the key name for the WorldAnchorStore.")]
        public string SavedAnchorFriendlyName = "SavedAnchorFriendlyName";

        [Tooltip("Place parent on tap instead of current game object.")]
        public bool PlaceParentOnTap;

        [Tooltip("Specify the parent game object to be moved on tap, if the immediate parent is not desired.")]
        public GameObject ParentGameObjectToPlace;

        /// <summary>
        /// Keeps track of if the user is moving the object or not.
        /// Setting this to true will enable the user to move and place the object in the scene.
        /// Useful when you want to place an object immediately.
        /// </summary>
        [Tooltip("Setting this to true will enable the user to move and place the object in the scene without needing to tap on the object. Useful when you want to place an object immediately.")]
        public bool IsBeingPlaced;

        /// <summary>
        /// Manages persisted anchors.
        /// </summary>
        protected WorldAnchorManager anchorManager;

        /// <summary>
        /// 是否可以改变颜色
        /// </summary>
        public bool canchangecolor = false;



        /// <summary>
        /// Controls spatial mapping.  In this script we access spatialMappingManager
        /// to control rendering and to access the physics layer mask.
        /// </summary>
        protected SpatialMappingManager spatialMappingManager;



        private TextMesh id_debuggertext, localpos_debuggertext;

        private Vector3 lastpos;

        private Vector3 lastrot;

        [SerializeField]
        private string myID;

        public string MyID
        {
            get { return myID; }
            set { myID = value; }
        }


        #endregion


        #region UNITY回调

        protected virtual void Start()
        {
            //状态机绑定


            ver1 = transform.position; ///////////////////

            Lerpfirst.OnUpdate = LerpUpdat;

            move.OnEnter = OnInputClicked;
            move.OnUpdate = MoveUpdate;
            move.OnLeave = OnInputClicked;

            rotate.OnEnter = EnterRotate;
            rotate.OnLeave = LeaveRoate;

            changecolor.OnLeave = LeaveChangColor;

            delet.OnEnter = Delet;

            //初始化状态
            // STATE = ido;

            selfmaterial = GetComponentInChildren<MeshRenderer>().material;

            Debug.Log(selfmaterial);
            //   WorldSpaceUI.Instance.e_tap += HitOnMe;

            // Make sure we have all the components in the scene we need.
            anchorManager = WorldAnchorManager.Instance;
            if (anchorManager == null)
            {
                Debug.LogError("This script expects that you have a WorldAnchorManager component in your scene.");
            }

            spatialMappingManager = SpatialMappingManager.Instance;
            if (spatialMappingManager == null)
            {
                Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
            }

            if (anchorManager != null && spatialMappingManager != null)
            {
                // If we are not starting out with actively placing the object, give it a World Anchor
                if (!IsBeingPlaced)
                {
                //    anchorManager.AttachAnchor(gameObject, SavedAnchorFriendlyName);
                }
            }
            else
            {
                // If we don't have what we need to proceed, we may as well remove ourselves.
                Destroy(this);
            }

            if (PlaceParentOnTap)
            {
                if (ParentGameObjectToPlace != null && !gameObject.transform.IsChildOf(ParentGameObjectToPlace.transform))
                {
                    Debug.LogError("The specified parent object is not a parent of this object.");
                }

                DetermineParent();
            }
            if (transform.Find("id_debuggertext") != null)
            {
                id_debuggertext = transform.Find("id_debuggertext").GetComponent<TextMesh>();
                localpos_debuggertext = transform.Find("localpos_debuggertext").GetComponent<TextMesh>();
                id_debuggertext.text = "我的ID::" + myID.ToString();
            }

        }




        void Update()
        {
            OnUpdater(ti);
        }


        #endregion

        #region 接口实现
        public void OnFocusEnter()
        {
            //MeshRenderer[] meshrenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            //for (int i = 0; i < meshrenderers.Length; i++)
            //{
            //    meshrenderers[i].material.color = Color.red;
            //}
            // CustomMessages.Instance.SendCommand("进入：：" + gameObject.name);
        }

        public void OnFocusExit()
        {

            //MeshRenderer[] meshrenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            //for (int i = 0; i < meshrenderers.Length; i++)
            //{
            //    meshrenderers[i].material.color = Color.yellow;
            //}
            //   CustomMessages.Instance.SendCommand("移出：：" + gameObject.name);
        }



        /// <summary>
        /// 该接口已不实现，由OnPressed代替
        /// </summary>
        public virtual void OnInputClicked()
        {
            // On each tap gesture, toggle whether the user is in placing mode.
            IsBeingPlaced = !IsBeingPlaced;

            // If the user is in placing mode, display the spatial mapping mesh.
            if (IsBeingPlaced)
            {
                spatialMappingManager.DrawVisualMeshes = true;

                //anchorManager.RemoveAnchor(gameObject);
            }
            // If the user is not in placing mode, hide the spatial mapping mesh.
            else
            {
                spatialMappingManager.DrawVisualMeshes = false;
                // Add world anchor when object placement is done.
                //anchorManager.AttachAnchor(gameObject, SavedAnchorFriendlyName);
            }
        }


        #endregion


        GameObject UI;
        #region 帮助方法

        public void Init(string id, Vector3 spownpos)
        {
            MyID = id;
            spawnPos = spownpos;

            ChangeState(ModelState.Lerp);
        }

        /// <summary>
        /// Guesture内定义回调
        /// </summary>
        private void OnPressed()
        {
            if (STATE == move)
            {
                ChangeState(ModelState.Ido);
                ObjectPool.Instance.UnSpawn(UI);
                UI = null;
            }
            else
            {
                if (UI == null)
                {
                    UI = ObjectPool.Instance.Spawn("ItemUI");
                    UI.GetComponent<UIcontroller>().Init(transform.Find("UIPos").position, this);

                    //showUI
                    Sound.Instance.PlayerEffect("ShowUI");
                }
                else
                {
                    if (act_callback != null)
                    {
                        act_callback();

                        //closeUI
                        Sound.Instance.PlayerEffect("CloseUI");
                    }
                }
            }

        }
        private void DetermineParent()
        {
            if (ParentGameObjectToPlace == null)
            {
                if (gameObject.transform.parent == null)
                {
                    Debug.LogError("The selected GameObject has no parent.");
                    PlaceParentOnTap = false;
                }
                else
                {
                    Debug.LogError("No parent specified. Using immediate parent instead: " + gameObject.transform.parent.gameObject.name);
                    ParentGameObjectToPlace = gameObject.transform.parent.gameObject;
                }
            }
        }
        #endregion


        Vector3 ver1;//=============
        Vector3 ver2;//================
        float ti; ///==========
        #region 状态机方法

        //初始Lerp到预定位置
        void LerpUpdat(float timer)
        {
            if (Vector3.Distance(transform.position, spawnPos) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, spawnPos, 0.0625f);
            }
            else
            {
                ChangeState(ModelState.Ido);
            }
        }


        //移动
        void MoveUpdate(float timer)
        {
            // If the user is in placing mode,
            // update the placement to match the user's gaze.

            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, spatialMappingManager.LayerMask))
            {
                // Rotate this object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;

                // Move this object to where the raycast
                // hit the Spatial Mapping mesh.
                // Here is where you might consider adding intelligence
                // to how the object is placed.  For example, consider
                // placing based on the bottom of the object's
                // collider so it sits properly on surfaces.
                if (PlaceParentOnTap)
                {
                    // Place the parent object as well but keep the focus on the current game object
                    Vector3 currentMovement = hitInfo.point - gameObject.transform.position;
                    ParentGameObjectToPlace.transform.position += currentMovement;
                    ParentGameObjectToPlace.transform.rotation = toQuat;
                }
                else
                {
                    gameObject.transform.position = hitInfo.point;
                }
            }

            //发送自己位置
            if (!string.IsNullOrEmpty(MyID) && lastpos != transform.localPosition && STATE == move)
            {
                CustomMessages.Instance.SendUpdatePos(MyID, transform.localPosition, transform.localRotation);
                lastpos = transform.localPosition;
                if (localpos_debuggertext)
                {
                    localpos_debuggertext.text = "我的位置和角度::" + transform.localPosition.ToString() + "\n" + transform.localEulerAngles;
                }
            }

        }

        //旋转

        void EnterRotate()
        {
            transform.Find("RoateIcon").gameObject.SetActive(true);
        }

        public void OnManipulationStarted(ManipulationEventData eventData)
        {
        }

        public void OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (STATE == rotate)
            {
                Vector3 temp = eventData.CumulativeDelta;
                float xdistance = temp.x;
                Vector3 localeulerangles = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(localeulerangles.x, localeulerangles.y - temp.x * 4, localeulerangles.z);

                if (!string.IsNullOrEmpty(MyID) && lastrot != transform.localEulerAngles)
                {
                    CustomMessages.Instance.SendUpdatePos(MyID, transform.localPosition, transform.localRotation);
                    lastrot = transform.localEulerAngles;
                    if (localpos_debuggertext)
                    {
                        localpos_debuggertext.text = "我的位置和角度::" + transform.localPosition.ToString() + "\n" + transform.localEulerAngles;
                    }
                }
            }
        }

        public void OnManipulationCompleted(ManipulationEventData eventData)
        {
        }

        public void OnManipulationCanceled(ManipulationEventData eventData)
        {
        }


        void LeaveRoate()
        {
            transform.Find("RoateIcon").gameObject.SetActive(false);
        }

        //改变颜色

        void EnterChangeColor()
        {

        }

        void LeaveChangColor()
        {
            if (act_callback != null)
            {
                act_callback();
            }
        }

        //删除
        void Delet()
        {
            ObjectPool.Instance.UnSpawn(UI);
            UI = null;
            CustomMessages.Instance.SendDelet(myID);
            MessageRecever.Instance.maps.Remove(MyID);
            Destroy(gameObject);
        }


        public void ChangeState(ModelState ms)
        {
            switch (ms)
            {
                case ModelState.Ido:
                    STATE = ido;
                    Sound.Instance.PlayerEffect("DropDown");
                    break;
                case ModelState.Move:
                    STATE = move;
                    Sound.Instance.PlayerEffect("Move");
                    break;
                case ModelState.Rotatet:
                    STATE = rotate;
                    break;
                case ModelState.ChangeColor:
                    // STATE = changecolor;
                    break;
                case ModelState.Dele:
                    STATE = delet;
                    Sound.Instance.PlayerEffect("Delete");
                    break;
                case ModelState.Lerp:
                    STATE = Lerpfirst;
                    //   Sound.Instance.PlayerEffect("PlaceMenu");
                    break;
                default:
                    break;
            }

        }

        public void Changcolor(Color color)
        {
            //    DebuggerForMe.Instance.WriteForword(gameObject.name +  "需要Changecolor....");
            selfmaterial.color = color;
            CustomMessages.Instance.SendColor(myID, new Vector3(color.r, color.g, color.b));
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            OnPressed();
        }

        #endregion

    }
}
