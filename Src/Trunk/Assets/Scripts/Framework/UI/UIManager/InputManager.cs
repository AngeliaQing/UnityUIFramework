using UnityEngine;
using System.Collections.Generic;

namespace YUIFramework
{
    public class EasyTouchEventCategoty
    {
        public const int ETEC_None = 0;
        public const int ETEC_All = (int)-1;
        public const int ETEC_Cancel = 1 << 0;
        public const int ETEC_Touch = 1 << 1;
        public const int ETEC_SimpleTap = 1 << 2;
        public const int ETEC_DoubleTap = 1 << 3;
        public const int ETEC_LongTap = 1 << 4;
        public const int ETEC_Drag = 1 << 5;
        public const int ETEC_Swipe = 1 << 6;
        public const int ETEC_Touch2Fingers = 1 << 7;
        public const int ETEC_SimpleTap2Fingers = 1 << 8;
        public const int ETEC_DoubleTap2Fingers = 1 << 9;
        public const int ETEC_LongTap2Fingers = 1 << 10;
        public const int ETEC_Twist = 1 << 11;
        public const int ETEC_Pinch = 1 << 12;
        public const int ETEC_Drag2Fingers = 1 << 13;
        public const int ETEC_Swipe2Fingers = 1 << 14;
    }

    /// <summary>
    /// 输入事件监听器, 每个事件的意义参照EasyTouch说明
    /// </summary>
    interface IInputHandler
    {
        // 优先级, 值约小越优先,比如 1 优先于 12, 如果不关心他的处理顺序请填LAST
        int Priority { get; }
        int CareCategory { get; }
        void OnCancel(Gesture gesture);
        void OnTouchStart(Gesture gesture);
        void OnTouchDown(Gesture gesture);
        bool OnTouchUp(Gesture gesture); // true表示不继续向其他lisnter传递
        void OnSimpleTap(Gesture gesture);
        void OnDoubleTap(Gesture gesture);
        void OnLongTapStart(Gesture gesture);
        void OnLongTap(Gesture gesture);
        void OnLongTapEnd(Gesture gesture);
        void OnDragStart(Gesture gesture);
        void OnDrag(Gesture gesture);
        void OnDragEnd(Gesture gesture);
        void OnSwipeStart(Gesture gesture);
        void OnSwipe(Gesture gesture);
        void OnSwipeEnd(Gesture gesture);
        void OnTouchStart2Fingers(Gesture gesture);
        void OnTouchDown2Fingers(Gesture gesture);
        void OnTouchUp2Fingers(Gesture gesture);
        void OnSimpleTap2Fingers(Gesture gesture);
        void OnDoubleTap2Fingers(Gesture gesture);
        void OnLongTapStart2Fingers(Gesture gesture);
        void OnLongTap2Fingers(Gesture gesture);
        void OnLongTapEnd2Fingers(Gesture gesture);
        void OnTwist(Gesture gesture);
        void OnTwistEnd(Gesture gesture);
        void OnPinchIn(Gesture gesture);
        void OnPinchOut(Gesture gesture);
        void OnPinchEnd(Gesture gesture);
        void OnDragStart2Fingers(Gesture gesture);
        void OnDrag2Fingers(Gesture gesture);
        void OnDragEnd2Fingers(Gesture gesture);
        void OnSwipeStart2Fingers(Gesture gesture);
        void OnSwipe2Fingers(Gesture gesture);
        void OnSwipeEnd2Fingers(Gesture gesture);
    }

    /// <summary>
    /// 输入管理器, 支持用户自定义输入处理, 基于EasyTouch支持NGUI输入事件过滤, 被NGUI捕获的输入不会通知到用户处理器,
    /// </summary>
    class InputManager : SingletonMonoBehaviour<InputManager>
    {
        private EasyTouch m_easy_touch = null;
        private List<IInputHandler> m_input_listener_list = new List<IInputHandler>();
        
        void Awake()
        {
            m_easy_touch = gameObject.GetComponent<EasyTouch>();
            if (m_easy_touch == null)
            {
                Debug.LogError("InputManager, EasyTouch is null");
                return;
            }

            //m_easy_touch.enabledNGuiMode = true;
            ClearInterceptiveCamera();

            if (UIManager.Instance.InterceptiveCameras != null)
            {
                for (int i = 0; i < UIManager.Instance.InterceptiveCameras.Count; ++i)
                {
                    Camera camera = UIManager.Instance.InterceptiveCameras[i];
                    if(camera == null)
                        Debug.LogError("InputManager, UICamera is null");
                    else
                        AddInterceptiveCamera(camera);
                }
            }

            int ui_layer = LayerMask.GetMask("UIDefaultLayer");
            SetInterceptiveCameraLayerMask(ui_layer);

            RegisterInputDelegates();
        }

