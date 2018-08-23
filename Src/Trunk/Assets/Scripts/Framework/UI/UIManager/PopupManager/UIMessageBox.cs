using UnityEngine;
using System.Collections;

namespace YUIFramework
{
    public class UIMessageBox : MonoBehaviour
    {
        public Transform m_btn_ok;
        public Transform m_btn_cancel;
        public Transform m_btn_close;       

        static Vector3 m_ok_original_pos;
        static Vector3 m_cancel_original_pos;

        protected string m_ok_original_name;
        protected string m_cancel_original_name;

        static BtnClickCallBack m_ok_callback = null;
        static BtnClickCallBack m_cancel_callback = null;
        static BtnClickCallBack m_close_callback = null;

        static object m_ok_param = null;
        static object m_cancel_param = null;
        static object m_close_param = null;

        #region pub
        public Transform TransBtnOK { get { return m_btn_ok; } }
        #endregion

        protected virtual void Awake()
        {
            if (m_btn_ok != null)
                m_ok_original_pos = m_btn_ok.localPosition;
            if (m_btn_cancel != null)
                m_cancel_original_pos = m_btn_cancel.localPosition;

            //if (m_label_ok != null)
            //    m_ok_original_name = m_label_ok.text;
            //if (m_label_cancel != null)
            //    m_cancel_original_name = m_label_cancel.text;
            //SetShow(false);
        }

        #region 一些对外的静态接口
        //OK
        public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param)
        {
            ClearCallback();

            UIMessageBox msg_box = UIManager.Instance.GetUIMessageBox();
            msg_box.SetTitleText(str_title);
            msg_box.SetText(str_text);

            msg_box.SetOkButtonText(ok_text);
            msg_box.m_btn_ok.localPosition = (m_ok_original_pos + m_cancel_original_pos) / 2f;

            UIHelper.SetActive(msg_box.m_btn_ok.gameObject, true);
            UIHelper.SetActive(msg_box.m_btn_cancel.gameObject, false);
            UIHelper.SetActive(msg_box.m_btn_close.gameObject, false);

            m_ok_callback = ok_callback;
            m_ok_param = ok_param;

            SetShow(true);
        }
        //OK Cancel
        public static void Alert(string str_title, string str_text, string ok_text, BtnClickCallBack ok_callback, object ok_param, 
            string cancel_text, BtnClickCallBack cancel_callback, object cancel_param)
        {
            ClearCallback();

            UIMessageBox msg_box = UIManager.Instance.GetUIMessageBox();
            msg_box.SetTitleText(str_title);
            msg_box.SetText(str_text);

            msg_box.SetOkButtonText(ok_text);
            msg_box.m_btn_ok.localPosition = m_ok_original_pos;
            msg_box.SetCancelButtonText(cancel_text);
            msg_box.m_btn_cancel.localPosition = m_cancel_original_pos;
            UIHelper.SetActive(msg_box.m_btn_ok.gameObject, true);
            UIHelper.SetActive(msg_box.m_btn_cancel.gameObject, true);
            UIHelper.SetActive(msg_box.m_btn_close.gameObject, false);

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

            UIMessageBox msg_box = UIManager.Instance.GetUIMessageBox();
            msg_box.SetTitleText(str_title);
            msg_box.SetText(str_text);

            msg_box.SetOkButtonText(ok_text);
            msg_box.m_btn_ok.localPosition = (m_ok_original_pos + m_cancel_original_pos) / 2f;
            UIHelper.SetActive(msg_box.m_btn_ok.gameObject, true);
            UIHelper.SetActive(msg_box.m_btn_cancel.gameObject, false);
            UIHelper.SetActive(msg_box.m_btn_close.gameObject, true);

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

            UIMessageBox msg_box = UIManager.Instance.GetUIMessageBox();
            msg_box.SetTitleText(str_title);
            msg_box.SetText(str_text);

            msg_box.SetOkButtonText(ok_text);
            msg_box.m_btn_ok.localPosition = m_ok_original_pos;
            msg_box.SetCancelButtonText(cancel_text);
            msg_box.m_btn_cancel.localPosition = m_cancel_original_pos;
            UIHelper.SetActive(msg_box.m_btn_ok.gameObject, true);
            UIHelper.SetActive(msg_box.m_btn_cancel.gameObject, true);
            UIHelper.SetActive(msg_box.m_btn_close.gameObject, true);

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
            UIHelper.SetActive(UIManager.Instance.GetUIMessageBox().gameObject, show);
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
        public virtual void SetText(string text) { }
        public virtual void SetTitleText(string text) { }
        public virtual void SetOkButtonText(string text) { }
        public virtual void SetCancelButtonText(string text) { }
        #endregion

        public void OnClickBtn(GameObject obj)
        {
            UIMessageBox msg_box = UIManager.Instance.GetUIMessageBox();
            if (obj == msg_box.m_btn_ok.gameObject)
            {
                SetShow(false);
                if (m_ok_callback != null)
                    m_ok_callback(m_ok_param);
            }
            else if(obj == msg_box.m_btn_cancel.gameObject)
            {
                SetShow(false);
                if (m_cancel_callback != null)
                    m_cancel_callback(m_cancel_param);
            }
            else if (obj == msg_box.m_btn_close.gameObject)
            {
                SetShow(false);
                if (m_close_callback != null)
                    m_close_callback(m_close_param);
            }
        }
    }

}