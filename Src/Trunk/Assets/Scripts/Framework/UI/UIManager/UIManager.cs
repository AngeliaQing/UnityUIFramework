using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace YUIFramework
{

    [RequireComponent(typeof(UIRegister))]
    [RequireComponent(typeof(UILayerManager))]
    [RequireComponent(typeof(UIStackManager))]
    [RequireComponent(typeof(UIMaskManager))]

    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        IUIResourceMgr m_ui_res_mgr;
        static UIRegister ms_register_manager;
        static UILayerManager ms_layer_manager;
        static UIStackManager ms_stack_manager;
        static UIMaskManager ms_mask_manager;
        static UIPopupManager ms_popup_manager;
        static UILockManager ms_lock_manager;

        // 打开的非状态UI的数量
        static int ms_opened_ui_count = 0;

        // 当前显示的UI（不管是否是状态UI）
        static Dictionary<int, IUIBase> ms_showed_ui = new Dictionary<int, IUIBase>();

        #region 常量
        const string m_switch_lock_name = "__switch_lock__";
        #endregion


        void Awake()
        {
            m_ui_res_mgr = new UIPrefabMgr();
            ms_register_manager = gameObject.GetComponent<UIRegister>();
            ms_layer_manager = gameObject.GetComponent<UILayerManager>();
            ms_stack_manager = gameObject.GetComponent<UIStackManager>();
            ms_mask_manager = gameObject.GetComponent<UIMaskManager>();
            ms_popup_manager = new UIPopupManager();
            ms_lock_manager = new UILockManager();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_ui_res_mgr.ClearLoadedResourse();
        }

        #region UI加载
        public bool IsLoaded(UIName ui_name)
        {
            return m_ui_res_mgr.GetLoadedUI(ui_name) != null;
        }

        public void LoadUI(UIName ui_name)
        {
            m_ui_res_mgr.LoadUI(ui_name, DoNothingWhenLoaded);
        }

        public IUIBase GetUI(UIName ui_name)
        {
            if (IsShow(ui_name))
                return m_ui_res_mgr.GetLoadedUI(ui_name);
            return null;
        }
        public IUIBase ForceGetUI(UIName ui_name, string ui_dir_path = "")
        {
            IUIBase ui = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui == null)
            {
                m_ui_res_mgr.LoadUI(ui_name, null, ui_dir_path);
                ui = m_ui_res_mgr.GetLoadedUI(ui_name);
                HideUIAsync(ui);
            }
            return ui;
        }

        public GameObject GetObjectInAssetBundle(UIName ui_name, string obj_name)
        {
            return m_ui_res_mgr.GetObjectInAssetBundle(ui_name, obj_name);
        }

        void DoNothingWhenLoaded(IUIBase ui_base, object param = null)
        {
        }
        #endregion

        #region UI生命周期控制

        public bool IsShow(UIName ui_name)
        {
            return ms_showed_ui.ContainsKey((int)ui_name);
        }
        bool IsShow(IUIBase i_ui_base)
        {
            if (i_ui_base == null)
                return false;
            return IsShow(i_ui_base.Name);
        }

        //UI显示
        public void ShowUI(UIName ui_name, object data = null)
        {
            if (ShowLoadedUI(ui_name, data))
                return;
            string ui_path = "";
            if (ms_register_manager != null)
                ui_path = ms_register_manager.GetUIPath(ui_name);
            m_ui_res_mgr.LoadUI(ui_name, ShowUIWhenLoaded, ui_path, data);
        }
        bool ShowLoadedUI(UIName ui_name, object data = null)
        {
            IUIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui_base != null)
            {
                ShowUIAsync(ui_base, data);
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
                ShowUIAsync(ui_base, data);
            }
            else
            {
                Debug.LogError("ShowUIWhenLoaded, null");
            }
        }

        public void ShowUIAsync(IUIBase i_ui_base, object data = null)
        {
            if (i_ui_base == null)
                return;
            if (IsShow(i_ui_base))
                return;
            StartCoroutine(ShowUIWithLoadData(i_ui_base, data));
        }

        IEnumerator ShowUIWithLoadData(IUIBase i_ui_base, object data = null)
        {
            while (HasLockUI(i_ui_base, m_switch_lock_name))
                yield return 0;

            LockUI(i_ui_base, m_switch_lock_name);

            //加载数据相关
            if (NeedLoadDataBeforeShow(i_ui_base))
            {
                UIAsyncRequestResult res = RecyclableObject.Create<UIAsyncRequestResult>();
                res.Success = true;
                yield return i_ui_base.LoadData(res);
                if (!res.Success)
                {
                    RecyclableObject.Recycle(res);
                    UnLockUI(i_ui_base, m_switch_lock_name);
                    yield break;
                }
            }

            if (i_ui_base.IsStateUI)
            {
                if (ms_stack_manager != null)
                    ms_stack_manager.OnShowMainUI(i_ui_base);

                yield return CloseAllShowedUI();
            }

            yield return ShowUIBase(i_ui_base, data);
            ms_showed_ui[(int)i_ui_base.Name] = i_ui_base;

            //显示附属UI
            if (i_ui_base.IsStateUI)
            {
                for (int i = 0; i < i_ui_base.MateUIList.Count; ++i)
                {
                    ShowUI(i_ui_base.MateUIList[i]);
                }
            }

            //加载数据相关
            if (NeedLoadDataBeforeShow(i_ui_base))
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

            UnLockUI(i_ui_base, m_switch_lock_name);
        }
        IEnumerator ShowUIBase(IUIBase i_ui_base, object data = null)
        {
            if (i_ui_base == null)
                yield break;
            UIHelper.SetActive(i_ui_base.GameObject, true);
            i_ui_base.OnShow(data);
            if (!i_ui_base.IsStateUI)
                Forward(i_ui_base);

            yield return i_ui_base.PlayEnterAnim();
        }

        //UI隐藏
        public void HideUI(UIName ui_name)
        {
            HideLoadedUI(ui_name);
        }
        bool HideLoadedUI(UIName ui_name)
        {
            IUIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
            if (ui_base != null)
            {
                HideUIAsync(ui_base);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void HideUIAsync(IUIBase i_ui_base, Action on_hided = null)
        {
            if (i_ui_base == null)
                return;
            if (!IsShow(i_ui_base))
                return;
            StartCoroutine(HideUI(i_ui_base, on_hided));
        }
        IEnumerator HideUI(IUIBase i_ui_base, Action on_hided = null)
        {
            yield return HideUIBase(i_ui_base, on_hided);
            ms_showed_ui.Remove((int)i_ui_base.Name);

            if (i_ui_base.IsStateUI)
            {
                IUIBase cur_main_ui = null;
                if (ms_stack_manager != null)
                    cur_main_ui = ms_stack_manager.OnHideMainUI(i_ui_base);

                ShowUIAsync(cur_main_ui);
            }
        }
        IEnumerator HideUIBase(IUIBase i_ui_base, Action on_hided = null)
        {
            if (i_ui_base == null)
                yield break;
            yield return i_ui_base.PlayLeaveAnim();

            if (i_ui_base.GameObject.activeInHierarchy)
            {
                UIHelper.SetActive(i_ui_base.GameObject, false);
                i_ui_base.OnHide();
                if (!i_ui_base.IsStateUI)
                    Backward(i_ui_base);
            }

            if (on_hided != null)
                on_hided();
        }

        public void DestroyUI(UIName ui_name)
        {
            m_ui_res_mgr.DestroyUI(ui_name);
        }

        IEnumerator CloseAllShowedUI()
        {
            foreach (IUIBase ui_base in ms_showed_ui.Values)
            {
                if (IsShow(ui_base))
                    yield return HideUIBase(ui_base);
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
            if (ms_stack_manager != null)
                ms_stack_manager.ClearUIStack();
        }
        #endregion

        #region MASK管理
        public void AddMask(IUIBase ui_base, UIEventListener.VoidDelegate call_back)
        {
            if (ms_mask_manager != null)
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
        public void CallUIMethod(UIName ui_name, string method_name, object value)
        {
            IUIBase ui = GetUI(ui_name);
            if (ui == null)
                return;
            ui.GameObject.SendMessage(method_name, value);
        }
        public void CallUIMethod(UIName ui_name, string method_name)
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
        public UIMessageBox GetUIMessageBox()
        {
            if (ms_layer_manager != null)
                return ms_layer_manager.GetUIMessageBox();
            return null;
        }

        #endregion

        #region UI锁
        bool HasLockUI(IUIBase i_ui_base, string lock_type)
        {
            if (i_ui_base != null && i_ui_base.Name == UIName.UILock)
                return false;
            if (ms_lock_manager != null)
                return ms_lock_manager.HasLockUI(lock_type);
            return false;
        }
        public void LockUI(IUIBase i_ui_base, string lock_type)
        {
            if (i_ui_base != null && i_ui_base.Name == UIName.UILock)
                return;
            if (ms_lock_manager != null)
                ms_lock_manager.LockUI(lock_type);
        }
        public void UnLockUI(IUIBase i_ui_base, string lock_type)
        {
            if (i_ui_base != null && i_ui_base.Name == UIName.UILock)
                return;
            if (ms_lock_manager != null)
                ms_lock_manager.UnLockUI(lock_type);
        }
        public void UnLockAllUI()
        {
            if (ms_lock_manager != null)
                ms_lock_manager.UnLockAllUI();
        }
        #endregion
    }
}