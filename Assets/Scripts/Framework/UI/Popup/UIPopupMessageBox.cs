using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUIFramework;

public class UIPopupMessageBox : IUIPopupBase, IRecyclable
{
    #region 变量
    PopupPriority m_priority = PopupPriority.Invalid;
    PopupMsgBoxType m_msg_box_type = PopupMsgBoxType.Ok;
    string m_title = "";
    string m_text = "";

    string m_ok_text = null;
    BtnClickCallBack m_ok_callback = null;
    object m_ok_param = null;

    string m_cancel_text = null;
    BtnClickCallBack m_cancel_callback = null;
    object m_cancel_param = null;

    BtnClickCallBack m_close_callback = null;
    object m_close_param = null;
    #endregion


    #region 常量
    public const PopupPriority DefaultPriority = PopupPriority.Normal;
    #endregion

    public UIPopupMessageBox() { }

    #region 对外提出的接口
    //OK
    public static void Alert(string str_title, string str_text, PopupPriority priority = DefaultPriority)
    {
        UIPopupMessageBox msg_box = UIPopupMessageBox.CreatePopupMessageBox(priority);
        msg_box.Title = str_title;
        msg_box.Text = str_text;
        msg_box.MsgBoxType = PopupMsgBoxType.Ok;

        UIManager.Instance.OpenPopup(msg_box);
    }
    public static void Alert(string str_title, string str_text, string ok_text, PopupPriority priority = DefaultPriority)
    {
        UIPopupMessageBox msg_box = UIPopupMessageBox.CreatePopupMessageBox(priority);
        msg_box.Title = str_title;
        msg_box.Text = str_text;
        msg_box.OkText = ok_text;
        msg_box.MsgBoxType = PopupMsgBoxType.Ok;

        UIManager.Instance.OpenPopup(msg_box);
    }
    public static void Alert(string str_title, string str_text, BtnClickCallBack ok_callback, object ok_param, PopupPriority priority = DefaultPriority)
    {
        UIPopupMessageBox msg_box = UIPopupMessageBox.CreatePopupMessageBox(priority);
        msg_box.Title = str_title;
        msg_box.Text = str_text;
        msg_box.OkCallback = ok_callback;
        msg_box.OkParam = ok_param;
        msg_box.MsgBoxType = PopupMsgBoxType.Ok;

        UIManager.Instance.OpenPopup(msg_box);
    }
    public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param, PopupPriority priority = DefaultPriority)
    {
        UIPopupMessageBox msg_box = UIPopupMessageBox.CreatePopupMessageBox(priority);
        msg_box.Title = str_title;
        msg_box.Text = str_text;
        msg_box.OkText = ok_text;
        msg_box.OkCallback = ok_callback;
        msg_box.OkParam = ok_param;
        msg_box.MsgBoxType = PopupMsgBoxType.Ok;

        UIManager.Instance.OpenPopup(msg_box);
    }

    //OK Cancel
    public static void Alert(string str_title, string str_text, BtnClickCallBack ok_callback, object ok_param, BtnClickCallBack cancel_callback, object cancel_param, 
        PopupPriority priority = DefaultPriority)
    {
        UIPopupMessageBox msg_box = UIPopupMessageBox.CreatePopupMessageBox(priority);
        msg_box.Title = str_title;
        msg_box.Text = str_text;
        msg_box.OkCallback = ok_callback;
        msg_box.OkParam = ok_param;
        msg_box.CancelCallback = cancel_callback;
        msg_box.CancelParam = cancel_param;
        msg_box.MsgBoxType = PopupMsgBoxType.OkCancel;

        UIManager.Instance.OpenPopup(msg_box);
    }
    public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param, 
        string cancel_text, BtnClickCallBack cancel_callback, object cancel_param, PopupPriority priority = DefaultPriority)
    {
        UIPopupMessageBox msg_box = UIPopupMessageBox.CreatePopupMessageBox(priority);
        msg_box.Title = str_title;
        msg_box.Text = str_text;
        msg_box.OkText = ok_text;
        msg_box.OkCallback = ok_callback;
        msg_box.OkParam = ok_param;
        msg_box.CancelText = cancel_text;
        msg_box.CancelCallback = cancel_callback;
        msg_box.CancelParam = cancel_param;
        msg_box.MsgBoxType = PopupMsgBoxType.OkCancel;

        UIManager.Instance.OpenPopup(msg_box);
    }
    //OK Cancel Close
    public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param, 
        string cancel_text, BtnClickCallBack cancel_callback, object cancel_param, BtnClickCallBack close_callback, object close_param, PopupPriority priority = DefaultPriority)
    {
        UIPopupMessageBox msg_box = UIPopupMessageBox.CreatePopupMessageBox(priority);
        msg_box.Title = str_title;
        msg_box.Text = str_text;
        msg_box.OkText = ok_text;
        msg_box.OkCallback = ok_callback;
        msg_box.OkParam = ok_param;
        msg_box.CancelText = cancel_text;
        msg_box.CancelCallback = cancel_callback;
        msg_box.CancelParam = cancel_param;
        msg_box.CloseCallback = close_callback;
        msg_box.CloseParam = close_param;
        msg_box.MsgBoxType = PopupMsgBoxType.OkCancelClose;

        UIManager.Instance.OpenPopup(msg_box);

    }
    #endregion


    #region IUIPopupBase
    public PopupPriority Priority
    {
        get { return m_priority; }
        set { m_priority = value; }
    }

    public void Open()
    {
        switch(MsgBoxType)
        {
            case PopupMsgBoxType.Ok:
                UIMessageBox.Alert(Title, Text, OkText, OnOkClicked, this);
                break;
            case PopupMsgBoxType.OkCancel:
                UIMessageBox.Alert(Title, Text, OkText, OnOkClicked, this, CancelText, OnCancelClicked, this);
                break;
            case PopupMsgBoxType.OkCancelClose:
                UIMessageBox.Alert(Title, Text, OkText, OnOkClicked, this, CancelText, OnCancelClicked, this, OnCloseClicked, this);
                break;
            case PopupMsgBoxType.OkClose:
                UIMessageBox.Alert(Title, Text, OkText, OnOkClicked, this, OnCloseClicked, this);
                break;
        }
    }

    public void Close(bool clear_callback = true)
    {
        if(clear_callback)
        {//确定关闭
            UIMessageBox.ClearCallback();
            RecyclableObject.Recycle(this);
        }
        else
        {//隐藏先压到列表
            UIMessageBox.SetShow(false);
        }
    }
    #endregion

    #region IRecyclable

    public void Reset()
    {
        m_priority = PopupPriority.Invalid;
        m_msg_box_type = PopupMsgBoxType.Ok;
        m_title = "";
        m_text = "";

        m_ok_text = null;
        m_ok_callback = null;
        m_ok_param = null;

        m_cancel_text = null;
        m_cancel_callback = null;
        m_cancel_param = null;

        m_close_callback = null;
        m_close_param = null;
    }
    #endregion

    #region 属性
    public PopupMsgBoxType MsgBoxType
    {
        get { return m_msg_box_type; }
        set { m_msg_box_type = value; }
    }
    public string Title { get { return m_title; } set { m_title = value; } }
    public string Text { get { return m_text; } set { m_text = value; } }
    public string OkText { get { return m_ok_text; } set { m_ok_text = value; } }
    public BtnClickCallBack OkCallback { get { return m_ok_callback; } set { m_ok_callback = value; } }
    public object OkParam { get { return m_ok_param; } set { m_ok_param = value; } }
    public string CancelText { get { return m_cancel_text; } set { m_cancel_text = value; } }
    public BtnClickCallBack CancelCallback { get { return m_cancel_callback; } set { m_cancel_callback = value; } }
    public object CancelParam { get { return m_cancel_param; } set { m_cancel_param = value; } }
    public BtnClickCallBack CloseCallback { get { return m_close_callback; } set { m_close_callback = value; } }
    public object CloseParam { get { return m_close_param; } set { m_close_param = value; } }

    #endregion

    #region 回调
    public static void OnOkClicked(object param)
    {
        Debug.Log("UIPopupMessageBox OnOkClicked");

        UIPopupMessageBox msg_box = param as UIPopupMessageBox;
        if(msg_box != null)
        {
            if(msg_box.OkCallback != null)
            {
                msg_box.OkCallback(msg_box.OkParam);
                msg_box.OkCallback = null;
            }
            UIManager.Instance.ClosePopup(msg_box);
        }
    }
    public static void OnCancelClicked(object param)
    {
        Debug.Log("UIPopupMessageBox OnCancelClicked");

        UIPopupMessageBox msg_box = param as UIPopupMessageBox;
        if (msg_box != null)
        {
            if (msg_box.CancelCallback != null)
            {
                msg_box.CancelCallback(msg_box.CancelParam);
                msg_box.CancelCallback = null;
            }
            UIManager.Instance.ClosePopup(msg_box);
        }
    }
    public static void OnCloseClicked(object param)
    {
        Debug.Log("UIPopupMessageBox OnCloseClicked");

        UIPopupMessageBox msg_box = param as UIPopupMessageBox;
        if (msg_box != null)
        {
            if (msg_box.CloseCallback != null)
            {
                msg_box.CloseCallback(msg_box.CloseParam);
                msg_box.CloseCallback = null;
            }
            UIManager.Instance.ClosePopup(msg_box);
        }
    }
    #endregion

    #region intrenal
    static UIPopupMessageBox CreatePopupMessageBox(PopupPriority priority)
    {
        UIPopupMessageBox msg_box = RecyclableObject.Create<UIPopupMessageBox>();
        msg_box.Priority = priority;
        return msg_box;
    }

    static void RecyclePopupMessageBox(UIPopupMessageBox popup)
    {
        RecyclableObject.Recycle(popup);
        popup = null;
    }
    #endregion
}
public delegate void BtnClickCallBack(object obj);