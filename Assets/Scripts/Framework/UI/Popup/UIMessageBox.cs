using UnityEngine;
using System.Collections;

namespace YUIFramework
{
    /*
     * 利用NGUI做的MessageBox示例
     * 每个项目对于系统弹框可以做成自己的样式，但是一个项目最好规范成只做一种
     */
    public class UIMessageBox : SingletonMonoBehaviour<UIMessageBox>
    {
        public Transform m_btn_ok;
        public Transform m_btn_cancel;
        public Transform m_btn_close;

        public UILabel m_label_ok;
        public UILabel m_label_cancel;

        public UILabel m_label_title;
        public UILabel m_label_text;

        static Vector3 m_ok_original_pos;
        static Vector3 m_cancel_original_pos;
        static string m_ok_original_name;
        static string m_cancel_original_name;

        static BtnClickCallBack m_ok_callback = null;
        static BtnClickCallBack m_cancel_callback = null;
        static BtnClickCallBack m_close_callback = null;

        static object m_ok_param = null;
        static object m_cancel_param = null;
        static object m_close_param = null;

        void Awake()
        {
            if (m_btn_ok != null)
                m_ok_original_pos = m_btn_ok.localPosition;
            if (m_btn_cancel != null)
                m_cancel_original_pos = m_btn_cancel.localPosition;

            if (m_label_ok != null)
                m_ok_original_name = m_label_ok.text;
            if (m_label_cancel != null)
                m_cancel_original_name = m_label_cancel.text;
            SetShow(false);
        }

        #region 一些对外的静态接口
        //OK
        public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param)
        {
            ClearCallback();

            SetTitleText(str_title);
            SetText(str_text);

            SetOkButtonText(ok_text);
            UIMessageBox.Instance.m_btn_ok.localPosition = (m_ok_original_pos + m_cancel_original_pos) / 2f;
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_ok.gameObject, true);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_cancel.gameObject, false);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_close.gameObject, false);

            m_ok_callback = ok_callback;
            m_ok_param = ok_param;

            SetShow(true);
        }
        //OK Cancel
        public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param, 
            string cancel_text, BtnClickCallBack cancel_callback, object cancel_param)
        {
            ClearCallback();

            SetTitleText(str_title);
            SetText(str_text);

            SetOkButtonText(ok_text);
            UIMessageBox.Instance.m_btn_ok.localPosition = m_ok_original_pos;
            SetCancelButtonText(cancel_text);
            UIMessageBox.Instance.m_btn_cancel.localPosition = m_cancel_original_pos;
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_ok.gameObject, true);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_cancel.gameObject, true);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_close.gameObject, false);

            m_ok_callback = ok_callback;
            m_ok_param = ok_param;
            m_cancel_callback = cancel_callback;
            m_cancel_param = cancel_param;

            SetShow(true);
        }
        //OK Close
        public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param,
            BtnClickCallBack close_callback, object close_param)
        {
            ClearCallback();

            SetTitleText(str_title);
            SetText(str_text);

            SetOkButtonText(ok_text);
            UIMessageBox.Instance.m_btn_ok.localPosition = (m_ok_original_pos + m_cancel_original_pos) / 2f;
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_ok.gameObject, true);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_cancel.gameObject, false);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_close.gameObject, true);

            m_ok_callback = ok_callback;
            m_ok_param = ok_param;
            m_close_callback = close_callback;
            m_close_param = close_param;

            SetShow(true);
        }
        //OK Cancel Close
        public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param,
            string cancel_text, BtnClickCallBack cancel_callback, object cancel_param, BtnClickCallBack close_callback, object close_param)
        {
            ClearCallback();

            SetTitleText(str_title);
            SetText(str_text);

            SetOkButtonText(ok_text);
            UIMessageBox.Instance.m_btn_ok.localPosition = m_ok_original_pos;
            SetCancelButtonText(cancel_text);
            UIMessageBox.Instance.m_btn_cancel.localPosition = m_cancel_original_pos;
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_ok.gameObject, true);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_cancel.gameObject, true);
            UIHelper.SetActive(UIMessageBox.Instance.m_btn_close.gameObject, true);

            m_ok_callback = ok_callback;
            m_ok_param = ok_param;
            m_cancel_callback = cancel_callback;
            m_cancel_param = cancel_param;
            m_close_callback = close_callback;
            m_close_param = close_param;

            SetShow(true);
        }


        public static void SetShow(bool show)
        {
            UIHelper.SetActive(UIMessageBox.Instance.gameObject, show);
        }
        public static void ClearCallback()
        {
            m_ok_callback = null;
            m_cancel_callback = null;
            m_close_callback = null;

            m_ok_param = null;
            m_cancel_param = null;
            m_close_param = null;
        }
        #endregion

        #region 内部UI设置
        static void SetText(string text)
        {
            UIMessageBox msg_box = UIMessageBox.Instance;
            if (msg_box == null)
                return;
            if (msg_box.m_label_text == null)
                return;
            msg_box.m_label_text.text = text;
        }
        static void SetTitleText(string text)
        {
            UIMessageBox msg_box = UIMessageBox.Instance;
            if (msg_box == null)
                return;
            if (msg_box.m_label_text == null)
                return;
            msg_box.m_label_title.text = text;
        }

        static void SetOkButtonText(string text)
        {
            if (text == null)
                text = m_ok_original_name;
            SetButtonText(text, UIMessageBox.Instance.m_label_ok);
        }
        static void SetCancelButtonText(string text)
        {
            if (text == null)
                text = m_cancel_original_name;
            SetButtonText(text, UIMessageBox.Instance.m_label_cancel);
        }
        //这个函数内的参数根据具体项目来定
        static void SetButtonText(string text, UILabel label)
        {
            if (label == null)
                return;
            if(text.Length > 2)
            {
                label.spacingX = 0;
            }
            else
            {
                label.spacingX = 15;
            }
            label.text = text;
        }
        #endregion

        public void OnClickBtn(GameObject obj)
        {
            if(obj == UIMessageBox.Instance.m_btn_ok.gameObject)
            {
                SetShow(false);
                if (m_ok_callback != null)
                    m_ok_callback(m_ok_param);
            }
            else if(obj == UIMessageBox.Instance.m_btn_cancel.gameObject)
            {
                SetShow(false);
                if (m_cancel_callback != null)
                    m_cancel_callback(m_cancel_param);
            }
            else if (obj == UIMessageBox.Instance.m_btn_close.gameObject)
            {
                SetShow(false);
                if (m_close_callback != null)
                    m_close_callback(m_close_param);
            }
        }
    }

}