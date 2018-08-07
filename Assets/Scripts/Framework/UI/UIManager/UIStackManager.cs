using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public class UIStackManager : MonoBehaviour
    {
        // 主UI栈（这个栈用于实现这样的功能：先显示A，当B显示时主A隐藏，当关闭B时A显示）
        static Stack<IUIBase> ms_main_ui_stack = new Stack<IUIBase>();
        // 当前的主UI
        static IUIBase ms_cur_main_ui = null;

        public void OnShowMainUI(IUIBase new_ui)
        {
            if(ms_cur_main_ui != null && ms_cur_main_ui != new_ui)
                ms_main_ui_stack.Push(ms_cur_main_ui);
            ms_cur_main_ui = new_ui;
        }
        public IUIBase OnHideMainUI(IUIBase ui)
        {
            if (ms_cur_main_ui == ui && ms_main_ui_stack.Count > 0)
                return ms_main_ui_stack.Pop();
            return null;
        }
        public void ClearUIStack()
        {
            ms_main_ui_stack.Clear();
            ms_cur_main_ui = null;
        }
    }
}