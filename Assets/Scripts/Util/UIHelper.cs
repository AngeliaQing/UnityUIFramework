using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper
{
    static Dictionary<int, string> m_s_int2string = new Dictionary<int, string>();

    public static string GetString(int param)
    {
        if (!m_s_int2string.ContainsKey(param))
        {
            m_s_int2string[param] = param.ToString();
        }
        return m_s_int2string[param];
    }

    public static void SetActive(GameObject obj, bool flag)
    {
        if (obj.activeSelf == flag)
            return;
        obj.SetActive(flag);
    }

    static GameObject m_s_ui_camera;
    public static GameObject GetUICamera()
    {
        if(m_s_ui_camera == null)
        {
            m_s_ui_camera = GameObject.Find("UIRoot/UICamera");
        }
        return m_s_ui_camera;
    }
}
