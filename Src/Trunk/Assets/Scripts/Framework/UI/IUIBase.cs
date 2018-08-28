using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public interface IUIBase
    {
        void OnShow(object data);
        void OnHide();

        IEnumerator LoadData(UIAsyncRequestResult res);
        void UpdateUIByDefaultDataOnShow();
        void UpdateUIOnShow();

        IEnumerator PlayEnterAnim();
        IEnumerator PlayLeaveAnim();

        //Get
        bool IsStateUI { get; }
        UIName Name { get; set; }
        GameObject GameObject { get; }
        List<UIName> MateUIList { get; }
    }

    public interface INGUIInterface
    {
        UIPanel[] Panels { get; }
    }

    public class UIAsyncRequestResult : IRecyclable
    {
        public bool Success
        {
            get; set;
        }

        public void Reset()
        {

        }
    }
}