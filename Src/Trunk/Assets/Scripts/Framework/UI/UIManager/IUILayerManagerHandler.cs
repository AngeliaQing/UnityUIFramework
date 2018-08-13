using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public interface IUILayerManagerHandler
    {
        void Forward(IUIBase ui);
        void Backward(IUIBase ui);
        void OnCloseAllShowedUI();
    }
}