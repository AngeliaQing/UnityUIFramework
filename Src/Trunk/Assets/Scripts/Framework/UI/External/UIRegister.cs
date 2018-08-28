using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUIFramework
{
    public enum UIName
    {
        None,
        UILock,
        UISampleA,
        UISampleB,
        UISampleC,

        //以下是项目里的
    }

    /*
     * UI注册管理器
     * 所有继承UIBase的UI面板都需要在这里进行注册
    */
    public class UIRegister : MonoBehaviour
    {
        Dictionary<int, UIRegisterInfo> m_ui_name2register_info = new Dictionary<int, UIRegisterInfo>();

        public const string UI_DIR = "UI/";
        void Awake()
        {
            RegisterUI();
        }
        // UI注册
        void RegisterUI()
        {
#if USE_NGUI
            RegisterUI(UIName.UILock,"Common/NGUI/");
            RegisterUI(UIName.UISampleA, "Example/NGUI/");
            RegisterUI(UIName.UISampleB, "Example/NGUI/UIDirTest/", true);
            RegisterUI(UIName.UISampleC, "Example/NGUI/");
#else
            RegisterUI(UIName.UILock, "Common/UGUI/");
            RegisterUI(UIName.UISampleA, "Example/UGUI/");
            RegisterUI(UIName.UISampleB, "Example/UGUI/UIDirTest/", true);
            RegisterUI(UIName.UISampleC, "Example/UGUI/");

#endif
            //以下是项目里的
        }

        public string GetUIPath(UIName ui_name)
        {
            int i_ui_name = (int)ui_name;
            if (m_ui_name2register_info.ContainsKey(i_ui_name))
                return m_ui_name2register_info[i_ui_name].m_ui_path;
            return "";
        }
        public bool NeedLoadDataBeforeShow(IUIBase ui)
        {
            if (ui == null)
                return false;
            int i_ui_name = (int)ui.Name;
            if (m_ui_name2register_info.ContainsKey(i_ui_name))
                return m_ui_name2register_info[i_ui_name].m_load_data_before_show;
            return false;
        }

    #region internal
        void RegisterUI(UIName ui_name, string ui_path = "", bool load_data_before_show = false)
        {
            UIRegisterInfo register_info = new UIRegisterInfo(ui_path, load_data_before_show);
            m_ui_name2register_info[(int)ui_name] = register_info;
        }
    #endregion
    }

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
}
