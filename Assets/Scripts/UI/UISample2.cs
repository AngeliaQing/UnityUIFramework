using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISample2 : UIBase {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
