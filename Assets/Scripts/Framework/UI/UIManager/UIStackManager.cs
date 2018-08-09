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

        Stack<IUIBase> m_stack_cache = new Stack<IUIBase>();

        public void OnShowMainUI(IUIBase new_ui)
        {
            if (ms_cur_main_ui != null && ms_cur_main_ui != new_ui)
            {
                if (ms_main_ui_stack.Contains(new_ui))
                {
                    m_stack_cache.Clear();

                    m_stack_cache.Push(ms_cur_main_ui);
                    while(ms_main_ui_stack.Count > 0)
                    {
                        IUIBase pop_ui = ms_main_ui_stack.Pop();
                        if (pop_ui == new_ui)
                            break;
                        else
                            m_stack_cache.Push(pop_ui);
                    }

                    while(m_stack_cache.Count > 0)
                    {
                        IUIBase ui = m_stack_cache.Pop();
                        ms_main_ui_stack.Push(ui);
                    }
                }
                else
                {
                    ms_main_ui_stack.Push(ms_cur_main_ui);
                }
                
            }
            ms_cur_main_ui = new_ui;
        }
        public IUIBase OnHideMainUI(IUIBase ui)
        {
            if (ms_cur_main_ui == ui && ms_main_ui_stack.Count > 0)
            {
                ms_cur_main_ui = ms_main_ui_stack.Pop();
                return ms_cur_main_ui;
            }
            return null;
        }
        public void ClearUIStack()
        {
            ms_main_ui_stack.Clear();
            ms_cur_main_ui = null;
        }

        public void PrintStack()
        {
            return;
            string log = "";
            foreach(IUIBase ui in ms_main_ui_stack)
            {
                log += ui.Name + " ";
            }
            Debug.LogError(log);
        }
    }
}