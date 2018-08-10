using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public enum PopupPriority
    {
        Invalid,
        Normal,
        Network,
        System,
    }
    public interface IUIPopupBase
    {
        PopupPriority Priority { get; set; }
        void Open();
        void Close(bool clear_callback = true);
    }

    public enum PopupMsgBoxType
    {
        Ok,
        OkCancel,
        OkCancelClose,
        OkClose,
    }
}