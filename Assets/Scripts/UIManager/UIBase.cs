using UnityEngine;
using System;
using System.Collections.Generic;

interface IUIBase
{
    void OnShow();
    void OnHide();
}

/*
 * 普通UI:可以在前一个UI的基础上打开新的UI界面
 * 模态UI：在普通UI的基础上，添加了不能处理下层UI的操作，设置show_mask为true
 * 主UI：独占型UI。主UI打开时，所有其他UI全部关闭，除了指定的几个UI（考虑到UI的公用性，一个UI可以由多块UI组成）
 * 名字规范：AssetBundle的名字 == 界面GameObject的名字 == 脚本的名字
 * 
 * UIRoot：NGUI的UI Root 默认放在UI/Common下
 */
public class UIBase : UIInputListener, IUIBase
{
    #region 静态
    const string UI_BLACK_MASK_ASSET = "UI/Common/UIBlackMask";
    // NGUI的UIRoot
    static GameObject ms_ui_root;
    static GameObject GetUIRoot()
    {
        if(ms_ui_root == null)
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

    // 主UI内的所有UIPanel的depth必须小于MAIN_UI_MAX_DEPTH，非主UI的UIPanel的depth会调整为从MAIN_UI_MAX_DEPTH起
    const int MAIN_UI_MAX_DEPTH = 20;
    // 当前的主UI
    static UIBase ms_cur_main_ui = null;
    // 当前最顶层的UIPanel的深度
    static int ms_cur_top_depth = MAIN_UI_MAX_DEPTH;
    // 打开的非主UI的数量
    static int ms_opened_ui_count = 0;
    // 所有打开的UI的所有UIPanel的depth
    static List<int> ms_all_depths = new List<int>();

    // 当前显示的UI（不管是否是主UI）
    static Dictionary<string, UIBase> ms_showed_ui = new Dictionary<string, UIBase>();
    // 主UI栈（这个栈用于实现这样的功能：先显示A，当B显示时主A隐藏，当关闭B时A显示）
    static Stack<UIBase> ms_main_ui_stack = new Stack<UIBase>();

    static Transform m_ui_parent;
    public static Transform GetUIParent()
    {
        if (m_ui_parent == null)
        {
            GameObject ui_root = GetUIRoot();
            if (ui_root == null)
            {
                Debug.LogError("Can't Find UIRoot");
                return null;
            }
            m_ui_parent = ui_root.transform.Find("UICamera");
        }
        return m_ui_parent;
    }

    public static void CloseAllShowedUI()
    {
        foreach (UIBase ui_base in ms_showed_ui.Values)
        {
            if (ui_base.IsShow())
                ui_base.HideSelf();  // 不调Hide，Hide有处理主UI栈
        }
        ms_all_depths.Clear();
        ms_showed_ui.Clear();
        ms_cur_top_depth = MAIN_UI_MAX_DEPTH;
    }

    static void PushMainUI(UIBase ui_base)
    {
        ms_main_ui_stack.Push(ui_base);
    }
    static UIBase PopMainUI(UIBase ui_base)
    {
        if (ms_cur_main_ui == ui_base && ms_main_ui_stack.Count > 0)
            return ms_main_ui_stack.Pop();
        return null;
    }
    static void ClearUIStack()
    {
        // 应该在LoadLevel时调用
        CloseAllShowedUI();
        ms_main_ui_stack.Clear();
        ms_cur_main_ui = null;
    }
    #endregion

    #region 界面实例公开数据，Unity可编辑
    // 是否是主UI,独占型窗口
    public bool m_is_main_wnd = false;
    // 是否是系统提示框
    public bool m_is_system_or_alert = false;
    // 主UI的附件UI，显示在主UI上面（之所以有附件UI，是因为附件UI可能会在多个主UI上显示）
    public List<string> m_mate_ui = new List<string>();
    // 显示黑色遮罩
	public bool show_mask = false;
    // 相机层级
    //public int m_camera_type = GameManager.UICameraType_Default;
    #endregion

    #region 界面实例私有数据
    bool m_is_show = true;
    UIPanel[] m_panels;
    UIPanel[] Panels
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
    string UIName
    {
        get { return name; }
    }
    #endregion
    

    void Awake()
    {
        m_is_show = false;
        if (show_mask)
            AddMask();
    }
    protected void OnDestroy()
    {
        //base.OnDestroy();
        UIManager ui_mgr = UIManager.Instance;
        if(ui_mgr)
            ui_mgr.DestroyUI(UIName);
    }
    void OnEnable()
    {
        OnShow();
    }
    void OnDisable()
    {
        OnHide();
    }

    public void InitializeUIBase()
    {
        if (m_is_show)
            OnEnable();
    }
    public GameObject GetObjectInAssetBundle(string obj_name)
    {
        return UIManager.Instance.GetObjectInAssetBundle(UIName, obj_name);
    }

    public string GetUIName()
    {
        return UIName;
    }
    public bool IsShow()
    {
        return m_is_show;
    }
    public void Show()
    {
        if (m_is_show)
            return;
        if (m_is_main_wnd)
        {
            if (ms_cur_main_ui != null && ms_cur_main_ui != this)
            {
                PushMainUI(ms_cur_main_ui);
            }
            ms_cur_main_ui = this;
            //CloseOtherUI(ms_cur_main_ui);
            CloseAllShowedUI();
            foreach (string mate_ui in m_mate_ui)
            {
                UIManager.Instance.ShowUI(mate_ui);
                //bool result = UIManager.Instance.ShowLoadedUI(mate_ui);
                //if(!result)
                //    FrameworkUtility.LogError("UIBase.Show(), " + UIName + "'s mate UI(" + mate_ui + ") has not been loaded! ");
            }
        }
        ShowSelf();
        ms_showed_ui[UIName] = this;
    }
    void ShowSelf()
    {
        gameObject.SetActive(true);  // 触发OnEnable
        m_is_show = true;
        if (!m_is_main_wnd && !m_is_system_or_alert)
            Forward();
    }
    public void Hide()
    {
        if (!m_is_show)
            return;
        HideSelf();
        ms_showed_ui.Remove(UIName);
        if (m_is_main_wnd)
        {
            ms_cur_main_ui = PopMainUI(this);
            if (ms_cur_main_ui != null)
                ms_cur_main_ui.Show();
        }
    }
    void HideSelf()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);  // 触发OnDisable
            m_is_show = false;
            if (!m_is_main_wnd && !m_is_system_or_alert)
                Backward();
        }
    }
    void Forward()
    {
        ms_opened_ui_count++;
        ms_all_depths.Sort((s1, s2) => s2 - s1);
        if (ms_all_depths.Count > 0)
            ms_cur_top_depth = ms_all_depths[0];
        for (int i = 0; i < Panels.Length; ++i)
        {
            m_panels[i].depth = ms_cur_top_depth + i + 1;
            ms_all_depths.Add(m_panels[i].depth);
        }
        ms_cur_top_depth = ms_cur_top_depth + Panels.Length;
    }
    public void Backward()
    {
        ms_opened_ui_count--;
        for (int i = 0; i < Panels.Length; i++)
        {
            ms_all_depths.Remove(Panels[i].depth);
            Panels[i].depth = i;
        }
        if (ms_all_depths.Count > 0)
            ms_cur_top_depth = ms_all_depths[0];
        else
            ms_cur_top_depth = MAIN_UI_MAX_DEPTH;
    }

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

    #region pub
    public static void ShowUI(string ui_name, string ui_dir_path = "")
    {
        UIManager.Instance.ShowUI(ui_name, ui_dir_path);
    }
    public static void HideUI(string ui_name)
    {
        UIManager.Instance.HideUI(ui_name);
    }
    public static bool IsShow(string ui_name)
    {
        return UIManager.Instance.IsShow(ui_name);
    }
    public virtual void OnShow() { }
    public virtual void OnHide() { }
    public void Destroy()
    {
        Hide();
        GameObject.Destroy(gameObject);
    }
    #endregion
    void AddMask()
    {
        GameObject mask_obj = DemoUnityResourceManager<GameObject>.Instance.AllocResource(UI_BLACK_MASK_ASSET);
        mask_obj.transform.parent = gameObject.transform;
        mask_obj.transform.localPosition = Vector3.zero;
        mask_obj.transform.localScale = Vector3.one;
        mask_obj.transform.localEulerAngles = Vector3.zero;

        UISprite sprite = mask_obj.GetComponent<UISprite>();
        sprite.SetAnchor(gameObject, 0, 0, 0, 0);
        mask_obj.SetActive(true);
        gameObject.SetActive(false);

        UIEventListener.Get(mask_obj).onClick = OnBtnClickMask;
    }
    void OnBtnClickMask(GameObject button)
    {
        Hide();
    }
}
