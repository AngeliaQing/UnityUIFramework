using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUIFramework;

public class TestUI : MonoBehaviour {

	// Use this for initialization
	void Awake () {

        UIBase.ShowUI("UISampleA");
	}
    void Start()
    {
        StartCoroutine(TestSendEvent());
    }

    IEnumerator TestSendEvent()
    {
        yield return new WaitForSeconds(5);
        UIEventDispatcher.Instance.NotifyUI(1, "Today Is Saturday");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
