using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public enum UILayer
    {
        LayerDefault,
        LayerSystemPopup,

    }

    public class UILayerManager : MonoBehaviour
    {
        IUILayerManagerHandler m_ui_layer_mng_handler;

        #region 常量
        #endregion

        void Awake()
        {
#if USE_NGUI
            m_ui_layer_mng_handler = new NGUILayerManagerHandler();
#else
            m_ui_layer_mng_handler = new UGUILayerManagerHandler();
#endif

            m_ui_layer_mng_handler.Init(InterceptiveCameras);
        }
        public Transform GetParentByLayer(UILayer layer)
        {
            if (m_ui_layer_mng_handler != null)
                return m_ui_layer_mng_handler.GetParentByLayer(layer);
            return null;
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

        public UIMessageBox GetUIMessageBox()
        {
            if (m_ui_layer_mng_handler != null)
                return m_ui_layer_mng_handler.GetUIMessageBox();
            return null;
        }

#region internal
#endregion
    }
}