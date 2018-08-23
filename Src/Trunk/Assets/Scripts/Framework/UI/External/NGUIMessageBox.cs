using UnityEngine;
using System.Collections;

namespace YUIFramework
{
    /*
     * 利用NGUI做的MessageBox示例
     * 每个项目对于系统弹框可以做成自己的样式，但是一个项目最好规范成只做一种
     */
    public class NGUIMessageBox : UIMessageBox
    {

        public UILabel m_label_ok;
        public UILabel m_label_cancel;

        public UILabel m_label_title;
        public UILabel m_label_text;


        protected override void Awake()
        {
            base.Awake();

            if (m_label_ok != null)
                m_ok_original_name = m_label_ok.text;
            if (m_label_cancel != null)
                m_cancel_original_name = m_label_cancel.text;
        }

        public override void SetText(string text)
        {
            if (m_label_text == null)
                return;
            m_label_text.text = text;
        }
        public override void SetTitleText(string text)
        {
            if (m_label_text == null)
                return;
           m_label_title.text = text;
        }
        public override void SetOkButtonText(string text)
        {
            if (text == null)
                text = m_ok_original_name;
            SetButtonText(text, m_label_ok);
        }
        public override void SetCancelButtonText(string text)
        {
            if (text == null)
                text = m_cancel_original_name;
            SetButtonText(text, m_label_cancel);
        }
        //这个函数内的参数根据具体项目来定
        void SetButtonText(string text, UILabel label)
        {
            if (label == null)
                return;
            if (text.Length > 2)
            {
                label.spacingX = 0;
            }
            else
            {
                label.spacingX = 15;
            }
            label.text = text;
        }
    }
}