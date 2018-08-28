using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUIFramework;

public class UISampleA : UIBase, IUIEventListener
{
    UIEventListenerContext m_event_listener_context;

    bool a = false;
    protected override void Awake()
    {
        base.Awake();
        this.CareCategory = EasyTouchEventCategoty.ETEC_SimpleTap + EasyTouchEventCategoty.ETEC_Drag;
        m_event_listener_context = UIEventListenerContext.Create(this);

        //StartCoroutine(Test());
        //Debug.LogError("Test1");
        //a = false;
    }

    IEnumerator Test()
    {
        a = true;
        Debug.LogError("Test2");
        yield return new WaitForSeconds(1);
        Debug.LogError("Test3");
        a = false;
    }

    public override void OnSimpleTap(Gesture gesture)
    {
        Debug.LogError("OK, OnSimpleTap");
    }

    public void OnBtnClick()
    {
        Debug.LogError("UISampleA OnBtnClick");
        UIBase.ShowUI(UIName.UISampleB, 22);
    }
    public void OnBtnClose()
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
        Debug.LogError("Test UISampleA ReceiveEvent "+ (string)event_data+" from combat");
    }
    #endregion

    public void YqqCallBack(string data)
    {
        Debug.LogError("UISampleA YqqCallBack " + data);
    }
}
