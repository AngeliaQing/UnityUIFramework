using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UILayer
{
    DefaultLayer,
    SystemPopupLayer,

}

public class UIRegisterInfo
{
    public string m_ui_name;
    public UILayer m_layer = UILayer.DefaultLayer;
    public string m_prefab_path;
}

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    IUIResourceMgr m_ui_res_mgr;
    UILayerManager m_layer_manager;
    UIStackManager m_stack_manage;

    void Awake()
    {
        m_ui_res_mgr = new UIPrefabMgr();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_ui_res_mgr.ClearLoadedResourse();
    }

    public void LoadUI(string ui_name)
    {
        m_ui_res_mgr.LoadUI(ui_name, DoNothingWhenLoaded);
    }

    public UIBase GetUI(string ui_name)
    {
        if (IsShow(ui_name))
            return m_ui_res_mgr.GetLoadedUI(ui_name);
        return null;
    }
    public UIBase ForceGetUI(string ui_name, string ui_dir_path = "")
    {
        UIBase ui = m_ui_res_mgr.GetLoadedUI(ui_name);
        if (ui == null)
        {
            m_ui_res_mgr.LoadUI(ui_name, null, ui_dir_path);
            ui = m_ui_res_mgr.GetLoadedUI(ui_name);
            ui.Hide();
        }
        return ui;
    }

    public void ShowUI(string ui_name, string ui_dir_path = "")
    {
        if (ShowLoadedUI(ui_name))
            return;
        m_ui_res_mgr.LoadUI(ui_name, ShowUIWhenLoaded, ui_dir_path);
    }
    public void HideUI(string ui_name)
    {
        HideLoadedUI(ui_name);
    }

    public bool IsLoaded(string ui_name)
    {
        return m_ui_res_mgr.GetLoadedUI(ui_name) != null;
    }

    public bool ShowLoadedUI(string ui_name)
    {
        UIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
        if (ui_base != null)
        {
            ui_base.Show();
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool HideLoadedUI(string ui_name)
    {
        UIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
        if (ui_base != null)
        {
            ui_base.Hide();
            return true;
        }
        else
        {
            return false;
        }
    }

    void DoNothingWhenLoaded(UIBase ui_base)
    {
    }
    void ShowUIWhenLoaded(UIBase ui_base)
    {
        if (ui_base != null)
        {
            ui_base.InitializeUIBase();
            ui_base.Show();
        }
        else
        {
            Debug.LogError("ShowUIWhenLoaded, null");
        }
    }

    public bool IsShow(string ui_name)
    {
        UIBase ui_base = m_ui_res_mgr.GetLoadedUI(ui_name);
        if (ui_base != null)
        {
            return ui_base.IsShow();
        }
        else
        {
            return false;
        }
    }

    public GameObject GetObjectInAssetBundle(string ui_name, string obj_name)
    {
        return m_ui_res_mgr.GetObjectInAssetBundle(ui_name, obj_name);
    }

    public void DestroyUI(string ui_name)
    {
        m_ui_res_mgr.DestroyUI(ui_name);
    }
}