        protected override void OnDestroy()
        {
            UnregisterInputDelegates();
        }

        public void RegisterInputListener(IInputHandler listener)
        {
            if (m_input_listener_list.Contains(listener))
                return;
            for (int i = 0; i < m_input_listener_list.Count; i++)
            {
                if (m_input_listener_list[i].Priority >= listener.Priority)
                {
                    m_input_listener_list.Insert(i, listener);
                    return;
                }
            }
            m_input_listener_list.Add(listener);
        }

        public void UnregisterInputListener(IInputHandler listener)
        {
            m_input_listener_list.Remove(listener);
        }

        #region InterceptiveCamera Support
        /// <summary>
        /// 增加可以拦截输入事件的摄像机, 如果输入可以捕获到指定摄像机下面的collider, 这个输入事件就不会传递到管理器中
        /// 开启|关闭拦截输入功能
        /// </summary>
        public void EnableInterceptiveCamera(bool enable)
        {
            m_easy_touch.enabledNGuiMode = enable ? m_easy_touch.nGUICameras.Count != 0 : false;
        }

        /// <summary>
        /// 增加可以拦截输入的摄像机
        /// </summary>
        public void AddInterceptiveCamera(Camera camera)
        {
            if (m_easy_touch.nGUICameras.Contains(camera) == false)
                m_easy_touch.nGUICameras.Add(camera);
            //m_easy_touch.enabledNGuiMode = true;
        }

        /// <summary>
        /// 删除可以拦截输入的摄像机	
        /// </summary>
        public void RemoveInterceptiveCamera(Camera camera)
        {
            m_easy_touch.nGUICameras.Remove(camera);
            m_easy_touch.enabledNGuiMode = m_easy_touch.nGUICameras.Count != 0;
        }

        /// <summary>
        /// 清除已设置的所有拦截输入摄像机
        /// </summary>
        public void ClearInterceptiveCamera()
        {
            m_easy_touch.nGUICameras.Clear();
            m_easy_touch.enabledNGuiMode = false;
        }

        /// <summary>
        /// 设置拦截摄像机可以捕获的Layer层
        /// </summary>
        public void SetInterceptiveCameraLayerMask(int layerMaskValue)
        {
            /// TODO : 是否需要对每个摄像机单独设置LayerMask
            m_easy_touch.nGUILayers.value = layerMaskValue;
        }

        /// <summary>
        /// 获取拦截摄像机可以捕获的Layer层
        /// </summary>
        public int GetInterceptiveCameraLayerMask()
        {
            return m_easy_touch.nGUILayers.value;
        }

        /// <summary>
        /// 添加easy touch摄像机
        /// </summary>
        public void SetCamera(Camera camera)
        {
            EasyTouch.AddCamera(camera);
        }
        #endregion

        private void RegisterInputDelegates()
        {
            EasyTouch.On_Cancel += OnCancel;
            EasyTouch.On_TouchStart += OnTouchStart;
            EasyTouch.On_TouchDown += OnTouchDown;
            EasyTouch.On_TouchUp += OnTouchUp;
            EasyTouch.On_SimpleTap += OnSimpleTap;
            EasyTouch.On_DoubleTap += OnDoubleTap;
            EasyTouch.On_LongTapStart += OnLongTapStart;
            EasyTouch.On_LongTap += OnLongTap;
            EasyTouch.On_LongTapEnd += OnLongTapEnd;
            EasyTouch.On_DragStart += OnDragStart;
            EasyTouch.On_Drag += OnDrag;
            EasyTouch.On_DragEnd += OnDragEnd;
            EasyTouch.On_SwipeStart += OnSwipeStart;
            EasyTouch.On_Swipe += OnSwipe;
            EasyTouch.On_SwipeEnd += OnSwipeEnd;
            EasyTouch.On_TouchStart2Fingers += OnTouchStart2Fingers;
            EasyTouch.On_TouchDown2Fingers += OnTouchDown2Fingers;
            EasyTouch.On_TouchUp2Fingers += OnTouchUp2Fingers;
            EasyTouch.On_SimpleTap2Fingers += OnSimpleTap2Fingers;
            EasyTouch.On_DoubleTap2Fingers += OnDoubleTap2Fingers;
            EasyTouch.On_LongTapStart2Fingers += OnLongTapStart2Fingers;
            EasyTouch.On_LongTap2Fingers += OnLongTap2Fingers;
            EasyTouch.On_LongTapEnd2Fingers += OnLongTapEnd2Fingers;
            EasyTouch.On_Twist += OnTwist;
            EasyTouch.On_TwistEnd += OnTwistEnd;
            EasyTouch.On_PinchIn += OnPinchIn;
            EasyTouch.On_PinchOut += OnPinchOut;
            EasyTouch.On_PinchEnd += OnPinchEnd;
            EasyTouch.On_DragStart2Fingers += OnDragStart2Fingers;
            EasyTouch.On_Drag2Fingers += OnDrag2Fingers;
            EasyTouch.On_DragEnd2Fingers += OnDragEnd2Fingers;
            EasyTouch.On_SwipeStart2Fingers += OnSwipeStart2Fingers;
            EasyTouch.On_Swipe2Fingers += OnSwipe2Fingers;
            EasyTouch.On_SwipeEnd2Fingers += OnSwipeEnd2Fingers;
        }

