using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{

    [RequireComponent(typeof(UIRegisterManager))]
    [RequireComponent(typeof(UILayerManager))]
    [RequireComponent(typeof(UIStackManager))]
    [RequireComponent(typeof(UIMaskManager))]

    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        IUIResourceMgr m_ui_res_mgr;
        static UIRegisterManager ms_register_manager;
        static UILayerManager ms_layer_manager;
        static UIStackManager ms_stack_manager;
        static UIMaskManager ms_mask_manager;

        // 打开的非状态UI的数量
        static int ms_opened_ui_count = 0;

        // 当前显示的UI（不管是否是状态UI）
        static Dictionary<string, IUIBase> ms_showed_ui = new Dictionary<string, IUIBase>();


        void Awake()
        {
            m_ui_res_mgr = new UIPrefabMgr();
            ms_register_manager = gameObject.GetComponent<UIRegisterManager>();
            ms_layer_manager = gameObject.GetComponent<UILayerManager>();
            ms_stack_manager = gameObject.GetComponent<UIStackManager>();
            ms_mask_manager = gameObject.GetComponent<UIMaskManager>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_ui_res_mgr.ClearLoadedResourse();
        }

        #region UI加载
        public bool IsLoaded(string ui_name)
        {
            return m_ui_res_mgr.GetLoadedUI(ui_name) != null;
        }

        public void LoadUI(string ui_name)
        {
            m_ui_res_mgr.LoadUI(ui_name, DoNothingWhenLoaded);
        }

        public IUIBase GetUI(string ui_name)
        {
            if (IsShow(ui_name))
                return m_ui_res_mgr.GetLoadedUI(ui_name);
            return null;
        }
        public IUIBase ForceGetUI(string ui_name, string ui_dir_path = "")
        {
            IUIBase ui = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui == null)
            {
                m_ui_res_mgr.LoadUI(ui_name, null, ui_dir_path);
                ui = m_ui_res_mgr.GetLoadedUI(ui_name);
                HideUI(ui);
            }
            return ui;
        }

        public GameObject GetObjectInAssetBundle(string ui_name, string obj_name)
        {
            return m_ui_res_mgr.GetObjectInAssetBundle(ui_name, obj_name);
        }

        void DoNothingWhenLoaded(IUIBase ui_base)
        {
        }
        #endregion

        #region UI生命周期控制

        public bool IsShow(string ui_name)
        {
            IUIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui_base != null)
            {
                return ui_base.IsShow();
            }
            else
            {
                return false;
            }
        }

        public void ShowUI(string ui_name)
        {
            if (ShowLoadedUI(ui_name))
                return;
            string ui_path = "";
            if (ms_register_manager != null)
                ui_path = ms_register_manager.GetUIPath(ui_name);
            m_ui_res_mgr.LoadUI(ui_name, ShowUIWhenLoaded, ui_path);
        }
        public bool ShowLoadedUI(string ui_name)
        {
            IUIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui_base != null)
            {
                ShowUI(ui_base);
                return true;
            }
            else
            {
                return false;
            }
        }

        void ShowUIWhenLoaded(IUIBase ui_base)
        {
            if (ui_base != null)
            {
                //ui_base.InitializeUIBase();
                ShowUI(ui_base);
            }
            else
            {
                Debug.LogError("ShowUIWhenLoaded, null");
            }
        }

        void ShowUI(IUIBase i_ui_base)
        {
            if (i_ui_base == null)
                return;
            if (i_ui_base.IsShow())
                return;
            if (i_ui_base.IsStateUI())
            {
                if (ms_stack_manager != null)
                    ms_stack_manager.OnShowMainUI(i_ui_base);
                CloseAllShowedUI();

                i_ui_base.ShowMateUI();
            }
            i_ui_base.ShowSelf();
            ms_showed_ui[i_ui_base.UIName] = i_ui_base;
        }
        public void HideUI(string ui_name)
        {
            HideLoadedUI(ui_name);
        }
        public bool HideLoadedUI(string ui_name)
        {
            IUIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui_base != null)
            {
                HideUI(ui_base);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void HideUI(IUIBase i_ui_base)
        {
            if (i_ui_base == null)
                return;
            if (!i_ui_base.IsShow())
                return;
            i_ui_base.HideSelf();
            ms_showed_ui.Remove(i_ui_base.UIName);
            if (i_ui_base.IsStateUI())
            {
                IUIBase cur_main_ui = null;
                if (ms_stack_manager != null)
                    cur_main_ui = ms_stack_manager.OnHideMainUI(i_ui_base);
                ShowUI(cur_main_ui);
            }

        }

        public void DestroyUI(string ui_name)
        {
            m_ui_res_mgr.DestroyUI(ui_name);
        }

        public static void CloseAllShowedUI()
        {
            foreach (UIBase ui_base in ms_showed_ui.Values)
            {
                if (ui_base.IsShow())
                    ui_base.HideSelf();
            }
           
            ms_showed_ui.Clear();
            if (ms_layer_manager != null)
                ms_layer_manager.OnCloseAllShowedUI();
        }
        #endregion

        #region 层管理
        public Transform GetParentByLayer(UILayer ui_layer)
        {
            if (ms_layer_manager != null)
                return ms_layer_manager.GetParentByLayer(ui_layer);
            return null;
        }

        public void Forward(IUIBase ui)
        {
            if (ui == null)
                return;
            ms_opened_ui_count++;
            if (ms_layer_manager != null)
                ms_layer_manager.Forward(ui);
        }
        public void Backward(IUIBase ui)
        {
            if (ui == null)
                return;
            ms_opened_ui_count--;
            if (ms_layer_manager != null)
                ms_layer_manager.Backward(ui);
        }

        public List<Camera> InterceptiveCameras
        {
            get
            {
                if (ms_layer_manager != null)
                    return ms_layer_manager.InterceptiveCameras;
                return null;
            }
        }
        #endregion

        #region 状态UI管理
        static void ClearUIStack()
        {
            // 应该在LoadLevel时调用
            CloseAllShowedUI();
            if(ms_stack_manager != null)
                ms_stack_manager.ClearUIStack();
        }
        #endregion

        #region MASK管理
        public void AddMask(IUIBase ui_base, UIEventListener.VoidDelegate call_back)
        {
            if(ms_mask_manager != null)
            {
                ms_mask_manager.AddMask(ui_base, call_back);
            }
        }
        #endregion
    }
}