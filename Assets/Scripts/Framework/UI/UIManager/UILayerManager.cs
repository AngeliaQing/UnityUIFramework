using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public enum UILayer
    {
        DefaultLayer,
        SystemPopupLayer,

    }

    public class UILayerManager : MonoBehaviour
    {
        IUILayerManagerHandler m_ui_layer_mng_handler;
        static GameObject ms_ui_root;
        Dictionary<int, Transform> m_layer2transfom = new Dictionary<int, Transform>();

        #region 常量
        const string UI_CAMERA_NAME = "UICamera";
        const string UI_CAMERA_PREFAB_PATH = "UI/Common/UICamera";
        const string UI_MESSAGE_BOX_PREFAB_PATH = "UI/Common/UIMessageBox";
        #endregion

        void Awake()
        {
            m_ui_layer_mng_handler = new NGUILayerManagerHandler();

            InitLayers();
            InitCameras();
        }
        public Transform GetParentByLayer(UILayer layer)
        {
            int i_layer = (int)layer;
            if (m_layer2transfom.ContainsKey(i_layer))
                return m_layer2transfom[i_layer];

            Debug.LogError("UILayerManager GetParentByLayer cannot find uilayer " + i_layer);
            return m_layer2transfom[(int)UILayer.DefaultLayer];
        }

        public List<Camera> InterceptiveCameras { get; set; }

        public void Forward(IUIBase ui)
        {
            if (m_ui_layer_mng_handler != null)
                m_ui_layer_mng_handler.Forward(ui);
        }

        public void Backward(IUIBase ui)
        {
            if (m_ui_layer_mng_handler != null)
                m_ui_layer_mng_handler.Backward(ui);
        }

        public void OnCloseAllShowedUI()
        {
            if (m_ui_layer_mng_handler != null)
                m_ui_layer_mng_handler.OnCloseAllShowedUI();
        }

        #region internal
        void InitLayers()
        {
            int depth = 2;
            int i = 0;
            foreach (int one_layer in Enum.GetValues(typeof(UILayer)))
            {
                string layer_name = "UI"+Enum.GetName(typeof(UILayer), one_layer);
                GameObject layer_obj = new GameObject();
                layer_obj.layer = LayerMask.NameToLayer(layer_name);
                layer_obj.transform.parent = GetUIRoot().transform;
                layer_obj.transform.position = Vector3.zero;
                layer_obj.transform.eulerAngles = Vector3.zero;
                layer_obj.transform.localScale = Vector3.one;
                layer_obj.name = layer_name;

                //放置相机
                GameObject camera_obj = DemoUnityResourceManager<GameObject>.Instance.AllocResource(UI_CAMERA_PREFAB_PATH);
                camera_obj.layer = LayerMask.NameToLayer(layer_name);
                camera_obj.transform.parent = layer_obj.transform;
                camera_obj.transform.position = Vector3.zero;
                camera_obj.transform.eulerAngles = Vector3.zero;
                camera_obj.transform.localScale = Vector3.one;
                camera_obj.name = UI_CAMERA_NAME;
                Camera camera = camera_obj.GetComponent<Camera>();
                camera.depth = depth + i;
                camera.cullingMask = 1 << LayerMask.NameToLayer(layer_name);

                m_layer2transfom[one_layer] = camera_obj.transform;

                i += 10;
            }

            InitPopupLayer();
        }
        void InitCameras()
        {
            InterceptiveCameras = new List<Camera>();
            Transform ui_camera_trans = GetParentByLayer(UILayer.DefaultLayer);
            if (ui_camera_trans != null)
            {
                Camera camera = ui_camera_trans.gameObject.GetComponent<Camera>();
                InterceptiveCameras.Add(camera);
            }
        }

        static GameObject GetUIRoot()
        {
            if (ms_ui_root == null)
            {
                string gui_name = "UIRoot";
                ms_ui_root = GameObject.Find(gui_name);
                if (ms_ui_root == null)
                {
                    GameObject prefab = Resources.Load("UI/Common/" + gui_name, typeof(GameObject)) as GameObject;
                    ms_ui_root = Instantiate(prefab) as GameObject;
                    ms_ui_root.name = gui_name;
                }
            }
            return ms_ui_root;
        }

        void InitPopupLayer()
        {
            Transform ui_camera_trans = GetParentByLayer(UILayer.SystemPopupLayer);
            GameObject msg_box_obj = DemoUnityResourceManager<GameObject>.Instance.AllocResource(UI_MESSAGE_BOX_PREFAB_PATH);
            msg_box_obj.transform.parent = ui_camera_trans;
            msg_box_obj.transform.name = "UIMessageBox";
            msg_box_obj.transform.localPosition = Vector3.zero;
            msg_box_obj.transform.localEulerAngles = Vector3.zero;
            msg_box_obj.transform.localScale = Vector3.one;

            UIMessageBox.SetShow(false);
        }
        #endregion
    }
}