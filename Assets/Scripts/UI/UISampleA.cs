using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISampleA : UIBase, IUIEventListener
{
    UIEventListenerContext m_event_listener_context;

    public override void Awake()
    {
        base.Awake();
        this.CareCategory = EasyTouchEventCategoty.ETEC_SimpleTap + EasyTouchEventCategoty.ETEC_Drag;
        m_event_listener_context = UIEventListenerContext.Create(this);
    }

    public override void OnSimpleTap(Gesture gesture)
    {
        //Debug.LogError("OK, OnSimpleTap");
    }

    public void OnBtnClick(GameObject button, bool isPress)
    {
        UIBase.ShowUI("UISampleB", 22);
    }
    public void OnBtnClose(GameObject button, bool isPress)
    {
        Close();
    }


    public override void OnShow(object data)
    {
        RegisterEasyTouch(CareCategory);
        UIEventDispatcher.Instance.AddListener(1, m_event_listener_context);
    }
    public override void OnHide()
    {
        UnregisterEasyTouch();
        UIEventDispatcher.Instance.RemoveListener(1, m_event_listener_context);
    }

    #region IUIEventListener
    public void ReceiveEvent(int event_type, System.Object event_data = null)
    {
        Debug.LogError("UISampleA ReceiveEvent "+ (string)event_data);
    }
    #endregion

    public void YqqCallBack(string data)
    {
        Debug.LogError("UISampleA YqqCallBack " + data);
    }
}
