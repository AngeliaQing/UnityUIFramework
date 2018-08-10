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
        static UIPopupManager ms_popup_manager;

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
            ms_popup_manager = new UIPopupManager();
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

        void DoNothingWhenLoaded(IUIBase ui_base, object param = null)
        {
        }
        #endregion

        #region UI生命周期控制

        public bool IsShow(string ui_name)
        {
            return ms_showed_ui.ContainsKey(ui_name);
        }
        bool IsShow(IUIBase i_ui_base)
        {
            if (i_ui_base == null)
                return false;
            return IsShow(i_ui_base.Name);
        }

        public void ShowUI(string ui_name, object data = null)
        {
            if (ShowLoadedUI(ui_name, data))
                return;
            string ui_path = "";
            if (ms_register_manager != null)
                ui_path = ms_register_manager.GetUIPath(ui_name);
            m_ui_res_mgr.LoadUI(ui_name, ShowUIWhenLoaded, ui_path, data);
        }
        bool ShowLoadedUI(string ui_name, object data = null)
        {
            IUIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui_base != null)
            {
                ShowUI(ui_base, data);
                return true;
            }
            else
            {
                return false;
            }
        }

        void ShowUIWhenLoaded(IUIBase ui_base, object data = null)
        {
            if (ui_base != null)
            {
                //ui_base.InitializeUIBase();
                ShowUI(ui_base, data);
            }
            else
            {
                Debug.LogError("ShowUIWhenLoaded, null");
            }
        }

        public void ShowUI(IUIBase i_ui_base, object data = null)
        {
            StartCoroutine(ShowUIWithLoadData(i_ui_base, data));
        }
        IEnumerator ShowUIWithLoadData(IUIBase i_ui_base, object data = null)
        {
            if (i_ui_base == null)
                yield break; ;
            if (IsShow(i_ui_base))
                yield break;

            //加载数据相关
            if(NeedLoadDataBeforeShow(i_ui_base))
            {
                UIAsyncRequestResult res = RecyclableObject.Create<UIAsyncRequestResult>();
                res.Success = true;
                yield return i_ui_base.LoadData(res);
                if (!res.Success)
                {
                    RecyclableObject.Recycle(res);
                    yield break;
                }
            }

            if (i_ui_base.IsStateUI)
            {
                if (ms_stack_manager != null)
                    ms_stack_manager.OnShowMainUI(i_ui_base);
                ms_stack_manager.PrintStack();
                CloseAllShowedUI();

                for (int i = 0; i < i_ui_base.MateUIList.Count; ++i)
                {
                    ShowUI(i_ui_base.MateUIList[i]);
                }
            }
            //i_ui_base.ShowSelf();
            ShowUIInternal(i_ui_base, data);
            ms_showed_ui[i_ui_base.Name] = i_ui_base;

            //加载数据相关
            if(NeedLoadDataBeforeShow(i_ui_base))
            {
                i_ui_base.UpdateUIOnShow();
            }
            else
            {
                i_ui_base.UpdateUIByDefaultDataOnShow();
                UIAsyncRequestResult res = RecyclableObject.Create<UIAsyncRequestResult>();
                res.Success = true;
                yield return i_ui_base.LoadData(res);
                RecyclableObject.Recycle(res);
                i_ui_base.UpdateUIOnShow();
            }
        }


        void ShowUIInternal(IUIBase i_ui_base, object data = null)
        {
            if (i_ui_base == null)
                return;
            UIHelper.SetActive(i_ui_base.GameObject, true);
            i_ui_base.OnShow(data);
            if (!i_ui_base.IsStateUI)
                Forward(i_ui_base);
        }

        public void HideUI(string ui_name)
        {
            HideLoadedUI(ui_name);
        }
        bool HideLoadedUI(string ui_name)
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
            if (!IsShow(i_ui_base))
                return;
            //i_ui_base.HideSelf();
            HideUIInternal(i_ui_base);
            ms_showed_ui.Remove(i_ui_base.Name);
            if (i_ui_base.IsStateUI)
            {
                IUIBase cur_main_ui = null;
                if (ms_stack_manager != null)
                    cur_main_ui = ms_stack_manager.OnHideMainUI(i_ui_base);

                ms_stack_manager.PrintStack();
                ShowUI(cur_main_ui);
            }
        }
        void HideUIInternal(IUIBase i_ui_base)
        {
            if (i_ui_base == null)
                return;
            if(i_ui_base.GameObject.activeInHierarchy)
            {
                UIHelper.SetActive(i_ui_base.GameObject, false);
                i_ui_base.OnHide();
                if (!i_ui_base.IsStateUI)
                    Backward(i_ui_base);
            }
        }

        public void DestroyUI(string ui_name)
        {
            m_ui_res_mgr.DestroyUI(ui_name);
        }

        void CloseAllShowedUI()
        {
            foreach (IUIBase ui_base in ms_showed_ui.Values)
            {
                if (IsShow(ui_base))
                    //ui_base.HideSelf();
                    HideUIInternal(ui_base);
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
        void ClearUIStack()
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

        #region 注册管理器
        bool NeedLoadDataBeforeShow(IUIBase ui)
        {
            if (ms_register_manager != null)
                return ms_register_manager.NeedLoadDataBeforeShow(ui);
            return false;
        }
        #endregion

        #region 打开的UI面板间通讯
        public void CallUIMethod(string ui_name, string method_name, object value)
        {
            IUIBase ui = GetUI(ui_name);
            if (ui == null)
                return;
            ui.GameObject.SendMessage(method_name, value);
        }
        public void CallUIMethod(string ui_name, string method_name)
        {
            IUIBase ui = GetUI(ui_name);
            if (ui == null)
                return;
            ui.GameObject.SendMessage(method_name);
        }
        #endregion

        #region 系统弹出式窗口
        public void OpenPopup(IUIPopupBase popup)
        {
            if (ms_popup_manager != null)
                ms_popup_manager.OpenPopup(popup);
        }
        public void ClosePopup(IUIPopupBase popup)
        {
            if (ms_popup_manager != null)
                ms_popup_manager.ClosePopup(popup);
        }
        #endregion
    }
}