using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public interface IUIBase
    {
        void OnShow();
        void OnHide();

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