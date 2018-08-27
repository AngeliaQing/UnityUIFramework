using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public class NGUILayerManagerHandler : UILayerManagerHandler
    {
        // 所有打开的UI的所有UIPanel的depth
        static List<int> ms_all_depths = new List<int>();
        // 当前最顶层的UIPanel的深度
        static int ms_cur_top_depth = MAIN_UI_MAX_DEPTH;

        #region 常量
        // 主UI内的所有UIPanel的depth必须小于MAIN_UI_MAX_DEPTH，非主UI的UIPanel的depth会调整为从MAIN_UI_MAX_DEPTH起
        const int MAIN_UI_MAX_DEPTH = 20;
        const string UI_CAMERA_NAME = "UICamera";
        const string UI_CAMERA_PREFAB_PATH = "UI/Common/NGUI/UICamera";
        const string UI_MESSAGE_BOX_PREFAB_PATH = "UI/Common/NGUI/NGUIMessageBox";
        #endregion

        public override void Init(List<Camera> interceptive_cameras)
        {
            m_ui_root_asset = "UI/Common/NGUI/NGUIRoot";
            InitLayers();
            InitCameras(interceptive_cameras);
        }
        public override void Forward(IUIBase ui)
        {
            if (ui == null)
                return;
            INGUIInterface ngui_base = ui as INGUIInterface;
            if (ngui_base == null)
                return;
            
            ms_all_depths.Sort((s1, s2) => s2 - s1);
            if (ms_all_depths.Count > 0)
                ms_cur_top_depth = ms_all_depths[0];
            for (int i = 0; i < ngui_base.Panels.Length; ++i)
            {
                ngui_base.Panels[i].depth = ms_cur_top_depth + i + 1;
                ms_all_depths.Add(ngui_base.Panels[i].depth);
            }
            ms_cur_top_depth = ms_cur_top_depth + ngui_base.Panels.Length;
        }
        public override void Backward(IUIBase ui)
        {
            if (ui == null)
                return;
            INGUIInterface ngui_base = ui as INGUIInterface;
            if (ngui_base == null)
                return;

            for (int i = 0; i < ngui_base.Panels.Length; i++)
            {
                ms_all_depths.Remove(ngui_base.Panels[i].depth);
                ngui_base.Panels[i].depth = i;
            }
            if (ms_all_depths.Count > 0)
                ms_cur_top_depth = ms_all_depths[0];
            else
                ms_cur_top_depth = MAIN_UI_MAX_DEPTH;
        }

        public override void OnCloseAllShowedUI()
        {
            ms_all_depths.Clear();
            ms_cur_top_depth = MAIN_UI_MAX_DEPTH;
        }

        

        #region internal
        void InitLayers()
        {
            int depth = 2;
            int i = 0;
            foreach (int one_layer in Enum.GetValues(typeof(UILayer)))
            {
                string hierarchy_layer_name = Enum.GetName(typeof(UILayer), one_layer);
                string layer_name = "UI" + hierarchy_layer_name;
                GameObject layer_obj = new GameObject();
                layer_obj.layer = LayerMask.NameToLayer(layer_name);
                layer_obj.transform.parent = GetUIRoot();
                layer_obj.transform.localPosition = Vector3.zero;
                layer_obj.transform.localEulerAngles = Vector3.zero;
                layer_obj.transform.localScale = Vector3.one;
                layer_obj.name = hierarchy_layer_name;

                //放置相机
                GameObject camera_obj = UnityResourceManager<GameObject>.Instance.AllocResource(UI_CAMERA_PREFAB_PATH);
                camera_obj.layer = LayerMask.NameToLayer(layer_name);
                camera_obj.transform.parent = layer_obj.transform;
                camera_obj.transform.localPosition = Vector3.zero;
                camera_obj.transform.localEulerAngles = Vector3.zero;
                camera_obj.transform.localScale = Vector3.one;
                camera_obj.name = UI_CAMERA_NAME;
                Camera camera = camera_obj.GetComponent<Camera>();
                camera.depth = depth + i;
                camera.cullingMask = 1 << LayerMask.NameToLayer(layer_name);
                UICamera ui_camera = camera_obj.GetComponent<UICamera>();
                ui_camera.eventReceiverMask = 1 << LayerMask.NameToLayer(layer_name);

                m_layer2transfom[one_layer] = camera_obj.transform;

                i += 10;
            }

            InitPopupLayer();
        }
        void InitPopupLayer()
        {
            Transform ui_camera_trans = GetParentByLayer(UILayer.LayerSystemPopup);
            GameObject msg_box_obj = UnityResourceManager<GameObject>.Instance.AllocResource(UI_MESSAGE_BOX_PREFAB_PATH);
            msg_box_obj.transform.parent = ui_camera_trans;
            msg_box_obj.transform.name = "UIMessageBox";
            msg_box_obj.transform.localPosition = Vector3.zero;
            msg_box_obj.transform.localEulerAngles = Vector3.zero;
            msg_box_obj.transform.localScale = Vector3.one;

            m_ui_msg_box = msg_box_obj.GetComponent<NGUIMessageBox>();
            UIHelper.SetActive(msg_box_obj, false);//UIMessageBox.SetShow(false);
        }

        void InitCameras(List<Camera> interceptive_cameras)
        {
            if(interceptive_cameras == null)
                interceptive_cameras = new List<Camera>();
            interceptive_cameras.Clear();

            Transform ui_camera_trans = GetParentByLayer(UILayer.LayerDefault);
            if (ui_camera_trans != null)
            {
                Camera camera = ui_camera_trans.gameObject.GetComponent<Camera>();
                interceptive_cameras.Add(camera);
            }
        }

        
        #endregion
    }
}