using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public class UIRegisterInfo
    {
        public string m_ui_path = "";
        //显示之前先加载数据，加载失败则不打开UI
        public bool m_load_data_before_show = false;
        public UIRegisterInfo(string ui_path, bool load_data_before_show)
        {
            m_ui_path = ui_path;
            m_load_data_before_show = load_data_before_show;
        }
    }

    public class UIRegisterManager : MonoBehaviour
    {
        Dictionary<string, UIRegisterInfo> m_ui_name2register_info = new Dictionary<string, UIRegisterInfo>();

        void Awake()
        {
            RegisterUI();
        }
        // UI注册
        public void RegisterUI()
        {
            RegisterUI("UISampleA");
            RegisterUI("UISampleB", "UIDirTest/");
            RegisterUI("UISampleC");
        }

        public string GetUIPath(string ui_name)
        {
            if (m_ui_name2register_info.ContainsKey(ui_name))
                return m_ui_name2register_info[ui_name].m_ui_path;
            return "";
        }

        #region internal
        void RegisterUI(string ui_name, string ui_path = "", bool load_data_before_show = false)
        {
            UIRegisterInfo register_info = new UIRegisterInfo(ui_path, load_data_before_show);
            m_ui_name2register_info[ui_name] = register_info;
        }
        #endregion
    }
}
