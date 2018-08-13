using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    /*
     * UI锁 锁住UI的时候不能进行任何UI操作，直到解锁
     */
    public class UILockManager
    {
        List<string> m_lock_types = new List<string>();

        #region 常量
        const string UILockName = "UILock";
        #endregion

        public void LockUI(string lock_type)
        {
            if (!m_lock_types.Contains(lock_type))
                m_lock_types.Add(lock_type);

            if (m_lock_types.Count == 1)
                UIBase.ShowUI(UILockName);
        }

        public void UnLockUI(string lock_type)
        {
            if (m_lock_types.Contains(lock_type))
                m_lock_types.Remove(lock_type);
            if (m_lock_types.Count < 1)
                UIBase.HideUI(UILockName);
        }

        public void UnLockAllUI()
        {
            UIBase.HideUI(UILockName);
            m_lock_types.Clear();
        }
    }
}