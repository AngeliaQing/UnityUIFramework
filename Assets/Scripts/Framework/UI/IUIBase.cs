using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public interface IUIBase
    {
        void OnShow();
        void OnHide();
        void Destroy();

        bool IsShow();
        bool IsStateUI();

        //void InitializeUIBase();
        void ShowSelf();
        void HideSelf();

        //状态UI相关
        void ShowMateUI();

        //Get
        string UIName { get; }
    }

    public interface INGUIInterface
    {
        UIPanel[] Panels { get; }
    }
}