using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISampleC : UIBase
{
    
    public void OnBtnClick(GameObject button, bool isPress)
    {
        UIBase.ShowUI("UISampleA");
    }
    public void OnBtnClose(GameObject button, bool isPress)
    {
        Close();
    }
}
