using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIEasyTouch
{
    void RegisterEasyTouch(int care_category);
    void UnregisterEasyTouch();
}

public class UIInputListener : MonoBehaviour, IUIEasyTouch, InputHandler
{
    #region EasyTouch事件：使用前需要RegisterEasyTouch，使用完了需要UnregisterEasyTouch
    int m_priority = 999;
    int m_care_category = 0;
    public void RegisterEasyTouch(int care_category)
    {
        m_care_category = care_category;
        InputManager.Instance.RegisterInputListener(this);
    }
    public void UnregisterEasyTouch()
    {//yqqtodo 暂时为了屏蔽错误
     //InputManager instance = InputManager.Instance;
     //if (instance != null)
     //    instance.UnregisterInputListener(this);
    }
    public void Care(int care_category)
    {
        m_care_category |= care_category;
    }

    public void AddCamera(Camera camera)
    {
        // 添加easy touch摄像机（是添加不是设置）
        InputManager.Instance.SetCamera(camera);
    }

    // 以下的都是实现InputHandler接口
    public int CareCategory
    {
        get { return m_care_category; }
        set { m_care_category = value; }
    }
    public virtual int Priority
    {
        get { return m_priority; }
        set { m_priority = value; }
    }
    public virtual void OnCancel(Gesture gesture) { }
    public virtual void OnTouchStart(Gesture gesture) { }
    public virtual void OnTouchDown(Gesture gesture) { }
    public virtual bool OnTouchUp(Gesture gesture) { return false; }
    public virtual void OnSimpleTap(Gesture gesture) { }
    public virtual void OnDoubleTap(Gesture gesture) { }
    public virtual void OnLongTapStart(Gesture gesture) { }
    public virtual void OnLongTap(Gesture gesture) { }
    public virtual void OnLongTapEnd(Gesture gesture) { }
    public virtual void OnDragStart(Gesture gesture) { }
    public virtual void OnDrag(Gesture gesture) { }
    public virtual void OnDragEnd(Gesture gesture) { }
    public virtual void OnSwipeStart(Gesture gesture) { }
    public virtual void OnSwipe(Gesture gesture) { }
    public virtual void OnSwipeEnd(Gesture gesture) { }
    public virtual void OnTouchStart2Fingers(Gesture gesture) { }
    public virtual void OnTouchDown2Fingers(Gesture gesture) { }
    public virtual void OnTouchUp2Fingers(Gesture gesture) { }
    public virtual void OnSimpleTap2Fingers(Gesture gesture) { }
    public virtual void OnDoubleTap2Fingers(Gesture gesture) { }
    public virtual void OnLongTapStart2Fingers(Gesture gesture) { }
    public virtual void OnLongTap2Fingers(Gesture gesture) { }
    public virtual void OnLongTapEnd2Fingers(Gesture gesture) { }
    public virtual void OnTwist(Gesture gesture) { }
    public virtual void OnTwistEnd(Gesture gesture) { }
    public virtual void OnPinchIn(Gesture gesture) { }
    public virtual void OnPinchOut(Gesture gesture) { }
    public virtual void OnPinchEnd(Gesture gesture) { }
    public virtual void OnDragStart2Fingers(Gesture gesture) { }
    public virtual void OnDrag2Fingers(Gesture gesture) { }
    public virtual void OnDragEnd2Fingers(Gesture gesture) { }
    public virtual void OnSwipeStart2Fingers(Gesture gesture) { }
    public virtual void OnSwipe2Fingers(Gesture gesture) { }
    public virtual void OnSwipeEnd2Fingers(Gesture gesture) { }
    #endregion
}
