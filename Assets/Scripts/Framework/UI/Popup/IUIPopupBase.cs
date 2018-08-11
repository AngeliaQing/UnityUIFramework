using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
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