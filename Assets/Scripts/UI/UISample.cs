using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISample : UIBase {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBtnClick(GameObject button, bool isPress)
    {
        UIManager.Instance.ShowUI("UISample2");
    }

    public override void OnShow()
    {
        base.OnShow();
    }
}