        private void UnregisterInputDelegates()
        {
            EasyTouch.On_Cancel -= OnCancel;
            EasyTouch.On_TouchStart -= OnTouchStart;
            EasyTouch.On_TouchDown -= OnTouchDown;
            EasyTouch.On_TouchUp -= OnTouchUp;
            EasyTouch.On_SimpleTap -= OnSimpleTap;
            EasyTouch.On_DoubleTap -= OnDoubleTap;
            EasyTouch.On_LongTapStart -= OnLongTapStart;
            EasyTouch.On_LongTap -= OnLongTap;
            EasyTouch.On_LongTapEnd -= OnLongTapEnd;
            EasyTouch.On_DragStart -= OnDragStart;
            EasyTouch.On_Drag -= OnDrag;
            EasyTouch.On_DragEnd -= OnDragEnd;
            EasyTouch.On_SwipeStart -= OnSwipeStart;
            EasyTouch.On_Swipe -= OnSwipe;
            EasyTouch.On_SwipeEnd -= OnSwipeEnd;
            EasyTouch.On_TouchStart2Fingers -= OnTouchStart2Fingers;
            EasyTouch.On_TouchDown2Fingers -= OnTouchDown2Fingers;
            EasyTouch.On_TouchUp2Fingers -= OnTouchUp2Fingers;
            EasyTouch.On_SimpleTap2Fingers -= OnSimpleTap2Fingers;
            EasyTouch.On_DoubleTap2Fingers -= OnDoubleTap2Fingers;
            EasyTouch.On_LongTapStart2Fingers -= OnLongTapStart2Fingers;
            EasyTouch.On_LongTap2Fingers -= OnLongTap2Fingers;
            EasyTouch.On_LongTapEnd2Fingers -= OnLongTapEnd2Fingers;
            EasyTouch.On_Twist -= OnTwist;
            EasyTouch.On_TwistEnd -= OnTwistEnd;
            EasyTouch.On_PinchIn -= OnPinchIn;
            EasyTouch.On_PinchOut -= OnPinchOut;
            EasyTouch.On_PinchEnd -= OnPinchEnd;
            EasyTouch.On_DragStart2Fingers -= OnDragStart2Fingers;
            EasyTouch.On_Drag2Fingers -= OnDrag2Fingers;
            EasyTouch.On_DragEnd2Fingers -= OnDragEnd2Fingers;
            EasyTouch.On_SwipeStart2Fingers -= OnSwipeStart2Fingers;
            EasyTouch.On_Swipe2Fingers -= OnSwipe2Fingers;
            EasyTouch.On_SwipeEnd2Fingers -= OnSwipeEnd2Fingers;
        }

