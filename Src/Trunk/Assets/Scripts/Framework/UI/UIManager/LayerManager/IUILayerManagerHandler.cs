using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public interface IUILayerManagerHandler
    {
        void Init(List<Camera> interceptive_cameras);
        void Forward(IUIBase ui);
        void Backward(IUIBase ui);
        void OnCloseAllShowedUI();
        Transform GetParentByLayer(UILayer layer);
        UIMessageBox GetUIMessageBox();
    }

    public class UILayerManagerHandler : IUILayerManagerHandler
    {
        protected Dictionary<int, Transform> m_layer2transfom = new Dictionary<int, Transform>();
        protected string m_ui_root_asset;
        Transform m_ui_root;
        protected UIMessageBox m_ui_msg_box;

        public virtual void Init(List<Camera> interceptive_cameras) { }
        public virtual void Forward(IUIBase ui) { }
        public virtual void Backward(IUIBase ui) { }
        public virtual void OnCloseAllShowedUI() { }
        public virtual Transform GetParentByLayer(UILayer layer)
        {
            int i_layer = (int)layer;
            if (m_layer2transfom.ContainsKey(i_layer))
                return m_layer2transfom[i_layer];

            Debug.LogError("UILayerManager GetParentByLayer cannot find uilayer " + i_layer);
            return m_layer2transfom[(int)UILayer.LayerDefault];
        }
        public UIMessageBox GetUIMessageBox() { return m_ui_msg_box; }

        protected Transform GetUIRoot()
        {
            if (m_ui_root == null)
            {
                string gui_name = "UIRoot";
                GameObject obj = GameObject.Find(gui_name);
                if (obj == null)
                {
                    obj = DemoUnityResourceManager<GameObject>.Instance.AllocResource(m_ui_root_asset);
                    obj.name = gui_name;
                }
                m_ui_root = obj.transform;
            }
            return m_ui_root;
        }
    }
}