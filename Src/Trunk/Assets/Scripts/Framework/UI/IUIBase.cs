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
        string Name { get; }
        GameObject GameObject { get; }
        List<string> MateUIList { get; }
    }

    public interface INGUIInterface
    {
        UIPanel[] Panels { get; }
    }
}