        #region EasyTouch Input Delegate
        private void OnCancel(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Cancel) == 0)
                    continue;
                m_input_listener_list[i].OnCancel(gesture);
            }
        }

        private void OnTouchStart(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Touch) == 0)
                    continue;
                m_input_listener_list[i].OnTouchStart(gesture);
            }
        }

        private void OnTouchDown(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Touch) == 0)
                    continue;
                m_input_listener_list[i].OnTouchDown(gesture);
            }
        }

        private void OnTouchUp(Gesture gesture)
        {
            if (UICamera.isOverUI)
                return;
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Touch) == 0)
                    continue;
                bool shouldStop = m_input_listener_list[i].OnTouchUp(gesture);
                if (shouldStop)
                    break;
            }
        }

        private void OnSimpleTap(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_SimpleTap) == 0)
                    continue;
                m_input_listener_list[i].OnSimpleTap(gesture);
            }
        }

        private void OnDoubleTap(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_DoubleTap) == 0)
                    continue;
                m_input_listener_list[i].OnDoubleTap(gesture);
            }
        }

        private void OnLongTapStart(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_LongTap) == 0)
                    continue;
                m_input_listener_list[i].OnLongTapStart(gesture);
            }
        }

        private void OnLongTap(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_LongTap) == 0)
                    continue;
                m_input_listener_list[i].OnLongTap(gesture);
            }
        }

        private void OnLongTapEnd(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_LongTap) == 0)
                    continue;
                m_input_listener_list[i].OnLongTapEnd(gesture);
            }
        }

        private void OnDragStart(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Drag) == 0)
                    continue;
                m_input_listener_list[i].OnDragStart(gesture);
            }
        }

        private void OnDrag(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Drag) == 0)
                    continue;
                m_input_listener_list[i].OnDrag(gesture);
            }
        }

        private void OnDragEnd(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Drag) == 0)
                    continue;
                m_input_listener_list[i].OnDragEnd(gesture);
            }
        }

        private void OnSwipeStart(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Swipe) == 0)
                    continue;
                m_input_listener_list[i].OnSwipeStart(gesture);
            }
        }

        private void OnSwipe(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Swipe) == 0)
                    continue;
                m_input_listener_list[i].OnSwipe(gesture);
            }
        }

        private void OnSwipeEnd(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Swipe) == 0)
                    continue;
                m_input_listener_list[i].OnSwipeEnd(gesture);
            }
        }

        private void OnTouchStart2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Touch2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnTouchStart2Fingers(gesture);
            }
        }

        private void OnTouchDown2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Touch2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnTouchDown2Fingers(gesture);
            }
        }

        private void OnTouchUp2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Touch2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnTouchUp2Fingers(gesture);
            }
        }

        private void OnSimpleTap2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_SimpleTap2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnSimpleTap2Fingers(gesture);
            }
        }

        private void OnDoubleTap2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_DoubleTap2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnDoubleTap2Fingers(gesture);
            }
        }

        private void OnLongTapStart2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_LongTap2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnLongTapStart2Fingers(gesture);
            }
        }

        private void OnLongTap2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_LongTap2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnLongTap2Fingers(gesture);
            }
        }

        private void OnLongTapEnd2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_LongTap2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnLongTapEnd2Fingers(gesture);
            }
        }

        private void OnTwist(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Twist) == 0)
                    continue;
                m_input_listener_list[i].OnTwist(gesture);
            }
        }

        private void OnTwistEnd(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Twist) == 0)
                    continue;
                m_input_listener_list[i].OnTwistEnd(gesture);
            }
        }

        private void OnPinchIn(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Pinch) == 0)
                    continue;
                m_input_listener_list[i].OnPinchIn(gesture);
            }
        }

        private void OnPinchOut(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Pinch) == 0)
                    continue;
                m_input_listener_list[i].OnPinchOut(gesture);
            }
        }

        private void OnPinchEnd(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Pinch) == 0)
                    continue;
                m_input_listener_list[i].OnPinchEnd(gesture);
            }
        }

        private void OnDragStart2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Drag2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnDragStart2Fingers(gesture);
            }
        }

        private void OnDrag2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Drag2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnDrag2Fingers(gesture);
            }
        }

        private void OnDragEnd2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Drag2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnDragEnd2Fingers(gesture);
            }
        }

        private void OnSwipeStart2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Swipe2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnSwipeStart2Fingers(gesture);
            }
        }

        private void OnSwipe2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Swipe2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnSwipe2Fingers(gesture);
            }
        }

        private void OnSwipeEnd2Fingers(Gesture gesture)
        {
            for (int i = 0; i < m_input_listener_list.Count; ++i)
            {
                if ((m_input_listener_list[i].CareCategory & EasyTouchEventCategoty.ETEC_Swipe2Fingers) == 0)
                    continue;
                m_input_listener_list[i].OnSwipeEnd2Fingers(gesture);
            }
        }
        #endregion
    }
}