using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISampleB : UIBase {

    public void OnBtnClick(GameObject button, bool isPress)
    {
        UIBase.ShowUI("UISampleC");
    }
    public void OnBtnClose(GameObject button, bool isPress)
    {
        Close();
    }

    public override void OnShow()
    {
        gameObject.GetComponent<Animator>().SetBool("OnShow", true);
    }

    public override void OnHide()
    {
        gameObject.GetComponent<Animator>().SetBool("OnShow", false);
    }
}
