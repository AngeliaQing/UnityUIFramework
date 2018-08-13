using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public class NGUILayerManagerHandler : IUILayerManagerHandler
    {
        // 所有打开的UI的所有UIPanel的depth
        static List<int> ms_all_depths = new List<int>();
        // 当前最顶层的UIPanel的深度
        static int ms_cur_top_depth = MAIN_UI_MAX_DEPTH;

        #region 常量
        // 主UI内的所有UIPanel的depth必须小于MAIN_UI_MAX_DEPTH，非主UI的UIPanel的depth会调整为从MAIN_UI_MAX_DEPTH起
        const int MAIN_UI_MAX_DEPTH = 20;
        #endregion

        public void Forward(IUIBase ui)
        {
            if (ui == null)
                return;
            INGUIInterface ngui_base = ui as INGUIInterface;
            if (ngui_base == null)
                return;
            
            ms_all_depths.Sort((s1, s2) => s2 - s1);
            if (ms_all_depths.Count > 0)
                ms_cur_top_depth = ms_all_depths[0];
            for (int i = 0; i < ngui_base.Panels.Length; ++i)
            {
                ngui_base.Panels[i].depth = ms_cur_top_depth + i + 1;
                ms_all_depths.Add(ngui_base.Panels[i].depth);
            }
            ms_cur_top_depth = ms_cur_top_depth + ngui_base.Panels.Length;
        }
        public void Backward(IUIBase ui)
        {
            if (ui == null)
                return;
            INGUIInterface ngui_base = ui as INGUIInterface;
            if (ngui_base == null)
                return;

            for (int i = 0; i < ngui_base.Panels.Length; i++)
            {
                ms_all_depths.Remove(ngui_base.Panels[i].depth);
                ngui_base.Panels[i].depth = i;
            }
            if (ms_all_depths.Count > 0)
                ms_cur_top_depth = ms_all_depths[0];
            else
                ms_cur_top_depth = MAIN_UI_MAX_DEPTH;
        }

        public void OnCloseAllShowedUI()
        {
            ms_all_depths.Clear();
            ms_cur_top_depth = MAIN_UI_MAX_DEPTH;
        }
    }
}