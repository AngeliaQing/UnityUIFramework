﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public class UIMaskManager : MonoBehaviour
    {
        #region 静态
#if USE_NGUI
        const string UI_BLACK_MASK_ASSET = UIRegister.UI_DIR + "Common/NGUI/UIBlackMask";
#else
        const string UI_BLACK_MASK_ASSET = UIRegister.UI_DIR + "Common/UGUI/UIBlackMask";
#endif
        #endregion
        public void AddMask(IUIBase ui_base, UIEventListener.VoidDelegate call_back)
        {
            GameObject ui_obj = null;
            GameObject mask_obj = UnityResourceManager<GameObject>.Instance.AllocResource(UI_BLACK_MASK_ASSET);
            if (ui_base != null)
            {
                UIBase the_ui_base = ui_base as UIBase;
                if (the_ui_base != null)
                {
                    ui_obj = the_ui_base.gameObject;
                    //mask_obj.transform.parent = ui_obj.transform;
                    mask_obj.transform.SetParent(ui_obj.transform);
                }
            }
            mask_obj.transform.localPosition = Vector3.zero;
            mask_obj.transform.localScale = Vector3.one;
            mask_obj.transform.localEulerAngles = Vector3.zero;

#if USE_NGUI
            UISprite sprite = mask_obj.GetComponent<UISprite>();
            if(ui_obj != null)
                sprite.SetAnchor(ui_obj, 0, 0, 0, 0);
#else
            RectTransform rt = mask_obj.GetComponent<RectTransform>();
            if (rt != null)
                rt.SetAsFirstSibling();
#endif

            mask_obj.SetActive(true);

            UIEventListener.Get(mask_obj).onClick = call_back;
        }
    }
}