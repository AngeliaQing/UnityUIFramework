using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISampleB : UIBase {

    public void OnBtnClick(GameObject button, bool isPress)
    {
        UIBase.ShowUI("UISampleC");
    }
    public void OnBtnClickA(GameObject button, bool isPress)
    {
        CallUIMethod("UISampleA", "YqqCallBack", "i love you");
    }
    public void OnBtnClose(GameObject button, bool isPress)
    {
        Close();
    }

    public override IEnumerator LoadData(UIAsyncRequestResult res)
    {
        Debug.LogError(DateTime.Now.ToString() + " UISampleB LoadData Start...");
        yield return new WaitForSeconds(2);
        res.Success = false;
        if(!res.Success)
        {
            UIPopupMessageBox.Alert("I am Title", "I am Context...", new BtnClickCallBack(OnBtnClickOK), "yqq");
            yield return new WaitForSeconds(1);
            UIPopupMessageBox.Alert("I am Title1", "I am Context1...", "Wait", PopupPriority.Network);
        }
        Debug.LogError(DateTime.Now.ToString() + " UISampleB LoadData End...Fail");
    }

    public override void OnShow(object data)
    {
        gameObject.GetComponent<Animator>().SetBool("OnShow", true);
        Debug.LogError(DateTime.Now.ToString() + " UISampleB OnShow data = " + (int)data);
    }
    public override void UpdateUIOnShow()
    {
        Debug.LogError(DateTime.Now.ToString() + " UISampleB UpdateUIOnShow");
    }

    public override void OnHide()
    {
        gameObject.GetComponent<Animator>().SetBool("OnShow", false);
    }

    void OnBtnClickOK(object param)
    {
        Debug.LogError("UISampleB Test System PopupBox Callback " + param);
    }
}
