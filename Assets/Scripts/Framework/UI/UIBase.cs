using UnityEngine;
using System;
using System.Collections.Generic;
using YUIFramework;

/*
 * 普通UI:可以在前一个UI的基础上打开新的UI界面
 * 模态UI：在普通UI的基础上，添加了不能处理下层UI的操作，设置m_show_mask为true
 * 主UI：独占型UI。主UI打开时，所有其他UI全部关闭，除了指定的几个UI（考虑到UI的公用性，一个UI可以由多块UI组成）
 * 名字规范：AssetBundle的名字 == 界面GameObject的名字 == 脚本的名字
 * 
 * UIRoot：NGUI的UI Root 默认放在UI/Common下
 */
public class UIBase : UIInputListener, IUIBase, INGUIInterface
{
    #region 界面实例公开数据，Unity可编辑
    public UILayer m_layer = UILayer.DefaultLayer;
    // 是否是状态UI
    public bool m_is_main_wnd = false;
    // 主UI的附件UI，显示在主UI上面（之所以有附件UI，是因为附件UI可能会在多个主UI上显示）
    public List<string> m_mate_ui = new List<string>();
    // 显示黑色遮罩
    public bool m_show_mask = false;
    #endregion

    #region 动态数据
    bool m_is_show = true;
    #endregion

    #region INGUIInterface
    UIPanel[] m_panels;
    public UIPanel[] Panels
    {
        get
        {
            if (m_panels == null || m_panels.Length == 0)
            {
                m_panels = transform.GetComponentsInChildren<UIPanel>(true);
                Array.Sort(m_panels, (s1, s2) => s1.depth - s2.depth);
            }
            if (m_panels == null)
            {
                m_panels = new UIPanel[0];
            }
            return m_panels;
        }
    }
    #endregion

    public virtual void Awake()
    {
        m_is_show = false;
        if (m_show_mask)
            UIManager.Instance.AddMask(this, OnBtnClickMask);
    }
    protected void OnDestroy()
    {
        UIManager ui_mgr = UIManager.Instance;
        if(ui_mgr)
            ui_mgr.DestroyUI(UIName);
    }

    #region 附属UI
    public void ClearMateUI()
    {
        m_mate_ui.Clear();
    }
    public void AddMateUI(string ui_name)
    {
        if (m_mate_ui.IndexOf(ui_name) > 0)
            return;
        m_mate_ui.Add(ui_name);
    }
    public void RemoveMateUI(string ui_name)
    {
        m_mate_ui.Remove(ui_name);
    }
    #endregion

    #region UI的接口
    public static void ShowUI(string ui_name)
    {
        UIManager.Instance.ShowUI(ui_name);
    }
    public static void HideUI(string ui_name)
    {
        UIManager.Instance.HideUI(ui_name);
    }
    public static bool IsShow(string ui_name)
    {
        return UIManager.Instance.IsShow(ui_name);
    }

    public Transform GetUIParent()
    {
        return UIManager.Instance.GetParentByLayer(m_layer);
    }
    public GameObject GetObjectInAssetBundle(string obj_name)
    {
        return UIManager.Instance.GetObjectInAssetBundle(UIName, obj_name);
    }
    #endregion

    #region IUIBase
    public virtual void OnShow() { }
    public virtual void OnHide() { }

    //一般用于自毁
    public void Destroy()
    {
        UIManager.Instance.HideUI(this);
        GameObject.Destroy(gameObject);
    }

    public bool IsShow()
    {
        return m_is_show;
    }
    public bool IsStateUI()
    {
        return m_is_main_wnd;
    }

    public void ShowSelf()
    {
        UIHelper.SetActive(gameObject, true);  // 触发OnEnable
        OnShow();
        m_is_show = true;
        if (!m_is_main_wnd)// && !m_is_system_or_alert
            UIManager.Instance.Forward(this);
    }

    public void HideSelf()
    {
        if (gameObject.activeInHierarchy)
        {
            UIHelper.SetActive(gameObject, false);  // 触发OnDisable
            OnHide();
            m_is_show = false;
            if (!m_is_main_wnd)// && !m_is_system_or_alert
                UIManager.Instance.Backward(this);
        }
    }
    public void ShowMateUI()
    {
        foreach (string mate_ui in m_mate_ui)
        {
            UIManager.Instance.ShowUI(mate_ui);
            //bool result = UIManager.Instance.ShowLoadedUI(mate_ui);
            //if(!result)
            //    FrameworkUtility.LogError("UIBase.Show(), " + UIName + "'s mate UI(" + mate_ui + ") has not been loaded! ");
        }
    }


    public string UIName
    {
        get { return name; }
    }
    #endregion

    #region Mask
    void OnBtnClickMask(GameObject button)
    {
        UIManager.Instance.HideUI(this);
    }
    #endregion


    //public void InitializeUIBase()
    //{
    //    if (m_is_show)
    //        OnEnable();
    //}
    //public int CareCategory
    //{
    //    get { return InputCareCategory; }
    //    set { InputCareCategory = value; }
    //}
}


public class EasyTouchEventCategoty
{
    public const int ETEC_None = 0;
    public const int ETEC_All = (int)-1;
    public const int ETEC_Cancel = 1 << 0;
    public const int ETEC_Touch = 1 << 1;
    public const int ETEC_SimpleTap = 1 << 2;
    public const int ETEC_DoubleTap = 1 << 3;
    public const int ETEC_LongTap = 1 << 4;
    public const int ETEC_Drag = 1 << 5;
    public const int ETEC_Swipe = 1 << 6;
    public const int ETEC_Touch2Fingers = 1 << 7;
    public const int ETEC_SimpleTap2Fingers = 1 << 8;
    public const int ETEC_DoubleTap2Fingers = 1 << 9;
    public const int ETEC_LongTap2Fingers = 1 << 10;
    public const int ETEC_Twist = 1 << 11;
    public const int ETEC_Pinch = 1 << 12;
    public const int ETEC_Drag2Fingers = 1 << 13;
    public const int ETEC_Swipe2Fingers = 1 << 14;
}