using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using YUIFramework;

/*
 * 普通UI:可以在不关闭的情况下打开多个，动态控制的层级显示
 * 状态主UI：当前只能有一个状态主UI，一般是全屏，可以带附属UI。主UI打开时，所有其他UI全部关闭，除了指定的几个附属UI（考虑到UI的公用性，一个UI可以由多块UI组成）
 * 名字规范：AssetBundle的名字 == 界面GameObject的名字 == 脚本的名字
 * 
 */
#if USE_NGUI
public class UIBase : UIInputListener, IUIBase, INGUIInterface
#else
public class UIBase : UIInputListener, IUIBase
#endif
{
#region 界面实例公开数据，Unity可编辑
    public UILayer m_layer = UILayer.LayerDefault;
    // 是否是状态UI
    public bool m_is_main_wnd = false;
    // 主UI的附件UI，显示在主UI上面（之所以有附件UI，是因为附件UI可能会在多个主UI上显示）
    public List<string> m_mate_ui_list = new List<string>();
    // 显示黑色遮罩
    public bool m_show_mask = false;
#endregion

#region 动态数据
    bool m_is_show = true;
    #endregion

    #region INGUIInterface

#if USE_NGUI
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
#endif

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
            ui_mgr.DestroyUI(Name);
    }

#region UI提供的接口

    public static void ShowUI(string ui_name, object data = null)
    {
        UIManager.Instance.ShowUI(ui_name, data);
    }
    public static void HideUI(string ui_name)
    {
        UIManager.Instance.HideUI(ui_name);
    }
    public static bool IsShow(string ui_name)
    {
        return UIManager.Instance.IsShow(ui_name);
    }

    public void CallUIMethod(string ui_name, string method_name, object value)
    {
        UIManager.Instance.CallUIMethod(ui_name, method_name, value);
    }
    public void CallUIMethod(string ui_name, string method_name)
    {
        UIManager.Instance.CallUIMethod(ui_name, method_name);
    }

    public Transform GetUIParent()
    {
        return UIManager.Instance.GetParentByLayer(m_layer);
    }
    public GameObject GetObjectInAssetBundle(string obj_name)
    {
        return UIManager.Instance.GetObjectInAssetBundle(Name, obj_name);
    }
#endregion

#region IUIBase
    public bool IsShow()
    {
        return m_is_show;
    }

    public virtual void OnShow(object data) { }
    public virtual void OnHide() { }

    /*面板打开前两种加载数据的方式,通过UI注册时变量m_load_data_before_show(显示之前先加载数据，通过成功与否判断是否打开UI)来控制
     * 一般用于请求服务器
     * 1、LoadData --> OnShow --> UpdateUIOnShow 提前加载数据，根据加载结果，成功则打开UI刷新；失败则不打开UI
     * 2、OnShow --> UpdateUIByDefaultDataOnShow --> LoadData --> UpdateUIOnShow 先打开UI用默认数据填充，然后加载数据，最后刷新UI
     * 注：LoadData的结果需要设置到UIAsyncRequestResult
     */
    public virtual IEnumerator LoadData(UIAsyncRequestResult res) { yield break; }
    public virtual void UpdateUIByDefaultDataOnShow() { }
    public virtual void UpdateUIOnShow() { }

    public bool IsStateUI
    {
        get { return m_is_main_wnd; }
    }
    public string Name
    {
        get { return name; }
    }
    public GameObject GameObject
    {
        get { return this.gameObject; }
    }
    public List<string> MateUIList
    {
        get { return m_mate_ui_list; }
    }
#endregion

#region Mask
    void OnBtnClickMask(GameObject button)
    {
        UIManager.Instance.HideUI(this);
    }
#endregion

#region internal
    protected void LockUI(string lock_type)
    {
        UIManager.Instance.LockUI(lock_type);
    }
    protected void UnLockUI(string lock_type)
    {
        UIManager.Instance.UnLockUI(lock_type);
    }
    protected void UnLockAllUI()
    {
        UIManager.Instance.UnLockAllUI();
    }
    protected void Close()
    {
        UIManager.Instance.HideUI(this);
    }

    //一般用于自毁
    protected void Destroy()
    {
        UIManager.Instance.HideUI(this);
        GameObject.Destroy(gameObject);
    }

#endregion
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

public class UIAsyncRequestResult : IRecyclable
{
    public bool Success
    {
        get; set;
    }

    public void Reset()
    {

    }
}