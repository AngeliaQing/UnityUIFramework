using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUIFramework;

public class UISampleB : UIBase {

    public void OnBtnClick()
    {
        UIBase.ShowUI(UIName.UISampleC);
    }
    public void OnBtnClickA()
    {
        CallUIMethod(UIName.UISampleA, "YqqCallBack", "This is callback with other ui");
    }
    public void OnBtnClose()
    {
        Close();
    }

    public override IEnumerator LoadData(UIAsyncRequestResult res)
    {
        Debug.LogError(DateTime.Now.ToString() + " UISampleB LoadData Start...");
        yield return new WaitForSeconds(2);
        res.Success = true;
        if(!res.Success)
        {
            UIPopupMessageBox.Alert("I am Title", "I am Context...", new BtnClickCallBack(OnBtnClickOK), "yqq");
            yield return new WaitForSeconds(1);
            UIPopupMessageBox.Alert("I am Title1", "I am Context1...", "Wait", PopupPriority.Network);
            Debug.LogError(DateTime.Now.ToString() + " UISampleB LoadData End...Fail");
        }
        else
            Debug.LogError(DateTime.Now.ToString() + " UISampleB LoadData End...Success");
    }

    public override void OnShow(object data)
    {
        //gameObject.GetComponent<Animator>().SetBool("OnShow", true);
        Debug.LogError(DateTime.Now.ToString() + " UISampleB OnShow data = " + (int)data);
    }
    public override void UpdateUIOnShow()
    {
        Debug.LogError(DateTime.Now.ToString() + " UISampleB UpdateUIOnShow");
    }

    public override void OnHide()
    {
        //gameObject.GetComponent<Animator>().SetBool("OnShow", false);
    }

    void OnBtnClickOK(object param)
    {
        Debug.LogError("UISampleB Test System PopupBox Callback " + param);
    }
}
