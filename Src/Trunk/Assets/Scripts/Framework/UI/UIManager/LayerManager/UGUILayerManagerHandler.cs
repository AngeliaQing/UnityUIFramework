using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public class UGUILayerManagerHandler : UILayerManagerHandler
    {
        #region 常量
        const string UI_MESSAGE_BOX_PREFAB_PATH = "UI/Common/UGUI/UGUIMessageBox";
        #endregion

        public override void Init(List<Camera> interceptive_cameras)
        {
            m_ui_root_asset = "UI/Common/UGUI/UGUIRoot";
            InitLayers();
        }
        public override void Forward(IUIBase ui)
        {
            if (ui == null)
                return;
            UIBase ui_base = ui as UIBase;
            if (ui_base == null)
                return;
            RectTransform rt = ui_base.gameObject.GetComponent<RectTransform>();
            if(rt != null)
            {
                rt.localScale = Vector3.one;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector3.one;

                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector3.zero;
                rt.SetAsLastSibling();
            }
        }

        #region internal
        void InitLayers()
        {
            foreach (int one_layer in Enum.GetValues(typeof(UILayer)))
            {
                string hierarchy_layer_name = Enum.GetName(typeof(UILayer), one_layer);
                string layer_name = "UI" + hierarchy_layer_name;
                GameObject layer_obj = new GameObject();
                layer_obj.layer = LayerMask.NameToLayer(layer_name);
                layer_obj.transform.SetParent(GetUIRoot());
                layer_obj.transform.localPosition = Vector3.zero;
                layer_obj.transform.localEulerAngles = Vector3.zero;
                layer_obj.transform.localScale = Vector3.one;
                layer_obj.name = hierarchy_layer_name;

                RectTransform rt = layer_obj.AddComponent<RectTransform>();
                rt.localScale = Vector3.one;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector3.zero;
                rt.SetAsLastSibling();

                m_layer2transfom[one_layer] = layer_obj.transform;
                
            }

            InitPopupLayer();
        }
        void InitPopupLayer()
        {
            Transform parent_trans = GetParentByLayer(UILayer.LayerSystemPopup);
            GameObject msg_box_obj = DemoUnityResourceManager<GameObject>.Instance.AllocResource(UI_MESSAGE_BOX_PREFAB_PATH);
            msg_box_obj.transform.SetParent(parent_trans);
            msg_box_obj.transform.name = "UIMessageBox";
            msg_box_obj.transform.localPosition = Vector3.zero;
            msg_box_obj.transform.localEulerAngles = Vector3.zero;
            msg_box_obj.transform.localScale = Vector3.one;

            RectTransform rt = msg_box_obj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector3.zero;
            m_ui_msg_box = msg_box_obj.GetComponent<UGUIMessageBox>();

            UIHelper.SetActive(msg_box_obj, false);
        }
        #endregion
    }
}