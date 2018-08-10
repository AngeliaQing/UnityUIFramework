using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public class UIPopupManager
    {
        List<IUIPopupBase> m_popup_list = new List<IUIPopupBase>();
        IUIPopupBase m_cur_popup = null;

        PopupPriority m_popup_filter = PopupPriority.Normal;
        public PopupPriority PriorityFilter
        {
            get { return m_popup_filter; }
            set
            {
                m_popup_filter = value;
                List<IUIPopupBase> delete_popups = new List<IUIPopupBase>();
                for(int i = 0; i < m_popup_list.Count; ++i)
                {
                    if (m_popup_list[i].Priority < m_popup_filter)
                        delete_popups.Add(m_popup_list[i]);
                }
                for (int i = 0; i < delete_popups.Count; ++i)
                {
                    delete_popups[i].Close();
                    m_popup_list.Remove(delete_popups[i]);
                }
            }
        }

        public void OpenPopup(IUIPopupBase popup)
        {
            if (popup == null)
                return;
            if (popup.Priority < PriorityFilter)
                return;

            IUIPopupBase popup_base = m_popup_list.Find((IUIPopupBase p) => { return p == popup; });
            if (popup_base != null)
                return;

            if(m_cur_popup == null || popup.Priority > m_cur_popup.Priority)
            {
                if(m_cur_popup != null)
                {
                    m_cur_popup.Close(false);
                }
                m_cur_popup = popup;
                m_cur_popup.Open();
            }
            else
            {
                popup.Close(false);
                //m_cur_popup.Open();
            }

            int index = m_popup_list.FindIndex((IUIPopupBase p) => { return p.Priority < popup.Priority; });
            if (index < 0)
                m_popup_list.Add(popup);
            else
                m_popup_list.Insert(index, popup);
        }
        public void ClosePopup(IUIPopupBase popup)
        {
            if (popup == null)
                return;
            popup.Close();
            m_popup_list.Remove(popup);

            if(m_cur_popup == popup)
            {
                if (m_popup_list.Count > 0)
                {
                    m_cur_popup = m_popup_list[0];
                    m_cur_popup.Open();
                }
                else
                    m_cur_popup = null;
            }
        }
        public void Clear()
        {
            if(m_cur_popup != null)
            {
                m_cur_popup.Close();
                m_cur_popup = null;
            }
            m_popup_list.Clear();
        }
    }
}