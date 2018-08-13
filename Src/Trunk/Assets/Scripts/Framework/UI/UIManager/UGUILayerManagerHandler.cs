using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public class UGUILayerManagerHandler : MonoBehaviour, IUILayerManagerHandler
    {
        public void Forward(IUIBase ui)
        {
            if (ui == null)
                return;
            UIBase ui_base = ui as UIBase;
            if (ui_base == null)
                return;
            RectTransform rt = ui_base.gameObject.GetComponent<RectTransform>();
            if(rt != null)
            {
                rt.localScale = Vector3.one;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector3.one;

                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector3.zero;
                rt.SetAsLastSibling();
            }
        }
        public void Backward(IUIBase ui)
        {

        }

        public void OnCloseAllShowedUI()
        {

        }
    }
}