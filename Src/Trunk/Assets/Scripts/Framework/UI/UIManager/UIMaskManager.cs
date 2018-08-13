using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public class UIMaskManager : MonoBehaviour
    {
        #region 静态
        const string UI_BLACK_MASK_ASSET = "UI/Common/UIBlackMask";
        #endregion
        public void AddMask(IUIBase ui_base, UIEventListener.VoidDelegate call_back)
        {
            GameObject ui_obj = null;
            GameObject mask_obj = DemoUnityResourceManager<GameObject>.Instance.AllocResource(UI_BLACK_MASK_ASSET);
            if (ui_base != null)
            {
                UIBase the_ui_base = ui_base as UIBase;
                if (the_ui_base != null)
                {
                    ui_obj = the_ui_base.gameObject;
                    mask_obj.transform.parent = ui_obj.transform;
                }
            }
            mask_obj.transform.localPosition = Vector3.zero;
            mask_obj.transform.localScale = Vector3.one;
            mask_obj.transform.localEulerAngles = Vector3.zero;

            UISprite sprite = mask_obj.GetComponent<UISprite>();
            if(ui_obj != null)
                sprite.SetAnchor(ui_obj, 0, 0, 0, 0);
            mask_obj.SetActive(true);

            UIEventListener.Get(mask_obj).onClick = call_back;
        }
    }
}